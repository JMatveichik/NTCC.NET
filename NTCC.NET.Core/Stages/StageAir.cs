using NTCC.NET.Core.Facility;
using NTCC.NET.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace NTCC.NET.Core.Stages
{
    public class StageAir : StageBase
    {
        public StageAir(string id) : base(id)
        {
        }

        public override StageResult Prepare()
        {
            OnTick($"Подготовка стадии  {Title} ...", MessageType.Info);

            var dataPoints = ArtMonbatFacility.DataPoints;

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

        protected override StageResult Finalization()
        {
            OnTick($"Завершение стадии  {Title} ...", MessageType.Info);

            //закрыть клапан подачи воздуха в камеру синтеза
            DataPointHelper.SetDiscreteParameter(this, "YA05.OPN", false, (int)OperationDelay.TotalMilliseconds);

            //закрыть клапан подачи азот/воздуха в камеру синтеза
            DataPointHelper.SetDiscreteParameter(this, "YA14.OPN", false, (int)OperationDelay.TotalMilliseconds);

            //задать расход воздуха в камеру синтеза
            DataPointHelper.SetAnalogParameter(this, "MD400C.SETPOINT.WR", 0.0, (int)OperationDelay.TotalMilliseconds);

            //ожидаем установление расхода воздуха
            DataPointHelper.WaitAnalogParameterSet(this, "MD400C.MEASSURE", 0.0, TimeSpan.FromSeconds(5.0));


            return StageResult.Successful;
        }

        protected override StageResult Main(CancellationToken cancel)
        {
            OnTick($"Начата стадия {Title} ...", MessageType.Info);
            
            while (true)
            {
                if (cancel.IsCancellationRequested)
                {

                }

            }
        }
    }
}