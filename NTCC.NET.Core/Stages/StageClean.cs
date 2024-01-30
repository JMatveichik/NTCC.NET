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
  class StageClean : StageBase
  {
    public StageClean(string id) : base(id)
    {
    }
        
    private bool WaitScraperPosition(DiscreteDataPoint positionDataPoint, TimeSpan moveTimeOut)
    {
      DateTime moveStart = DateTime.Now;
      TimeSpan moveTime = DateTime.Now - moveStart;

      //ожидаем положения скребка или таймаута
      while (true)
      {
        //обновляем время движения скребка
        moveTime = moveStart - DateTime.Now;

        //если превышено время ожидания положения скребка
        if (moveTime > moveTimeOut)
          return false;

        if (positionDataPoint.State)
          return true;
      }
    }

    private bool MoveScraperTo(bool moveDown, TimeSpan moveTimeOut)
    {
      var dataPoints = ArtMonbatFacility.DataPoints;

      DiscreteOutputDataPoint moveTopDataPoint = dataPoints["YA01.1"] as DiscreteOutputDataPoint;
      DiscreteOutputDataPoint moveDownDataPoint = dataPoints["YA01.2"] as DiscreteOutputDataPoint;

      DiscreteDataPoint isScaperOnTop  = dataPoints["CS01"] as DiscreteDataPoint;
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

        //подача азота в сребок
        nitroToScraper.SetState(true);

        //технологическая задержка
        Thread.Sleep(100);

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

        //включаем линейный модуль скребка #2
        moveDownDataPoint.SetState(false);

        //технологическая задержка
        Thread.Sleep(100);

        //отключаем линейный модуль скребка #1
        moveTopDataPoint.SetState(true);

        //если не достигли верхнего положения ошибка
        if (!WaitScraperPosition(isScaperOnTop, moveTimeOut))
          return false;
      }

      return false;
    }

    public override StageResult Prepare()
    {
      OnTick($"Подготовка стадии  {Title} ...", MessageType.Info);

      //задание параметров прогрева
      SetupHeating();

      //включаем ворошитель
      DataPointHelper.SetDiscreteParameter(this, "M01.RUN", true, (int)OperationDelay.TotalMilliseconds);

      //включаем шнек
      DataPointHelper.SetDiscreteParameter(this, "M02.RUN", true, (int)OperationDelay.TotalMilliseconds);

      //отжимаем уплотнения
      DataPointHelper.SetDiscreteParameter(this, "YA02", false, (int)OperationDelay.TotalMilliseconds);

      return StageResult.Successful;
    }

    protected override StageResult Finalization()
    {
      OnTick($"Завершение стадии  {Title} ...", MessageType.Warning);

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


      return StageResult.Successful;
    }

    protected override StageResult Main(CancellationToken cancel)
    {
      OnTick($"Начато выполнение стадии {Title}.", MessageType.Warning);
      StartTime = DateTime.Now;

      const bool DOWN = false;
      const bool UP   = true;

      int currentPass = 1;

      while (currentPass <= StageParameters.PassCount)
      {
        //проверяем на прерывание стадии пользователем
        if (stop.IsCancellationRequested)
          return StageResult.Breaked;


        if ( !MoveScraperTo(DOWN, TimeSpan.FromSeconds(StageParameters.OneWayTimeout)))
        {
          //TODO:если застрял при движении вниз
          OnTick($"Скребок не достиг нижнего положения {Title}. Ожидаем ручного ДОЛБЛЕНИЯ в течении 5 мин!!!", MessageType.Warning);
          DataPointHelper.WaitDiscreteParameterSet(this, "CS02", true, TimeSpan.FromMinutes(5.0));
        }

        if (!MoveScraperTo(UP, TimeSpan.FromSeconds(StageParameters.OneWayTimeout)))
        {
          //TODO:если застрял при движении вверх
          OnTick($"Скребок не достиг верхнего положения {Title}. Ожидаем ручного ДОЛБЛЕНИЯ  в течении 5 мин!!!", MessageType.Warning);
          DataPointHelper.WaitDiscreteParameterSet(this, "CS01", true, TimeSpan.FromMinutes(5.0));
        }

        OnTick($"Завершен проход [{currentPass}] скребка.", MessageType.Info);
        currentPass++;

        //ожидаем охлождение штоков
        Thread.Sleep(TimeSpan.FromSeconds(StageParameters.CoolingTime));        
      }

      return StageResult.Successful;
    }
  }
}
