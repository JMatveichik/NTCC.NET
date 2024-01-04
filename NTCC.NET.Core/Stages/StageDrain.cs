using Dispergator.Common.Helpers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dispergator.Common.Stages
{
    /// <summary>
    /// Класс стадии слива остатков продукта
    /// </summary>

    public class StageDrain : StageBase
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        public StageDrain(string id) : base(id)
        {
            ContentID = "DRAIN";
        }

        /// <summary>
        /// Выбор режима слива
        /// </summary>
        public IBooleanSelector DrainToExternalSelector
        {
            get;
            set;
        }

        #region РЕАЛИЗАЦИЯ ФУНКЦИЙ ШАБЛОННОГО МЕТОДА


        /// <summary>
        /// Подготовка к стадии слива
        /// </summary>
        /// <returns></returns>
        protected override StageResult Prepare()
        {
            throw new NotImplementedException();
            //return StageResult.Failed;
        }

        /// <summary>
        /// Основная функция
        /// </summary>
        /// <param name="cancel"></param>
        /// <returns></returns>
        protected override StageResult Main(CancellationToken cancel)
        {
            throw new NotImplementedException();
            //return StageResult.Failed;

            /*
            
            MessageBoxResult res = MessageBoxResult.Yes;

            App.Current.Dispatcher.Invoke(() =>
            {
                res = RadMessageBox.Show(
                            App.Current.MainWindow,
                            App.Localize("msgDrainTo"),
                            App.Localize("StageDrainTitle"),
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Warning);
            }
            );


            if (res == MessageBoxResult.Yes)
            {
                return Stage.DrainToExternalTank.Do(this);
                //return DrainToExternalTank(cancel);
            }

            else
                return Stage.DrainToRecycle.Do(this);
                //return DrainToRecycle(cancel);
           */

        }


        /// <summary>
        /// Функция окончания процесса слива продукта
        /// </summary>
        /// <returns></returns>
        protected override StageResult Finalization()
        {
            throw new NotImplementedException();

            /*
            //5.6.2.1.Выключить предварительный насос М2(Насос предварительный - "Включить" A10.1.0 = 0)
            Facility.PreliminaryPump = false;
            Thread.Sleep(OperationDelay);

            //5.6.2.2.Закрыть клапан слива продукта YA2(Клапан слива - "Открыть" A10.2.6 = 0)
            Facility.OutputValve = false;
            Thread.Sleep(OperationDelay);

            //5.6.2.3.Закрыть кран слива готового продукта YA7(Кран слива готового продукта "Открыть A10.3.4 =0) . Ожидать закрытия крана YA7 (Кран слива готового продукта "Открыт" A10.4.8 = 0 , Кран слива готового продукта "Закрыт" A10.4.9 = 1) . Максимальное время ожидания перехода кранов (YA3-YA7) в заданное состояние – 25-40 сек. Если превышен интервал времени ожидания, то выдать окно сообщений и остановить дальнейшую работу установки.
            onStageTick(App.Localize("msgCommonSwitchTaps"));
            Task<bool> switchTap = Facility.OutputTap.CloseAsync();
            switchTap.Wait();

            if (switchTap.Result == false)
            {
                onStageTick ( App.Localize("msgCommonSwitchTapsFailed") );
                return StageState.Failed;
            }

            //5.7.Выдать сообщение о окончании стадии «Слив» с выводом количества слитого продукта.            
            return StageState.Suсcessful;

            */
        }
        
        #endregion

    }
}
