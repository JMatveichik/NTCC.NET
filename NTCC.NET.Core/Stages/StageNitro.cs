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
  class StageNitro : StageBase
  {
    public StageNitro(string id) : base(id)
    {
    }

    public override StageResult Prepare()
    {
      OnTick($"Подготовка стадии  {Title} ...", MessageType.Warning);

      //задание параметров прогрева
      SetupHeating();

      //открыть клапан подачи азота на расходомер
      DataPointHelper.SetDiscreteParameter(this, "YA06.OPN", true, (int)OperationDelay.TotalMilliseconds);

      //открыть клапан подачи азот/воздуха в камеру синтеза
      DataPointHelper.SetDiscreteParameter(this, "YA14.OPN", true, (int)OperationDelay.TotalMilliseconds);

      //задать расход азота в камеру синтеза
      DataPointHelper.SetAnalogParameter(this, "MD400C.SETPOINT.WR", StageParameters.FlowRate);

      //ожидаем установление расхода азота
      DataPointHelper.WaitAnalogParameterSet(this, "MD400C.MEASSURE", StageParameters.FlowRate, TimeSpan.FromSeconds(5.0));

      //если предусмотрена продувка линии пропан бутана
      if (StageParameters.PurgePropaneLine)
      {
        //открыть клапан продувки линии пропан-бутана
        DataPointHelper.SetDiscreteParameter(this, "YA10.OPN", true, (int)OperationDelay.TotalMilliseconds);

        //задать расход азота в камеру синтеза через линию пропан-бутана
        //TODO : хватит ли расхода азота ? 
        DataPointHelper.SetAnalogParameter(this, "BH.SETPOINT.WR", StageParameters.FlowRate);
      }

      return StageResult.Successful;
    }

    protected override StageResult Finalization()
    {
      OnTick($"Завершение стадии  {Title} ...", MessageType.Warning);

      //сбросить расход азота в камеру синтеза
      DataPointHelper.SetAnalogParameter(this, "MD400C.SETPOINT.WR", ZERRO);

      //закрыть клапан подачи азота на расходомер
      DataPointHelper.SetDiscreteParameter(this, "YA06.OPN", false, (int)OperationDelay.TotalMilliseconds);

      //закрыть клапан подачи азот/воздуха в камеру синтеза
      DataPointHelper.SetDiscreteParameter(this, "YA14.OPN", false, (int)OperationDelay.TotalMilliseconds);

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

      return StageResult.Successful;
    }

    protected override StageResult Main(CancellationToken cancel)
    {
      OnTick($"Начата стадия {Title} ...", MessageType.Warning);

      StartTime = DateTime.Now;
      Duration = DateTime.Now - StartTime;

      TotalDuration = TimeSpan.FromMinutes(StageParameters.Duration);

      //ожидаем истечения заданного времени 
      while (Duration < TotalDuration)
      {
        Thread.Sleep((int)OperationDelay.TotalMilliseconds);

        //проверяем на прерывание стадии пользователем
        if (stop.IsCancellationRequested)
          return StageResult.Breaked;

        Duration = DateTime.Now - StartTime;
      }
      return StageResult.Successful;
    }
  }
}
