using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dispergator.Common.Stages
{
    /// <summary>
    /// Класс стадии обработки пасты
    /// </summary>

    public class StageTreatment : StageBase
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        public StageTreatment(string id) : base(id)
        {
            ContentID = "TREATMENT";
            /*
            StageAlarmTests = new TestGroup("Treatment", false, "Alarm")
            {
                //Давление в рабочей магистрали после насоса высокого давления(А10.6.2)  18 МПа  22 МПа
                new FacilityTestDoubleExcess(Facility, "PressureAfterHighPressurePump", 22.0, false, "Alarm"),                

               //Давление в рабочей магистрали после кавитатора(А10.6.3)    6МПа    8 МПа
               new FacilityTestDoubleExcess(Facility, "PressureAfterCavitator", 8.0, false, "Alarm"),
                
               //Температура суспензии до теплообменника TE1(A10.6.4)   55 C    60 C
               new FacilityTestDoubleExcess(Facility, "TemperatureBeforeHeatExchanger", 60.0, false, "Alarm"),

                //Температура суспензии за теплообменником TE2(A10.6.5)   45 C    50 C
                new FacilityTestDoubleExcess(Facility, "TemperatureAfterHeatExchanger", 50.0, false, "Alarm"),                

                //Температура воды охлаждающего контура TE3(A10.6.6) 40 C    50C
                new FacilityTestDoubleExcess(Facility, "TemperatureCircuitWater", 50.0, false, "Alarm"),

                //Давление в рабочей магистрали до фильтра(А10.6.0)  0,6 МПа 0,8 МПа
                new FacilityTestDoubleExcess(Facility, "PressureBeforeFilter", 0.8, false, "Alarm"),
                
                //Давление в рабочей магистрали после фильтра(А10.6.1)   0,6 МПа 0,8 МПа
                new FacilityTestDoubleExcess(Facility, "PressureAfterFilter", 0.8, false, "Alarm"),

                //Разница показаний Давление в рабочей магистрали до фильтра(А10.6.0) - Давление в рабочей магистрали после фильтра(А10.6.1)    0,4 МПа 0,5 МПа
                new FacilityTestDoubleExcess(Facility, "PressureDropAtFilter", 0.5, false, "Alarm")

            };

            StageWarningTests = new TestGroup("Treatment", true, "Warnings")
            {
                //Сигнал  Предупреждение  Авария
               //Давление в рабочей магистрали после насоса высокого давления(А10.6.2)  18 МПа  22 МПа
                new FacilityTestDoubleExcess(Facility, "PressureAfterHighPressurePump", 18.0, true, "Warning"),                

               //Давление в рабочей магистрали после кавитатора(А10.6.3)    6МПа    8 МПа
               new FacilityTestDoubleExcess(Facility, "PressureAfterCavitator", 6.0, true, "Warning"),
                
               //Температура суспензии до теплообменника TE1(A10.6.4)   55 C    60 C
               new FacilityTestDoubleExcess(Facility, "TemperatureBeforeHeatExchanger", 55.0, true, "Warning"),

                //Температура суспензии за теплообменником TE2(A10.6.5)   45 C    50 C
                new FacilityTestDoubleExcess(Facility, "TemperatureAfterHeatExchanger", 45.0, true, "Warning"),                

                //Температура воды охлаждающего контура TE3(A10.6.6) 40 C    50C
                new FacilityTestDoubleExcess(Facility, "TemperatureCircuitWater", 40.0, true, "Warning"),

                //Давление в рабочей магистрали до фильтра(А10.6.0)  0,6 МПа 0,8 МПа
                new FacilityTestDoubleExcess(Facility, "PressureBeforeFilter", 0.6, true, "Warning"),
                
                //Давление в рабочей магистрали после фильтра(А10.6.1)   0,6 МПа 0,8 МПа
                new FacilityTestDoubleExcess(Facility, "PressureAfterFilter", 0.6, true, "Warning"),

                //Разница показаний Давление в рабочей магистрали до фильтра(А10.6.0) - Давление в рабочей магистрали после фильтра(А10.6.1)    0,4 МПа 0,5 МПа
                new FacilityTestDoubleExcess(Facility, "PressureDropAtFilter", 0.4, true, "Warning")
        };

            */
            


        }

        #region Реализация абстрактных функций

        /// <summary>
        /// Подготовка 
        /// </summary>
        /// <returns></returns>
        protected override StageResult Prepare()
        {
            throw new NotImplementedException();

            /*
            //мигаем зеленой лампой
            LampControl.SpeedGreen = BlinkSpeed.Normal;

            
             
            ///запрашиваем у пользователя длительность стадии
            StageDurationDialog durationWnd = null;
            bool? doTreatment = null;
            App.Current.Dispatcher.Invoke(() =>
            {

                durationWnd = new StageDurationDialog
                {
                    Owner = App.Current.MainWindow,
                    Message = App.Localize("msgTreatmentDuration"),
                    Caption = App.Localize("OperationTreatment"),
                    HideAfter = TimeSpan.FromSeconds(10.0),
                    Duration = TimeSpan.FromMinutes(30.0),

                    MinimalDuration = TimeSpan.FromMinutes(10.0),
                    MaximalDuration = TimeSpan.FromMinutes(80.0),
                    StepDuration = TimeSpan.FromMinutes(5.0)
                };

                durationWnd.ShowDialog();

                doTreatment = durationWnd.DialogResult;
            }
            );

            //пользователь выбрал пропуск стадии 
            if (doTreatment == false)
            {
                Facility.LastMessage = App.Localize("msgCommonStageSkip");
                return OperationResult.Skipped;
            }


            Facility.LastMessage = App.Localize("msgCommonSwitchTaps");
            TotalDuration = durationWnd.Duration;


            //4.2.Перевести все краны в состояние для обработки
            //Сигнал Выход(DO)  Состояние
            //Клапан подачи воды -"Открыть"  A10.2.5 0
            Facility.WaterInputValve = false;
            Thread.Sleep(OperationDelay);

            //Клапан слива -"Открыть"    A10.2.6 0
            Facility.OutputValve = false;
            Thread.Sleep(OperationDelay);

            Task<bool>[] tapTasksInit = new Task<bool>[]
            {
                //Кран подачи пасты "Открыть"(YA3)   A10.3.0 0
                //Facility.TapPasteIn.CloseAsync(),
                //Кран заполнения рабочей емкости пастой "Открыть"(YA4) A10.3.1 0
                Facility.TapFill.CloseAsync(),
                //Впускной кран циркуляционного контура "Открыть"(YA5)  A10.3.2 0
                Facility.TapCircuit.CloseAsync(),
                //Кран рабочего контура "Открыть"(YA6)  A10.3.3 1
                Facility.TapWork.OpenAsync(),
                //Кран слива готового продукта "Открыть"(YA7)    A10.3.4 0
                Facility.TapDrain.CloseAsync()
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
            
            //4.3.Включить питание(контактор КМ2) предварительного насоса(Насос предварительный -"Электропитание Включить" A10.2.4 = 1), 
            Facility.PreliminaryPumpPower = true;
            Thread.Sleep(OperationDelay);

            //включить питание(контактор КМ1) насоса высокого давления(Насос высокого давления - "Электропитание Включить" - "Электропитание Включить" A10.2.3 = 1)  
            //и выдержать паузу 2 сек(для выполнения загрузки ПО инвероров)            
            Facility.HighPressurePumpPower = true;
            Thread.Sleep(TimeSpan.FromSeconds(2.0));

            //4.4.Включить циркуляционный насос системы охлаждения M3(Насос циркуляционный охлаждающего контура - "Включить" A10.2.0 = 1)
            Facility.CoolingCircuitPump = true;
            Thread.Sleep(OperationDelay);

            //4.5.Включить вентиляторы М4 и М5 теплообменников(Вентилятор теплообменника  #1 - "Включить" A10.2.1 = 1 , Вентилятор теплообменника  #2 - "Включить" A10.2.2 = 1 ) 
            Facility.HeatExchangerFan1 = true;
            Thread.Sleep(OperationDelay);

            Facility.HeatExchangerFan2 = true;
            Thread.Sleep(OperationDelay);

            //4.6.Включить предварительный насос М2(Насос предварительный - "Включить" A10.1.0 = 1)
            Facility.PreliminaryPump = true;

            //4.7.Задать частоту питания насоса высокого давления М1  5 - 50 Гц( ?????? )(50Гц соответствует 1000 об / мин).
            ValueInputWnd confirm = null;
            bool? doSetFreq = null;
            App.Current.Dispatcher.Invoke(() =>
            {

                confirm = new ValueInputWnd
                {
                    Owner = App.Current.MainWindow,
                    Value = Parameters.DefaultInvertorFrequency,
                    MinimalValue = 5.0,
                    MaximalValue = 50.0,
                    Step = 1.0,
                    Units = App.Localize("unitsFrequency"),
                    Caption = App.Localize("OperationTreatment"),
                    Message = App.Localize("msgHighPressurePumpFrequency")
                };
                confirm.ShowDialog();

                doSetFreq = confirm.DialogResult;
            }
            );

            if (doSetFreq == true)
                Facility.InvertorFrequency = confirm.Value;
            else
                Facility.InvertorFrequency = Parameters.DefaultInvertorFrequency;


            //4.8.Включить насос высокого давления М1(Насос высокого давления - "Включить" A10.1.3 = 1).
            //При отладке режимов обеспечить возможность ручной регулировки оборотов насоса высокого давления(50Гц соответствует 1000 об / мин).
            Facility.HighPressurePump = true;
            

            return StageState.Suсcessful;
            */
        }



        /// <summary>
        /// Основная функция процесса обработки пасты
        /// </summary>
        /// <param name="cancel"></param>
        /// <returns></returns>
        protected override StageResult Main(CancellationToken cancel)
        {
            throw new NotImplementedException();
            /*
            //зажигаем зеленую лампу
            LampControl.SpeedGreen = BlinkSpeed.Full;

            //4.1.Выдать сообщение о начале стадии «Обработка» с указанием необходимого времени обработки 10 - 80 минут с выбором оператором Пропустить или Продолжить.
            //Сообщение активно 5 секунд.При отсутствии действий оператора выполнить выбор Продолжить с указанным в сообщении временем обработки.

            Facility.CurrentOperation = App.Localize("OperationTreatment");
            Facility.LastMessage = App.Localize("msgTreatmentPrepareFor");


            //4.9.В процессе перемешивания выводить оставшееся время обработки, контролировать 
            //и в случае необходимости выдавать предупреждения или останавливать работу установки
            DateTime start = DateTime.Now;


            //4.10.Ожидание истечения указанного в п .4.1 обработки.
            while (true)
            {
                Facility.LastMessage = App.Localize("msgMixingProcess");

                //прошло времени с начала стадии
                TimeSpan dt = DateTime.Now - start;
                LeftTime = TotalDuration - dt;

                //время операции закончено
                if (dt > TotalDuration)
                    return OperationResult.Suсcessful;

                //остановлено оператором
                if (stop.IsCancellationRequested)
                {
                    Facility.LastMessage = App.Localize("msgStopByOperator");
                    IsBusy = false;
                    return OperationResult.Breaked;
                }

                //проверка аварийных онлайн-тестов
                if (!CheckAlarms())
                {
                    //зажигаем красную лампу
                    LampControl.SpeedRed = BlinkSpeed.Full;
                    return OperationResult.Failed;
                }
                else
                {
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


            //4.17.При успешном завершении предыдущих действий выдать в течение 5 сек сообщение об окончании текущей стадии и перейти к следующей стадии 5 «Слив»
            //Facility.LastMessage = App.Localize("msgCommonStageComplete");
            //IsBusy = false;
            //return bAlarm;
            return StageState.Suсcessful;
            */

        }


        /// <summary>
        /// Функция окончания процесса обработки пасты
        /// </summary>
        /// <returns></returns>
        protected override StageResult Finalization()
        {
            throw new NotImplementedException();
            /*
            //4.11.Выключить насосы высокого давления М1(Насос высокого давления - "Включить" A10.1.3 = 0).
            

            Facility.HighPressurePump1 = false;
            Facility.HighPressurePump2 = false;
            Facility.HighPressurePump3 = false;
            Facility.HighPressurePump4 = false;
            Thread.Sleep(OperationDelay);

            //4.12.Выключить предварительный насос М2(Насос предварительный - "Включить" A10.1.0 = 0)
            Facility.PreliminaryPump = false;
            Thread.Sleep(OperationDelay);

            //4.13.Задать частоту питания насоса высокого давления М1  0 Гц( ?????? )(50Гц соответствует 1000 об / мин).
            //Facility.InvertorFrequency = 0;
            Thread.Sleep(OperationDelay);

            //4.14.Выключить циркуляционный насос системы охлаждения M3(Насос циркуляционный охлаждающего контура - "Включить" A10.2.0 = 0)
            Facility.CoolingCircuitPump = false;
            Thread.Sleep(OperationDelay);

            //4.15.Выключить вентиляторы М4 и М5 теплообменников(Вентилятор теплообменника  #1 - "Включить" A10.2.1 = 0 ,
            //  Вентилятор теплообменника  #2 - "Включить" A10.2.2 = 0 ) 
            Facility.HeatExchangerFan1 = false;
            Facility.HeatExchangerFan2 = false;
            Thread.Sleep(OperationDelay);

            //4.16.Перевести все краны в закрытое состояние.
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
                Title = App.Localize("ValuePressureAfterHighPressurePump"),
                Description = App.Localize("ValuePressureAfterHighPressurePumpInfo"),
                Units = App.Localize("PressureUnits"),
                ListenPropertyName = "PressureBeforeFilter",
                ListenPropertyOwner = Facility,
                MinimalValue = 0.0,
                MaximalValue = 26.0,
                Format = "{0:F1}",
                ValueStep = 2.0,
                Ranges = {
                    GaugeRangeBuilder.BuildRange(0.0, 18, Colors.DarkGreen, 0.02) ,
                    GaugeRangeBuilder.BuildRange(18.0, 22.0, Colors.Orange, 0.02),
                    GaugeRangeBuilder.BuildRange(22.0, 26.0, Colors.DarkRed, 0.02)
                }
            });
            
            /////////////////////////////////////////////////////////
            FlippedParameters.Add(new FlippedParameter()
            {
                Title = App.Localize("ValuePressureAfterCavitator"),
                Description = App.Localize("ValuePressureAfterCavitatorInfo"),
                Units = App.Localize("PressureUnits"),
                ListenPropertyName = "PressureAfterCavitator",
                ListenPropertyOwner = Facility,
                MinimalValue = 0.0,
                MaximalValue = 10.0,
                Format = "{0:F1}",
                ValueStep = 1.0,
                Ranges = {
                    GaugeRangeBuilder.BuildRange(0.0, 6.0, Colors.DarkGreen, 0.02) ,
                    GaugeRangeBuilder.BuildRange(6.0, 8.0, Colors.Orange, 0.02),
                    GaugeRangeBuilder.BuildRange(8.0, 10.0, Colors.DarkRed, 0.02)
                }
            });

            /////////////////////////////////////////////////////////
            FlippedParameters.Add(new FlippedParameter()
            {
                Title = App.Localize("ValueTemperatureBeforeHeatExchanger"),
                Description = App.Localize("ValueTemperatureBeforeHeatExchangerInfo"),
                Units = App.Localize("TemperatureUnits"),
                ListenPropertyName = "TemperatureBeforeHeatExchanger",
                ListenPropertyOwner = Facility,
                MinimalValue = 0.0,
                MaximalValue = 80.0,
                Format = "{0:F1}",
                ValueStep = 10.0,
                Ranges = {
                    GaugeRangeBuilder.BuildRange(0.0, 55.0, Colors.DarkGreen, 0.02) ,
                    GaugeRangeBuilder.BuildRange(55.0, 60.0, Colors.Orange, 0.02),
                    GaugeRangeBuilder.BuildRange(60.0, 80.0, Colors.DarkRed, 0.02)
                }
            });

            /////////////////////////////////////////////////////////
            FlippedParameters.Add(new FlippedParameter()
            {
                Title = App.Localize("ValueTemperatureAfterHeatExchanger"),
                Description = App.Localize("ValueTemperatureAfterHeatExchangerInfo"),
                Units = App.Localize("TemperatureUnits"),
                ListenPropertyName = "TemperatureAfterHeatExchanger",
                ListenPropertyOwner = Facility,
                MinimalValue = 0.0,
                MaximalValue = 80.0,
                Format = "{0:F1}",
                ValueStep = 10.0,
                Ranges = {
                    GaugeRangeBuilder.BuildRange(0.0, 45.0, Colors.DarkGreen, 0.02) ,
                    GaugeRangeBuilder.BuildRange(45.0, 50.0, Colors.Orange, 0.02),
                    GaugeRangeBuilder.BuildRange(50.0, 80.0, Colors.DarkRed, 0.02)
                }
            });

            /////////////////////////////////////////////////////////
            FlippedParameters.Add(new FlippedParameter()
            {
                Title = App.Localize("ValueTemperatureCircuitWater"),
                Description = App.Localize("ValueTemperatureCircuitWaterInfo"),
                Units = App.Localize("TemperatureUnits"),
                ListenPropertyName = "TemperatureCircuitWater",
                ListenPropertyOwner = Facility,
                MinimalValue = 0.0,
                MaximalValue = 80.0,
                Format = "{0:F1}",
                ValueStep = 10.0,
                Ranges = {
                    GaugeRangeBuilder.BuildRange(0.0, 40.0, Colors.DarkGreen, 0.02) ,
                    GaugeRangeBuilder.BuildRange(40.0, 50.0, Colors.Orange, 0.02),
                    GaugeRangeBuilder.BuildRange(50.0, 80.0, Colors.DarkRed, 0.02)
                }
            });
            
            
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
                    GaugeRangeBuilder.BuildRange(0.0, 0.4, Colors.DarkGreen, 0.02),
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
