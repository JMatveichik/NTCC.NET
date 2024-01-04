using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dispergator.Common.Stages
{
    /// <summary>
    /// Класс стадии стадии перемешивания пасты
    /// </summary>

    public class StageMixing : StageBase
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        public StageMixing(string id) : base(id)
        {
            ContentID = "MIXING";

            /*
            ///аварийные тесты
            StageAlarmTests = new TestGroup("Mixing", false, "Alarms")
             {
                new TestDoubleExcess(Facility, "PressureBeforeFilter", 0.8, false, "Alarm"),

                new TestDoubleExcess(Facility, "PressureAfterFilter", 0.8, false, "Alarm"),

                new TestDoubleExcess(Facility, "PressureDropAtFilter", 0.6, false, "Alarm")
            };

            ///предупредительные тесты
            StageWarningTests = new TestGroup("Mixing", true, "Warnings")
            {

                new TestDoubleExcess(Facility, "PressureBeforeFilter", 0.6, true, "Warning"),

                new TestDoubleExcess(Facility, "PressureAfterFilter", 0.6, true, "Warning"),

                new TestDoubleExcess(Facility, "PressureDropAtFilter", 0.4, true, "Warning")
                
            };
            */
        }

        #region Реализация абстрактных функций


        /// <summary>
        /// Подготовка к стадии перемешивания 
        /// </summary>
        /// <returns></returns>
        protected override StageResult Prepare()
        {
            throw new NotImplementedException();
            /*
            // 3.1.Выдать сообщение о начале стадии «Перемешивание пасты» с указанием необходимого времени перемешивания 2 - 5 минут с выбором оператором Пропустить или Продолжить.
            // Сообщение активно 5 секунд.При отсутствии действий оператора выполнить выбор Продолжить с указанным в сообщении временем перемешивания.

            onStageTick(App.Localize("msgWaitOperator"));

            StageDurationDialog mixWnd = null;
            bool? doMix = null;
            App.Current.Dispatcher.Invoke(() =>
            {

                mixWnd = new StageDurationDialog
                {
                    Owner = App.Current.MainWindow,
                    Message = App.Localize("msgMixingDuration"),
                    Caption = App.Localize("OperationMixing"),
                    HideAfter = TimeSpan.FromSeconds(10.0),
                    Duration = TimeSpan.FromMinutes(5.0),
                    MinimalDuration = TimeSpan.FromMinutes(2.0),
                    MaximalDuration = TimeSpan.FromMinutes(5.0),
                    StepDuration = TimeSpan.FromMinutes(0.5)
                };
                mixWnd.ShowDialog();

                doMix = mixWnd.DialogResult;
            });

            //2.3.	Если оператор выбрал Пропустить, то программа переходит к следующей стадии  3 «Перемешивание пасты», а если Продолжить, то далее
            if (doMix == false)
            {
                onStageTick(App.Localize("msgCommonStageSkip"));
                return OperationResult.Skipped;
            }


            TotalDuration = mixWnd.Duration;

            //3.2.Перевести все краны в состояние для перемешивания пасты
            //Сигнал  Выход(DO)  Состояние

            //Клапан подачи воды -"Открыть"  A10.2.5  - 0
            onStageTick(App.Localize("msgCloseYA1"));
            Facility.InputValve = false;
            Thread.Sleep(OperationDelay);

            //Клапан слива -"Открыть"    A10.2.6  - 0
            onStageTick(App.Localize("msgCloseYA2"));
            Facility.OutputValve = false;
            Thread.Sleep(OperationDelay);


            Task<bool>[] tapTasksInit = new Task<bool>[]
            {
                //Впускной кран циркуляционного контура "Открыть"(YA5)  A10.3.2 1
                Facility.CircuitTap.OpenAsync(),
                //Кран рабочего контура "Открыть"(YA6)  A10.3.3 0
                Facility.WorkTap.CloseAsync(),
                //Кран слива готового продукта "Открыть"(YA7)    A10.3.4 0
                Facility.OutputTap.CloseAsync()
            };

            Task.WaitAll(tapTasksInit);
            foreach (Task<bool> t in tapTasksInit)
            {
                if (t.Result == false)
                {
                    Facility.LastMessage = App.Localize("msgCommonSwitchTapsFailed");
                    return OperationResult.Failed;
                }
            }
            return StageState.Suсcessful;
            */

        }


        /// <summary>
        /// Основная функция стадии перемешивания
        /// </summary>
        /// <param name="cancel"></param>
        /// <returns></returns>
        protected override StageResult Main(CancellationToken cancel)
        {
            throw new NotImplementedException();
            /*
            //3.3.Включить питание(контактор КМ2) предварительного насоса М2(Насос предварительный - "Электропитание Включить" A10.2.4 = 1) и выдержать паузу 2 сек
            //3.4.Включить предварительный насос М2(Насос предварительный - "Включить" A10.1.0 = 1)
            Facility.PreliminaryPump = true;
            Thread.Sleep(OperationDelay);

            //3.5.В процессе перемешивания выводить оставшееся время перемешивания, контролировать и в случае необходимости выдавать предупреждения или останавливать работу установки
            //Сигнал Предупреждение  Авария
            //Давление в рабочей магистрали до фильтра(А10.6.0)  0,6 МПа 0,8 МПа
            //Давление в рабочей магистрали после фильтра(А10.6.1)   0,6 МПа 0,8 МПа
            //Разница показаний Давление в рабочей магистрали до фильтра(А10.6.0) - Давление в рабочей магистрали после фильтра(А10.6.1)    0,4 МПа 0,5 МПа

            DateTime start = DateTime.Now;

            while (true)
            {
                //прошло времени с начала стадии
                TimeSpan dt = DateTime.Now - start;
                LeftTime = TotalDuration - dt;

                //закончено время операции
                if (dt > TotalDuration)
                    return OperationResult.Suсcessful;

                //остановлено оператором
                if (cancel.IsCancellationRequested)
                {
                    Facility.LastMessage = App.Localize("msgStopByOperator");
                    return OperationResult.Breaked;
                }


                //проверка аварийных состояний
                if (!CheckAlarms())
                {
                    //зажигаем красную лампу
                    LampControl.SpeedRed = BlinkSpeed.Full;
                    return OperationResult.Failed;
                }
                else
                {
                    //гасим красную лампу
                    LampControl.SpeedRed = BlinkSpeed.None;
                }

                //проверка предаварийных онлайн-тестов
                if (!CheckWarnings())
                {
                    LampControl.SpeedYellow = BlinkSpeed.Normal;
                }
                else
                {
                    LampControl.SpeedYellow = BlinkSpeed.None;
                }

                Thread.Sleep(TimeSpan.FromMilliseconds(200));
            }
            
            return StageState.Suсcessful;
            */
        }

        /// <summary>
        /// Функция окончания стадии перемешивания
        /// </summary>
        /// <returns></returns>
        protected override StageResult Finalization()
        {
            throw new NotImplementedException();
            /*
            //3.6.По истечению заданного в п.3.1 времени выключить предварительный насос(Насос предварительный -"Включить" A10.1.0 = 0)
            Facility.PreliminaryPump = false;

            //3.7.Перевести все краны в закрытое состояние.
            Task<bool>[] tapTasksFinal = new Task<bool>[]
            {
                //Впускной кран циркуляционного контура "Открыть"(YA5)  A10.3.2 0
                Facility.CircuitTap.CloseAsync(),
                //Кран рабочего контура "Открыть"(YA6)  A10.3.3 0
                Facility.WorkTap.CloseAsync(),
                //Кран слива готового продукта "Открыть"(YA7)    A10.3.4 0
                Facility.OutputTap.CloseAsync()
            };

            Task.WaitAll(tapTasksFinal);
            foreach (Task<bool> t in tapTasksFinal)
            {
                if (t.Result == false)
                {
                    Facility.LastMessage = App.Localize("msgCommonSwitchTapsFailed");
                    IsBusy = false;
                    return OperationResult.Failed;
                }
            }
            return StageState.Suсcessful;
            */
            
        }

        #endregion


        /*
        protected override void BuildFlippedValues()
        {
            
            FlippedParameters.Clear();

            /////////////////////////////////////////////////////////
            FlippedParameters.Add(new FlippedParameter()
            {
                Title = App.Localize("ValuePressureBeforeFilter"),
                Description = App.Localize("ValuePressureBeforeFilterInfo"),
                Units = App.Localize("PressureUnits"),
                ListenPropertyName = "PressureBeforeFilter",
                ListenPropertyOwner = Facility,
                MinimalValue = 0.0,
                MaximalValue = 1.0,
                Format = "{0:F1}",
                ValueStep = 0.1,
                Ranges = {
                    GaugeRangeBuilder.BuildRange(0.0, 0.6, Colors.DarkGreen, 0.02) ,
                    GaugeRangeBuilder.BuildRange(0.6, 0.8, Colors.Orange, 0.02),
                    GaugeRangeBuilder.BuildRange(0.8, 1.0, Colors.DarkRed, 0.02)
                }
            });



            /////////////////////////////////////////////////////////
            FlippedParameters.Add(new FlippedParameter()
            {
                Title = App.Localize("ValuePressureAfterFilter"),
                Description = App.Localize("ValuePressureAfterFilterInfo"),
                Units = App.Localize("PressureUnits"),
                ListenPropertyName = "PressureAfterFilter",
                ListenPropertyOwner = Facility,
                MinimalValue = 0.0,
                MaximalValue = 1.0,
                ValueStep = 0.1,
                Format = "{0:F1}",
                Ranges = {
                    GaugeRangeBuilder.BuildRange(0.0, 0.6, Colors.DarkGreen, 0.02) ,
                    GaugeRangeBuilder.BuildRange(0.6, 0.8, Colors.Orange, 0.02),
                    GaugeRangeBuilder.BuildRange(0.8, 1.0, Colors.DarkRed, 0.02)
                }
            });

            /////////////////////////////////////////////////////////
            FlippedParameters.Add(new FlippedParameter()
            {
                Title = App.Localize("ValuePressureDropAtFilter"),
                Description = App.Localize("ValuePressureDropAtFilterInfo"),
                Units = App.Localize("PressureUnits"),
                ListenPropertyName = "PressureDropAtFilter",
                ListenPropertyOwner = Facility,
                MinimalValue = 0.0,
                MaximalValue = 1.0,
                ValueStep = 0.1,
                Format = "{0:F1}",
                Ranges = {
                    GaugeRangeBuilder.BuildRange(0.0, 0.4, Colors.DarkGreen, 0.02) ,
                    GaugeRangeBuilder.BuildRange(0.4, 0.5, Colors.Orange, 0.02),
                    GaugeRangeBuilder.BuildRange(0.5, 1.0, Colors.DarkRed, 0.02)
                }
            });



            foreach (FlippedParameter fp in FlippedParameters)
                Facility.PropertyChanged += fp.OnValueChanged;


            ActiveParameter = FlippedParameters[0];
            
        }
        */

    }
}
