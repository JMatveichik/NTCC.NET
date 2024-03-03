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
  class StagePropane : StageTimeBased
  {
   
    public StagePropane(string id) : base(id)
    {

    }

    public override StageResult Prepare()
    {
      //задание параметров прогрева
      SetupHeating();

      //открыть клапан подачи пропан-бутана на расходомер
      DataPointHelper.SetDiscreteParameter(this, "YA13.OPN", true, (int)OperationDelay.TotalMilliseconds);

      //задать расход пропан-бутана в камеру синтеза
      DataPointHelper.SetAnalogParameter(this, "BH.SETPOINT.WR", StageParameters.FlowRate);


      //если задана проверка уровня воды
      //TODO:Перенести проверку и заполнение увлажнителя в отдельный класс 
      if (StageParameters.CheckWaterLevel)
      {
        if (!DataPointHelper.CheckDiscreteParameter("M06.1", true))
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
      //закрыть клапан подачи пропан-бутана на расходомер
      DataPointHelper.SetDiscreteParameter(this, "YA13.OPN", false, (int)OperationDelay.TotalMilliseconds);

      //сбросить расход пропан-бутана в камеру синтеза
      DataPointHelper.SetAnalogParameter(this, "BH.SETPOINT.WR", ZERRO);

      return StageResult.Successful;
    }

    
  }
}
