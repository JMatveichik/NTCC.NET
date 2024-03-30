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
    /// Текущий проход скребка
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
    /// Максимальное число проходов скребка
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
    /// Время ожидания охлождения штоков скребка
    /// </summary>
    private TimeSpan CollingTime
    {
      get => collingTime;
      set
      {
        if (value == collingTime)
          return;

        collingTime = value;
        OnPropertyChanged();
      }
    }
    private TimeSpan collingTime = TimeSpan.FromSeconds(5);

    /// <summary>
    /// Oжидания прохода скребка в одном напровлении
    /// </summary>
    /// <param name="positionDataPoint">Датчик положения скребка, который будет проверяться для подтверждения завершения перемещения </param>
    /// <param name="moveTimeOut">Максимальное время перемещения скребка к выбранной позиции</param>
    /// <returns>true - если скребок достиг заданной точки , false - если скребок застрял</returns>
    private bool WaitScraperPosition(DiscreteDataPoint positionDataPoint, TimeSpan moveTimeOut)
    {
      DateTime moveStart = DateTime.Now;
      TimeSpan moveTime = TimeSpan.FromMilliseconds(0);

      //ожидаем положения скребка или таймаута
      while (true)
      {
        Thread.Sleep(50);

        //обновляем время движения скребка
        moveTime = DateTime.Now - moveStart;

        //если превышено время ожидания положения скребка
        if (moveTime > moveTimeOut)
          return false;

        if (positionDataPoint.State)
          return true;
      }
    }

    /// <summary>
    /// Перемещение скребка в заданное положение
    /// </summary>
    /// <param name="moveDown"> Направление перемещения (true - вниз, false - вверх)</param>
    /// <param name="moveTimeOut">Максимальное время перемещения скребка к выбранной позиции</param>
    /// <returns>true - если скребок достиг заданной точки , false - если скребок застрял</returns>
    private bool MoveScraperTo(bool moveDown, TimeSpan moveTimeOut)
    {
      var dataPoints = ArtMonbatFacility.DataPoints;

      DiscreteOutputDataPoint moveTopDataPoint = dataPoints["YA01.1"] as DiscreteOutputDataPoint;
      DiscreteOutputDataPoint moveDownDataPoint = dataPoints["YA01.2"] as DiscreteOutputDataPoint;

      DiscreteDataPoint isScaperOnTop = dataPoints["CS01"] as DiscreteDataPoint;
      DiscreteDataPoint isScaperOnDown = dataPoints["CS02"] as DiscreteDataPoint;

      DiscreteOutputDataPoint nitroToScraper = dataPoints["YA08.OPN"] as DiscreteOutputDataPoint;

      if (moveDown)
      {
        //отключаем линейный модуль скребка #1
        moveTopDataPoint.SetState(false);

        //технологическая задержка
        Thread.Sleep(100);

        //включаем линейный модуль скребка #2
        moveDownDataPoint.SetState(true);

        //технологическая задержка
        Thread.Sleep(100);

        //подача азота в сребок
        nitroToScraper.SetState(true);

        //если не достигли нижнего положения ошибка
        if (!WaitScraperPosition(isScaperOnDown, moveTimeOut))
          return false;

      }
      else
      {
        //снятие подачи азота в сребок
        nitroToScraper.SetState(false);

        //технологическая задержка
        Thread.Sleep(100);

        //выключаем линейный модуль скребка #2
        moveDownDataPoint.SetState(false);

        //технологическая задержка
        Thread.Sleep(100);

        //включаем линейный модуль скребка #1
        moveTopDataPoint.SetState(true);

        //если не достигли верхнего положения ошибка
        if (!WaitScraperPosition(isScaperOnTop, moveTimeOut))
          return false;
      }

      return true;
    }

    /// <summary>
    /// Перемещение скребка вниз
    /// </summary>
    /// <param name="moveTimeOut">Максимальное время перемещения скребка</param>
    /// <returns>true - если скребок достиг нижнего положения в пределах заданного времени, false - если скребок застрял</returns>
    private bool MoveScraperDown(TimeSpan moveTimeOut)
    {
      return MoveScraperTo(true, moveTimeOut);
    }

    /// <summary>
    /// Перемещение скребка вверх
    /// </summary>
    /// <param name="moveTimeOut">Максимальное время перемещения скребка</param>
    /// <returns>True - если скребок достиг верхнего положения в пределах заданного времени, false - если скребок застрял</returns>
    private bool MoveScraperUp(TimeSpan moveTimeOut)
    {
      return MoveScraperTo(false, moveTimeOut);
    }

    public override StageResult Prepare()
    {
      //задание параметров прогрева
      SetupHeating();

      //включаем ворошитель
      DataPointHelper.SetDiscreteParameter(this, "M01.RUN", true, (int)OperationDelay.TotalMilliseconds);

      //включаем шнек
      DataPointHelper.SetDiscreteParameter(this, "M02.RUN", true, (int)OperationDelay.TotalMilliseconds);

      //отжимаем уплотнения
      DataPointHelper.SetDiscreteParameter(this, "YA02", false, (int)OperationDelay.TotalMilliseconds);

      //запускаем управление заслонкой
      ArtMonbatFacility.Damper.StartControl();

      return StageResult.Successful;
    }

    protected override StageResult Finalization()
    {
      //отправляем команду на перемещение скребка вверх
      MoveScraperUp(TimeSpan.FromSeconds(StageParameters.OneWayTimeout));

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
      DataPointHelper.SetDiscreteParameter(this, "YA02", true, (int)OperationDelay.TotalMilliseconds);

      //останавливаем управление заслонкой
      ArtMonbatFacility.Damper.StopControl();

      return StageResult.Successful;
    }

    protected override StageResult Main(CancellationToken stop, CancellationToken skip)
    {
      StartTime = DateTime.Now;

      CurrentPass = 0;
      MaxPassCount = StageParameters.PassCount;
      CollingTime = TimeSpan.FromSeconds(StageParameters.CoolingTime);

      while (CurrentPass < MaxPassCount)
      {
        CurrentPass++;

        //проверяем на прерывание стадии пользователем
        if (stop.IsCancellationRequested)
          return StageResult.Stopped;

        //проверяем на пропуск стадии пользователем
        if (skip.IsCancellationRequested)
          return StageResult.Skipped;

        try
        {
          if (!MoveScraperDown(TimeSpan.FromSeconds(StageParameters.OneWayTimeout)))
          {
            //TODO:если застрял при движении вниз
            OnTick($"Скребок не достиг нижнего положения.", MessageType.Error);
            DataPointHelper.WaitDiscreteParameterSet(this, "CS02", true, TimeSpan.FromSeconds(5.0));
          }

          if (!MoveScraperUp(TimeSpan.FromSeconds(StageParameters.OneWayTimeout)))
          {
            //TODO:если застрял при движении вверх
            OnTick($"Скребок не достиг верхнего положения.", MessageType.Error);
            DataPointHelper.WaitDiscreteParameterSet(this, "CS01", true, TimeSpan.FromSeconds(5.0));
          }
        }
        catch (TimeoutException toex)
        {
          MoveScraperUp(TimeSpan.FromSeconds(StageParameters.OneWayTimeout));
          OnTick(toex.Message, MessageType.Error);
          
          return StageResult.Failed;
        }
        catch (Exception ex)
        {
          MoveScraperUp(TimeSpan.FromSeconds(StageParameters.OneWayTimeout));
          OnTick(ex.Message, MessageType.Exception);
          return StageResult.Excepted;
        }

        OnTick($"Завершен проход [{CurrentPass}] скребка. Ожидаем охлаждения штоков", MessageType.Info);

        //ожидаем охлождение штоков
        Thread.Sleep(TimeSpan.FromSeconds(StageParameters.CoolingTime));
      }

      return StageResult.Successful;
    }
  }
}
