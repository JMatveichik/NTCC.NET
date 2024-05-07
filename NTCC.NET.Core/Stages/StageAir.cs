using NTCC.NET.Core.Facility;
using NTCC.NET.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace NTCC.NET.Core.Stages
{
  public class StageAir : StageTimeBased
  {
    public StageAir(string id) : base(id)
    {
    }

    /// <summary>
    ///Интервал между вызовами отжига скребка
    /// </summary>
    public TimeSpan BurningInterval
    {
      get => burningInterval;
      set
      {
        if (burningInterval == value)
          return;

        burningInterval = value;
        OnPropertyChanged();
      }
    }    
    private TimeSpan burningInterval = TimeSpan.FromMinutes(5.0);

    /// <summary>
    /// Время перемещения скребка в горячую зону (зону отжига)
    /// </summary>
    public TimeSpan BurningMoveInterval
    {
      get => burningMoveInterval;
      set
      {
        if (burningMoveInterval == value)
          return;

        burningMoveInterval = value;
        OnPropertyChanged();
      }
    }
    private TimeSpan burningMoveInterval = TimeSpan.FromMilliseconds(250);

    /// <summary>
    /// Использовать отжиг скребка на текущей стадии
    /// </summary>
    public bool UseScrapperBurning
    {
      get => useScrapperBurning;
      set
      {
        if (useScrapperBurning == value)
          return;

        useScrapperBurning = value;
        OnPropertyChanged();
      }
    }
    private bool useScrapperBurning = true;

    /// <summary>
    /// Время последнего отжига скребка
    /// </summary>
    private DateTime lastBurningTime = DateTime.Now;
    
    /// <summary>
    /// Метод вызываемый для стадии с протоком воздуха при ее инициализации
    /// </summary>
    /// <returns> </returns>
    public override StageResult Prepare()
    {

      //задание параметров прогрева
      SetupHeating();

      //если задана проверка уровня воды
      if (StageParameters.CheckWaterLevel)
      {
        CheckWaterLevel(TimeSpan.FromSeconds(20.0));
      }

      //Если предусмотрено использование подогревателя газа
      if (StageParameters.UseGasHeating)
      {
        ArtMonbatFacility.GasHeater.StartControl();
      }

      //открыть клапан подачи воздуха в камеру синтеза
      DataPointHelper.SetDiscreteParameter(this, "YA05.OPN", true, (int)OperationDelay.TotalMilliseconds);

      //открыть клапан подачи азот/воздуха в камеру синтеза
      DataPointHelper.SetDiscreteParameter(this, "YA14.OPN", true, (int)OperationDelay.TotalMilliseconds);

      //задать расход воздуха в камеру синтеза
      DataPointHelper.SetAnalogParameter(this, "MD400C.SETPOINT.WR", StageParameters.FlowRate);

      //ожидаем установление расхода воздуха
      DataPointHelper.WaitAnalogParameterSet(this, "MD400C.MEASSURE", StageParameters.FlowRate, TimeSpan.FromSeconds(5.0));

      return StageResult.Successful;
    }

    /// <summary>
    /// Метод вызываемый для стадии с протоком воздуха при ее завершении
    /// </summary>
    /// <returns></returns>
    protected override StageResult Finalization()
    {
      //закрыть клапан подачи воздуха в камеру синтеза
      DataPointHelper.SetDiscreteParameter(this, "YA05.OPN", false, (int)OperationDelay.TotalMilliseconds);

      //закрыть клапан подачи азот/воздуха в камеру синтеза
      DataPointHelper.SetDiscreteParameter(this, "YA14.OPN", false, (int)OperationDelay.TotalMilliseconds);

      //задать расход воздуха в камеру синтеза
      DataPointHelper.SetAnalogParameter(this, "MD400C.SETPOINT.WR", ZERRO, (int)OperationDelay.TotalMilliseconds);

      //ожидаем установление расхода воздуха
      DataPointHelper.WaitAnalogParameterSet(this, "MD400C.MEASSURE", ZERRO, TimeSpan.FromSeconds(5.0));

      //Если предусмотрено использование подогревателя газа
      if (StageParameters.UseGasHeating)
      {
        ArtMonbatFacility.GasHeater.StopControl();
      }

      return StageResult.Successful;
    }

    /// <summary>
    /// Выполняем действия на каждом тике при выполнении основного алгоритма стадии.
    /// Для стадии использующей проток воздуха проверяем необходимость отжига скребка
    /// </summary>
    protected override void OnMainTick()
    {
      //вызываем отжиг скребка на "нулевой" точке стадии 

      if (UseScrapperBurning == false)
      {
        return;
      }        
      
      //если прошло время для отжига скребка запускаем отжиг скребка
      if (DateTime.Now - lastBurningTime > BurningInterval)
      {
        //отжиг скребка
        ArtMonbatFacility.Scrapper.BurnOut(BurningMoveInterval);
        lastBurningTime = DateTime.Now;
      }
    }
  }
}