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
    public class StageHeating : StageBase
    {
        public StageHeating(string id) : base(id)
        {

        }

        public override StageResult Prepare()
        {
            //задание параметров прогрева
            SetupHeating();

            //если задан расход для стадии прогрева задаем проток воздуха
            if(StageParameters.FlowRate > 0.0)
            {
                //открыть клапан YA5 подачи воздуха в камеру синтеза
                DataPointHelper.SetDiscreteParameter(this, "YA05.OPN", true, (int)OperationDelay.TotalMilliseconds);

                //открыть клапан YA14 подачи азот/воздуха в камеру синтеза
                DataPointHelper.SetDiscreteParameter(this, "YA14.OPN", true, (int)OperationDelay.TotalMilliseconds);

                //задать расход воздуха в камеру синтеза
                DataPointHelper.SetAnalogParameter(this, "MD400C.SETPOINT.WR", StageParameters.FlowRate, (int)OperationDelay.TotalMilliseconds);

                //ожидаем установление расхода воздуха 5 секунд
                DataPointHelper.WaitAnalogParameterSet(this, "MD400C.MEASSURE", StageParameters.FlowRate, TimeSpan.FromSeconds(5.0));

            }

            return StageResult.Successful;
        }

        protected override StageResult Finalization()
        {
            //если был задан расход для стадии прогрева задаем проток воздуха 0.0
            if (StageParameters.FlowRate > 0.0)
            {
                //задать расход воздуха в камеру синтеза
                DataPointHelper.SetAnalogParameter(this, "MD400C.SETPOINT.WR", 0.0, (int)OperationDelay.TotalMilliseconds);

                //ожидаем установление расхода воздуха 5 секунд
                DataPointHelper.WaitAnalogParameterSet(this, "MD400C.MEASSURE", 0.0, TimeSpan.FromSeconds(5.0));

                //закрыть клапан YA14 подачи азот/воздуха в камеру синтеза
                DataPointHelper.SetDiscreteParameter(this, "YA14.OPN", false, (int)OperationDelay.TotalMilliseconds);

                //звкрыть клапан YA5 подачи воздуха в камеру синтеза
                DataPointHelper.SetDiscreteParameter(this, "YA05.OPN", false, (int)OperationDelay.TotalMilliseconds);

            }

            return StageResult.Successful;
        }

        protected override StageResult Main(CancellationToken cancel)
        {
            OnTick($"Начато выполнение стадии {Title}.", MessageType.Info);
            StartTime = DateTime.Now;

            //ожидаем пока средняя температура стенок реактора превысит заданную в параметрах стадии прогрева
            while (ArtMonbatFacility.AverageWallTemperature < StageParameters.AverageTemperature)
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
