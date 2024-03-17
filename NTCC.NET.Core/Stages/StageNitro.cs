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
  class StageNitro : StageTimeBased
  {
    public StageNitro(string id) : base(id)
    {
    }

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

      //открыть клапан подачи азота на расходомер
      DataPointHelper.SetDiscreteParameter(this, "YA06.OPN", true, (int)OperationDelay.TotalMilliseconds);

      //открыть клапан подачи азота/воздуха в камеру синтеза на секунд
      DataPointHelper.SetDiscreteParameter(this, "YA14.OPN", true, (int)OperationDelay.TotalMilliseconds);

      //задать расход азота в камеру синтеза
      DataPointHelper.SetAnalogParameter(this, "MD400C.SETPOINT.WR", StageParameters.FlowRate);

      Thread.Sleep(TimeSpan.FromSeconds(30.0));
      DataPointHelper.SetDiscreteParameter(this, "YA14.OPN", false, (int)OperationDelay.TotalMilliseconds);

      //открыть клапан подачи азота в тару
      DataPointHelper.SetDiscreteParameter(this, "YA15.OPN", true, (int)OperationDelay.TotalMilliseconds);

      //ожидаем установление расхода азота
      DataPointHelper.WaitAnalogParameterSet(this, "MD400C.MEASSURE", StageParameters.FlowRate, TimeSpan.FromSeconds(5.0));

      //если предусмотрена продувка линии пропан бутана
      if (StageParameters.PurgePropaneLine)
      {
        //открыть клапан продувки линии пропан-бутана
        DataPointHelper.SetDiscreteParameter(this, "YA10.OPN", true, (int)OperationDelay.TotalMilliseconds);

        //задать расход азота в камеру синтеза через линию пропан-бутана
        DataPointHelper.SetAnalogParameter(this, "BH.SETPOINT.WR", 5.0);
      }

      return StageResult.Successful;
    }

    protected override StageResult Finalization()
    {
      //сбросить расход азота в камеру синтеза
      DataPointHelper.SetAnalogParameter(this, "MD400C.SETPOINT.WR", ZERRO);

      //закрыть клапан подачи азота на расходомер
      DataPointHelper.SetDiscreteParameter(this, "YA06.OPN", false, (int)OperationDelay.TotalMilliseconds);

      //закрыть клапан подачи азот/воздуха в камеру синтеза
      DataPointHelper.SetDiscreteParameter(this, "YA14.OPN", false, (int)OperationDelay.TotalMilliseconds);

      //закрыть клапан подачи азота в тару
      DataPointHelper.SetDiscreteParameter(this, "YA15.OPN", false, (int)OperationDelay.TotalMilliseconds);

      //ожидаем установление расхода азота
      DataPointHelper.WaitAnalogParameterSet(this, "MD400C.MEASSURE", ZERRO, TimeSpan.FromSeconds(5.0));

      //если предусмотрена продувка линии пропан бутана
      if (StageParameters.PurgePropaneLine)
      {
        //закрыть клапан продувки линии пропан-бутана
        DataPointHelper.SetDiscreteParameter(this, "YA10.OPN", false, (int)OperationDelay.TotalMilliseconds);

        //задать расход азота в камеру синтеза через линию пропан-бутана (0.0)
        DataPointHelper.SetAnalogParameter(this, "BH.SETPOINT.WR", ZERRO);
      }

      //Если предусмотрено использование подогревателя газа
      if (StageParameters.UseGasHeating)
      {
        ArtMonbatFacility.GasHeater.StopControl();
      }

      return StageResult.Successful;
    }

  }
}
