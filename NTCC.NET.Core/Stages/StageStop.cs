using System;
using System.Threading;

namespace Dispergator.Common.Stages
{
    /// <summary>
    /// Класс завершения работы установки
    /// </summary>

    public class StageStop : StageBase
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        public StageStop(string id) : base(id)
        {
              
        }

        #region Реализация абстрактных функций

        /// <summary>
        /// Функция окончания процесса подготовки
        /// </summary>
        /// <returns></returns>
        protected override StageResult Finalization()
        {
            throw new NotImplementedException();
            //return StageState.Suсcessful;
        }

        /// <summary>
        /// Основная функция
        /// </summary>
        /// <param name="cancel"></param>
        /// <returns></returns>
        protected override StageResult Main(CancellationToken cancel)
        {
            throw new NotImplementedException();
            /*
            //7.3.Перевести все насосы, вентиляторы, клапана, краны в выкл. состояние
            Facility.PreparePumps();

            
            //Клапан входной - "Открыть"  A10.2.5 0
            Facility.InputValve = false;
            Thread.Sleep(OperationDelay);

            //Клапан выходной -"Открыть"    A10.2.6 0
            Facility.OutputValve = false;
            Thread.Sleep(OperationDelay);

            Task<bool>[] tapsClose = new Task<bool>[]
            {
                //Впускной кран циркуляционного контура "Открыть"(YA5)  A10.3.2 0
                Facility.CircuitTap.CloseAsync(),
                //Кран рабочего контура "Открыть"(YA6)  A10.3.3 0
                Facility.WorkTap.CloseAsync(),
                //Кран слива готового продукта "Открыть"(YA7)    A10.3.4 0
                Facility.OutputTap.CloseAsync()
            };

            Task.WaitAll(tapsClose);
            //Кран подачи пасты "Открыть"(YA3)   A10.3.0 0
            //Кран заполнения рабочей емкости пастой "Открыть"(YA4) A10.3.1 0
            //Впускной кран циркуляционного контура "Открыть"(YA5)  A10.3.2 0
            //Кран рабочего контура "Открыть"(YA6)  A10.3.3 0
            //Кран слива готового продукта "Открыть"(YA7)    A10.3.4 0

            LampControl.SpeedGreen = BlinkSpeed.None;
            LampControl.SpeedYellow = BlinkSpeed.None;
            LampControl.SpeedRed = BlinkSpeed.None;
            return StageState.Suсcessful;
            */

        }

        /// <summary>
        /// Подготовка 
        /// </summary>
        /// <returns></returns>
        protected override StageResult Prepare()
        {
            throw new NotImplementedException();
            //return StageState.Suсcessful;
        }

        #endregion
       
    }
}
