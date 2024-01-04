using System;
using System.Threading;

namespace Dispergator.Common.Stages
{
    /// <summary>
    /// Класс стадии слива остатков продукта
    /// </summary>

    public class StageDrainToExternalTank : StageBase 
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        public StageDrainToExternalTank(string id) : base(id)
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
            //Facility.ResetProductVolume();
            //return OperationResult.Suсcessful;
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
             * 
            //5.1.Выдать сообщение о начале стадии «Слив». 
            //Указать о необходимости проверить состояние кранов : Valve 10 в открытом состоянии, а Valve 11 в закрытом состоянии.
            //Продолжение по подтверждению оператора.

            //5.2.Проверить отсутствие максимального уровня в приемной емкости(Максимальный уровень в приемной емкости -"Уровень максимальный" A10.4.15 ).

            //5.2.1.Если уровень жидкости в приемной емкости максимальный(Максимальный уровень в приемной емкости -"Уровень максимальный" A10.4.15 = 1 ), 
            //то указать о невозможности слива.Тогда ждать 10 - 15 минут(настроечный параметр) и переход к п. 5.2

            //5.2.2.Если уровень жидкости в приемном баке ниже максимального(Максимальный уровень в приемной емкости - "Уровень максимальный"  A10.4.15 = 0),  
            //то продолжить выполнение программы

            /*
            MessageBoxResult res = MessageBoxResult.Yes;
            while (Facility.OuterTankLevel == true)
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    res = RadMessageBox.Show(
                                App.Current.MainWindow,
                                App.Localize("msgExternalIsFull"),
                                App.Localize("StageDrainToTank"),
                                MessageBoxButton.OKCancel,
                                MessageBoxImage.Warning);
                }
                );

                if (res == MessageBoxResult.Cancel)
                    return OperationResult.Breaked;

            }
            

            //5.5.Открыть кран слива готового продукта YA7(Кран слива готового продукта "Открыть A10.3.4 =1) . 
            //Ожидать открытия крана YA7 (Кран слива готового продукта "Открыт" A10.4.8 = 1 , Кран слива готового продукта "Закрыт" A10.4.9 = 0) . 
            //Максимальное время ожидания перехода кранов (YA3-YA7) в заданное состояние – 25-40 сек. 
            //Если превышен интервал времени ожидания, то выдать окно сообщений и остановить дальнейшую работу установки.


            onStageTick(App.Localize("msgOpenYA7"));

            Task<bool> switchTap = Facility.TapDrain.OpenAsync();
            switchTap.Wait();

            if (switchTap.Result == false)
            {
                //onStageAlarm( new FacilityTestUnit( "TestTapYA7", "TestTapYA7Info", "TestTapTroubleShoot") );
                return OperationResult.Failed;
            }

            //5.3.Открыть клапан слива продукта YA2(Клапан слива -"Открыть" A10.2.6 = 1)
            onStageTick(App.Localize("msgOpenYA2"));
            Facility.OutputValve = true;
            Thread.Sleep(OperationDelay);

            //5.4.Включить предварительный насос М2(Насос предварительный - "Включить" A10.1.0 = 1)
            onStageTick(App.Localize("msgTurnOnM2"));
            Facility.PreliminaryPump = true;
            Thread.Sleep(OperationDelay);

            //5.6.Во время выполнения п.5.3 – п.5.5 выполнять проверку датчиков уровня

            
            while (true)
            {
                //5.6.1.Если уровень жидкости в приемной емкости достиг максимального(Максимальный уровень в приемной емкости - "Уровень максимальный" A10.4.15 = 1) то

                if (Facility.OuterTankLevel == true)
                {
                    onStageTick(App.Localize("msgExternalTankIsFull"));

                    //5.6.1.1.Выключить предварительный насос М2(Насос предварительный - "Включить" A10.1.0 = 0)
                    onStageTick(App.Localize("msgTurnOffM2"));
                    Facility.PreliminaryPump = false;
                    Thread.Sleep(OperationDelay);

                    //5.6.1.2.Закрыть клапан слива продукта YA2(Клапан слива - "Открыть" A10.2.6 = 0)
                    onStageTick(App.Localize("msgCloseYA2"));
                    Facility.OutputValve = false;
                    Thread.Sleep(OperationDelay);

                    //5.6.1.3.Закрыть кран слива готового продукта YA7(Кран слива готового продукта "Открыть A10.3.4 =0) . 
                    //Ожидать закрытия крана YA7 (Кран слива готового продукта "Открыт" A10.4.8 = 0 , Кран слива готового продукта "Закрыт" A10.4.9 = 1) .
                    //Максимальное время ожидания перехода кранов (YA3-YA7) в заданное состояние – 25-40 сек. 
                    //Если превышен интервал времени ожидания, то выдать окно сообщений и остановить дальнейшую работу установки.

                    onStageTick(App.Localize("msgCloseYA7"));
                    switchTap = Facility.TapDrain.CloseAsync();                    

                    switchTap.Wait();

                    if (switchTap.Result == false)
                    {
                        //onStageAlarm(new FacilityTestUnit("TestTapYA7", "TestTapYA7Info", null, "TestTapTroubleShoot"));
                        return OperationResult.Failed;
                    }

                    //5.6.1.4.Переход к п.5.2.1
                    return this.Main(cancel);
                }

                //остановлено оператором
                if (cancel.IsCancellationRequested)
                {
                    Facility.LastMessage = App.Localize("msgStopByOperator");                    
                    return OperationResult.Breaked;
                }
                

                //5.6.2.Если уровень жидкости в рабочей емкости достиг минимального(Минимальный уровень в рабочей емкости - "Уровень минимальный" A10.4.13 = 0) то
                if (Facility.MinimumLevel == false)
                    return OperationResult.Suсcessful;
            }*/
        }

        
        /// <summary>
        /// Функция окончания процесса слива продукта
        /// </summary>
        /// <returns></returns>
        protected override StageResult Finalization()
        {
            throw new NotImplementedException();
        }
        #endregion

        /*
        protected override void BuildFlippedValues()
        {
            /////////////////////////////////////////////////////////            
            FlippedParameter fv = new FlippedParameter()
            {
                Title = App.Localize("ValueDrainiVolume"),
                Description = App.Localize("ValueDrainiVolumeInfo"),
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
