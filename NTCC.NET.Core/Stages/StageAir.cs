using NTCC.NET.Core.Facility;
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
            SetDiscreteParameter("YA05.OPN", true);

            //открыть клапан подачи азот/воздуха в камеру синтеза
            SetDiscreteParameter("YA14.OPN", true);

            //задать расход воздуха в камеру синтеза
            SetAnalogParameter("MD400C.SETPOINT.WR", StageParameters.FlowRate);

            return StageResult.Successful;
        }

        protected override StageResult Finalization()
        {
            OnTick($"Завершение стадии  {Title} ...", MessageType.Info);

            //закрыть клапан подачи воздуха в камеру синтеза
            SetDiscreteParameter("YA05.OPN", false);

            //закрыть клапан подачи азот/воздуха в камеру синтеза
            SetDiscreteParameter("YA14.OPN", false);

            //задать расход воздуха в камеру синтеза
            SetAnalogParameter("MD400C.SETPOINT.WR", 0.0);


            return StageResult.Successful;
        }

        protected override StageResult Main(CancellationToken cancel)
        {
            while (true)
            {

            }
        }
    }
}