using NTCC.NET.Core.Facility;
using NTCC.NET.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NTCC.NET.Core.Stages
{
  public class StageClean : StageBase
  {
    public StageClean(string id) : base(id)
    {
    }

    /// <summary>
    /// Текущий (успешный) проход скребка
    /// </summary>
    public int CurrentPass
    {
      get => currentPass;
      private set
      {
        if (value == currentPass)
          return;
        currentPass = value;
        OnPropertyChanged();
      }
    }
    private int currentPass = 0;

    /// <summary>
    /// Максимальное число проходов скребка до окончания удаления депозита (15 - раз по умолчанию)
    /// </summary>
    public int MaxPassCount
    {
      get => maxPassCount;
      private set
      {
        if (value == maxPassCount)
          return;

        maxPassCount = value;
        OnPropertyChanged();
      }
    }
    private int maxPassCount = 15;


    /// <summary>
    /// Максимальное число попыток перемещения скребка (5 раз по умолчанию)
    /// </summary>
    public int MaxPassAttempts
    {
      get => maxPassAttempts;
      private set
      {
        if (value == maxPassAttempts)
          return;

        maxPassAttempts = value;
        OnPropertyChanged();
      }
    }    
    private int maxPassAttempts = 5;


    /// <summary>
    /// Время ожидания охлождения штоков скребка
    /// </summary>
    private TimeSpan CoolingTime
    {
      get => coolingTime;
      set
      {
        if (value == coolingTime)
          return;

        coolingTime = value;
        OnPropertyChanged();
      }
    }
    private TimeSpan coolingTime = TimeSpan.FromSeconds(5);

    public override StageResult Prepare()
    {
      //задание параметров прогрева
      SetupHeating();

      //включаем ворошитель
      DataPointHelper.SetDiscreteParameter(this, "M01.RUN", true, (int)OperationDelay.TotalMilliseconds);

      //включаем шнек
      DataPointHelper.SetDiscreteParameter(this, "M02.RUN", true, (int)OperationDelay.TotalMilliseconds);

      //отжимаем уплотнения
      DataPointHelper.SetDiscreteParameter(this, "YA02", true, (int)OperationDelay.TotalMilliseconds);

      //запускаем управление заслонкой
      ArtMonbatFacility.Damper.StartControl();

      return StageResult.Successful;
    }

    protected override StageResult Finalization()
    {
      //отправляем команду на перемещение скребка вверх
      Scrapper scrapper = ArtMonbatFacility.Scrapper;
      scrapper.MoveScraperUp();
      
      //выключаем шнек
      DataPointHelper.SetDiscreteParameter(this, "M02.RUN", false, (int)OperationDelay.TotalMilliseconds);

      //выключаем ворошитель
      DataPointHelper.SetDiscreteParameter(this, "M01.RUN", false, (int)OperationDelay.TotalMilliseconds);

      //выключаем подачу азота в скребок
      DataPointHelper.SetDiscreteParameter(this, "YA08.OPN", false, (int)OperationDelay.TotalMilliseconds);

      //выключаем линейный модуль #2
      DataPointHelper.SetDiscreteParameter(this, "YA01.2", false, (int)OperationDelay.TotalMilliseconds);

      //включаем линейный модуль #1
      DataPointHelper.SetDiscreteParameter(this, "YA01.1", true, (int)OperationDelay.TotalMilliseconds);

      //зажимаем уплотнения
      DataPointHelper.SetDiscreteParameter(this, "YA02", false, (int)OperationDelay.TotalMilliseconds);

      //останавливаем управление заслонкой
      ArtMonbatFacility.Damper.StopControl();

      return StageResult.Successful;
    }

    protected override StageResult Main(CancellationToken stop, CancellationToken skip)
    {
      StartTime = DateTime.Now;
      MaxPassCount = StageParameters.PassCount;
      CoolingTime = TimeSpan.FromSeconds(StageParameters.CoolingTime);
      CurrentPass = 1;
      
      //локальный счетчик попыток перемещения скребка
      //счетчик обнуляется при успешном проходе скребка
      int currentAttempt = 0;

      while (CurrentPass < MaxPassCount)
      {
        //проверяем на прерывание стадии пользователем
        if (stop.IsCancellationRequested)
          return StageResult.Stopped;

        //проверяем на пропуск стадии пользователем
        if (skip.IsCancellationRequested)
          return StageResult.Skipped;

        //получаем экземпляр скребка и настраиваем время перемещения вверх и вниз
        Scrapper scrapper = ArtMonbatFacility.Scrapper;
        scrapper.MoveDownTimeOut = TimeSpan.FromSeconds(StageParameters.OneWayTimeout);
        scrapper.MoveUpTimeOut = TimeSpan.FromSeconds(StageParameters.OneWayTimeout);

        try
        {
          //попытка сделать полный проход скребка
          if (scrapper.MakePass())
          {
            OnTick($"Завершен проход [{CurrentPass}] скребка. Ожидаем охлаждения штоков {CoolingTime}...", MessageType.Info);

            //ожидаем охлождение штоков
            Thread.Sleep(CoolingTime);

            //сбрасываем число попыток перемещения скребка и увеличиваем число успешных проходов
            currentAttempt = 0;
            CurrentPass++;
          }
          else
          {
            //увеличиваем число попыток перемещения скребка
            if (++currentAttempt > MaxPassAttempts)
            {
              throw new IndexOutOfRangeException($"Превышено максимальное число попыток перемещения скребка [{MaxPassAttempts}]");
            }
          }
        }
        catch (IndexOutOfRangeException ex)
        {
          string message = $"Проход [{CurrentPass}] скребка не завершен {ex.Message}. Закончите удаление депозита вручную и нажмите [Да] для продолжения технологического цикла.";
          OnTick(message, MessageType.Exception);
          
          //выдаем сообщение пользователю и ожидаем подтверждения
          if (UserConfirmation.Confirm(message))
            return StageResult.Successful;

          //если пользователь не подтвердил продолжение технологического цикла завершаем стадию с неудачей
          return StageResult.Failed;
        }

        catch (Exception ex)
        {
          //увеличиваем число попыток перемещения скребка
          currentAttempt++;
          OnTick($"Проход [{CurrentPass}] скребка не завершен, попытка [{currentAttempt}] : {ex.Message}", MessageType.Exception);

          //ожидаем охлождение штоков
          Thread.Sleep(CoolingTime);
        }
      }

      return StageResult.Successful;
    }
  }
}
