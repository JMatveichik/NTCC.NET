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
            OnTick($"Подготовка стадии  {Title} ...", MessageType.Info);

            var dataPoints = ArtMonbatFacility.DataPoints;

            /*YA1.1 (скребок вверх) открыт,  датчик скребка ON
            YA7 (азот в ресивер) открыт
            YA5 (воздух на ф.-к.) закрывается 
            YA14 (подача в реактор) закрывается
            Флоу-контроллер YE1  задаёт расход азота
            YA6 открывается  (подача азота на ф.-контроллер YE1)
            YA14 (подача в реактор) открыт в течение первых 30 сек. стадии для продувки линии подачи воздуха в ре-актор
            YA15 (продувка тары азотом) открывается  после за-крытия YA14
            Остальные клапаны закрыты
            Подогреватель пропана включен, управление по ТЕ21*/

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

            }

            return StageResult.Successful;
        }

        protected override StageResult Finalization()
        {
            OnTick($"Подготовка стадии  {Title} ...", MessageType.Info);

            var dataPoints = ArtMonbatFacility.DataPoints;

            //закрыть клапан подачи азота на расходомер
            DataPointHelper.SetDiscreteParameter(this, "YA06.OPN", false, (int)OperationDelay.TotalMilliseconds);

            //закрыть клапан подачи азот/воздуха в камеру синтеза
            DataPointHelper.SetDiscreteParameter(this, "YA14.OPN", false, (int)OperationDelay.TotalMilliseconds);

            //задать расход азота в камеру синтеза
            DataPointHelper.SetAnalogParameter(this, "MD400C.SETPOINT.WR", 0.0);

            //ожидаем установление расхода азота
            DataPointHelper.WaitAnalogParameterSet(this, "MD400C.MEASSURE", 0.0, TimeSpan.FromSeconds(5.0));

            return StageResult.Successful;
        }

        protected override StageResult Main(CancellationToken cancel)
        {
            throw new NotImplementedException();
        }
    }
}
