using System;
using System.Threading;

namespace Dispergator.Common.Stages
{
    /// <summary>
    /// Класс стадии слива остатков продукта
    /// </summary>

    public class StageDrainToRecycle :StageBase
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        public StageDrainToRecycle(string id) : base(id)
        {
            ContentID = "DRAIN";
        }

        #region Реализация абстрактных функций

        /// <summary>
        /// Подготовка к стадии слива
        /// </summary>
        /// <returns></returns>
        protected override StageResult Prepare()
        {

            throw new NotImplementedException();

            /*
            //Facility.ResetProductVolume();

            //2.5.2.1.	Выдать сообщение о необходимости подключения емкости к линии отходов с подтверждением оператором
            MessageBoxResult res = MessageBoxResult.Yes;
            App.Current.Dispatcher.Invoke(() =>
            {
                res = RadMessageBox.Show(
                            App.Current.MainWindow,
                            App.Localize("msgDrainToRecycleStep1"),
                            App.Localize("StageDrainToRecycle"),
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Warning);
            }
           );

            if (res == MessageBoxResult.Cancel)
                return StageState.Breaked;

            //2.5.2.2.	Выдать сообщение о необходимости перевести ручной кран Valve 10 в закрытое состояние 
            //          и перевести ручной кран Valve 11 в открытое состояние с подтверждением оператором

            App.Current.Dispatcher.Invoke(() =>
            {
                res = RadMessageBox.Show(
                        App.Current.MainWindow,
                            App.Localize("msgDrainToRecycleStep2"),
                            App.Localize("StageDrainToRecycle"),
                            MessageBoxButton.OKCancel,
                            MessageBoxImage.Warning);
            }
           );

            if (res == MessageBoxResult.Cancel)
            {
                res = RadMessageBox.Show(
                            App.Current.MainWindow,
                            App.Localize("msgDrainToRecycleStep3"),
                            App.Localize("StageDrainToRecycle"),
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);

                return StageState.Breaked;
            }


            return StageState.Suсcessful;
            */
        }

        /// <summary>
        /// Основная функция
        /// </summary>
        /// <param name="cancel"></param>
        /// <returns></returns>
        protected override StageResult Main(CancellationToken cancel)
        {
            throw new NotImplementedException();


            //2.5.2.	При сливе в отходы :

            //2.5.2.3.	Выполнить стадию 5 «Слив» рабочей емкости до достижения минимального уровня жидкости в рабочей емкости 
            ///        (Минимальный уровень в рабочей емкости - "Уровень минимальный" A10.4.13 = 0). 
            ///        Сигнал максимального уровня жидкости в приемной емкости (Максимальный уровень в приемной емкости - "Уровень максимальный" A10.4.15 ) игнорировать. 
            ///        При успешном завершении выдать сообщение о опорожнении рабочей емкости и перейти к п.2.5.2.4. 


            /*
            while (Facility.MinimumLevel != false)
            {
                //остановлено оператором
                if (cancel.IsCancellationRequested)
                {
                    onStageTick( App.Localize("msgStopByOperator") );
                    return StageState.Breaked;
                }

                Thread.Sleep(TimeSpan.FromMilliseconds(200));
            }


            
            MessageBoxResult res = MessageBoxResult.OK;
            App.Current.Dispatcher.Invoke(() =>
            {
                res = RadMessageBox.Show(string.Format(App.Localize("msgDrainToRecycleStep4"), Facility.ProductOutputVolume),
                            App.Localize("StageDrainToRecycle"),
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
            });
             
            return StageState.Suсcessful;
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
            MessageBoxResult res = MessageBoxResult.OK;

            //2.5.2.4.	Выдать сообщение о необходимости перевести ручной кран Valve 10 в открытое состояние, 
            ////        а ручной кран Valve 11 в закрытое состояние с подтверждением оператором.
            App.Current.Dispatcher.Invoke(() =>
            {
                res = RadMessageBox.Show(App.Localize("msgDrainToRecycleStep3"),
                            App.Localize("OperationDrainToRecycle"),
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
            }
           );

            return StageState.Suсcessful;
            */
        }
        #endregion

        /*
        protected override void BuildFlippedValues()
        {
            /////////////////////////////////////////////////////////            
            FlippedParameter fv = new FlippedParameter()
            {
                Title = App.Localize("ValueDrainVolume"),
                Description = App.Localize("ValueDrainVolumeInfo"),
                Units = App.Localize("VolumeUnits"),
                ListenPropertyName = "ProductOutputVolume",
                ListenPropertyOwner = Facility,
                MinimalValue = 0.0,
                MaximalValue = 200.0,
                Ranges = {
                    GaugeRangeBuilder.BuildRange(0.0, 200.0, Colors.DarkGray, 0.02)
                }
            };


            FlippedParameters.Add(fv);

            Facility.PropertyChanged += fv.OnValueChanged;

            ActiveParameter = FlippedParameters[0];

        }
        */
    }
}
