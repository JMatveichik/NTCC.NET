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

    public override StageResult Prepare()
    {
      //задание параметров прогрева
      SetupHeating();
      
      //открыть клапан подачи воздуха в камеру синтеза
      DataPointHelper.SetDiscreteParameter(this, "YA05.OPN", true, (int)OperationDelay.TotalMilliseconds);

      //открыть клапан подачи азот/воздуха в камеру синтеза
      DataPointHelper.SetDiscreteParameter(this, "YA14.OPN", true, (int)OperationDelay.TotalMilliseconds);

      //задать расход воздуха в камеру синтеза
      DataPointHelper.SetAnalogParameter(this, "MD400C.SETPOINT.WR", StageParameters.FlowRate);

      //ожидаем установление расхода воздуха
      DataPointHelper.WaitAnalogParameterSet(this, "MD400C.MEASSURE", StageParameters.FlowRate, TimeSpan.FromSeconds(5.0));

      //если задана проверка уровня воды
      //TODO:Перенести проверку и заполнение увлажнителя в отдельный класс 
      if (StageParameters.CheckWaterLevel)
      {
        if( !DataPointHelper.CheckDiscreteParameter("M06.1", true))
        {
          DataPointHelper.SetDiscreteParameter(this, "YA16.OPN", true, (int)OperationDelay.TotalMilliseconds);
          DataPointHelper.WaitDiscreteParameterSet(this, "M06.1", true, TimeSpan.FromSeconds(10.0));
          DataPointHelper.SetDiscreteParameter(this, "YA16.OPN", false, (int)OperationDelay.TotalMilliseconds);
        }
      }

      //Если предусмотрено использование подогревателя газа
      if (StageParameters.UseGasHeating) 
      {
        ArtMonbatFacility.GasHeater.StartControl();
      }

      return StageResult.Successful;
    }

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

      return StageResult.Successful;
    }
  }
}