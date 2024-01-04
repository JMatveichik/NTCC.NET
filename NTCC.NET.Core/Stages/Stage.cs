using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace Dispergator.Common.Facility
{

    /*
    internal delegate void OperationHandler(Stage sender, OperationEventArgs ea);

    internal delegate void TestHandler(Stage sender, TestEventArgs ea);

    

    internal class OperationEventArgs : StringEventArgs
    {

        public OperationEventArgs(string message, OperationResult res) : base(message)
        {
            Result = res;
        }

        public OperationResult Result
        {
            get;
            private set;
        }

    }

    internal class TestEventArgs : EventArgs
    {

        public TestEventArgs(TestUnitBase tu)
        {
            TestUnit = tu;
        }

        public TestUnitBase TestUnit
        {
            get;
            private set;
        }

    }

    */


    internal abstract class Stage : FacilityElement
    {

        /// <summary>
        /// Конструктор
        /// </summary>
        /*static Stage()
        {

            Facility = ((App)App.Current).BanchModel;

            Parameters = ((App)App.Current).ParametersModel;


            #region СТАДИИ 

            /// Полный цикл работы
            FullCycle = new StageFullCycle("StageFullCycleTitle", "StageFullCycleInfo");


            /// Стадия подготовки установки
            Initialize = new StageInitialize("StageInitializeTitle", "StageInitializeInfo");


            /// Стадия слива продукта        
            Drain = new StageDrain("StageDrainTitle", "StageDrainInfo");

            /// Стадия слива продукта  в отходы      
            DrainToRecycle = new StageDrainToRecycle("StageDrainToRecycleTitle", "StageDrainToRecycleInfo");

            /// Стадия слива продукта  во внешнюю емкость      
            DrainToExternalTank = new StageDrainToExternalTank("StageDrainToExternalTankTitle", "StageDrainToExternalTankInfo");

            /// Стадия заливки рабочей емкости
            FillUp = new StageFillUp("StageFillUpTitle", "StageFillUpInfo");

            /// Стадия перемешивания пасты
            Mixing = new StageMixing("StageMixingTitle", "StageMixingInfo");

            /// Стадия обработки пасты
            Treatment = new StageTreatment("StageTreatmentTitle", "StageTreatmentInfo");


            /// Стадия остановки
            Stop = new StageStop("StageStopTitle", "StageStopInfo");

            #endregion

            

//             List<Tap> taps = new List<Tap> { TapPasteIn, TapFill, TapCircuit, TapWork, TapDrain };
//             foreach(Tap t in taps)
//             {
//                 t.Opening += TapOpening;
//                 t.Opened += TapOpened;
//                 t.Closing += TapClosing;
//                 t.Closed += TapClosed;
//             }
            
            #region ОБЩИЕ АВАРИЙНЫЕ ТЕСТЫ



            CommonAlarmTests = new FacilityTestGroup("Common", false, "Alarms")
            {

                // 6.1.Дверь ШУ - "Закрыта" A10.4.14(A10.4.14 = 1). Если сигнал не в норме, то остановить дальнейшую работу установки с выводом сообщения.
                new FacilityTestDiscrete( Facility, "DoorIsOpen", false, false) ,                    

                // 6.2.Входное сетевое напряжение в норме -"Норма"(A10.2.8 = 1). Если сигнал не в норме, то остановить дальнейшую работу установки с выводом сообщения.
                new FacilityTestDiscrete(Facility, "InputMainsVoltage", false, false),                    

                // 6.3.Кнопка Аварийный Стоп -"Нажата"(A10.2.13 = 1) Если есть сигнал, то остановить дальнейшую работу установки с выводом сообщения.
                new FacilityTestDiscrete(Facility, "EmergencyStopButton", false, false),

                // 6.4.Внешний сигнал Аварийный Стоп - "Включен"(A10.2.15 = 1) Если есть сигнал, то остановить дальнейшую работу установки с выводом сообщения.
                new FacilityTestDiscrete(Facility, "OuterEmergencyStop", false, false),
                    
                // 6.5.Оценка исправности аналоговых датчиков. Если один из датчиков вне диапазона, то остановить дальнейшую работу установки с выводом сообщения.
                
                //датчик давления перед фильтром
                new FacilityTestDoubleInRange(Facility, "PressureBeforeFilter", 0.0, 1.0,  false, "Sensor" ),

                //датчик давления после фильтра
                new FacilityTestDoubleInRange(Facility, "PressureAfterFilter", 0.0, 1.0,  false, "Sensor" ),

                //датчик давления после насоса высокого давления
                new FacilityTestDoubleInRange(Facility, "PressureAfterHighPressurePump", 0.0, 25.0,  false, "Sensor" ),
                
                //датчик давления после кавитатора
                new FacilityTestDoubleInRange(Facility, "PressureAfterCavitator", 0.0, 10.0,  false, "Sensor" ),
                    
                //датчик температуры перед теплообменником
                new FacilityTestDoubleInRange(Facility, "TemperatureBeforeHeatExchanger", -50.0, 250.0,  false, "Sensor" ),                    
                
                //датчик температуры после теплообменника
                new FacilityTestDoubleInRange(Facility, "TemperatureAfterHeatExchanger", -50.0, 250.0,  false, "Sensor" ),
                    
                //датчик температуры воды охлаждающего контура
                new FacilityTestDoubleInRange(Facility, "TemperatureCircuitWater", -50.0, 250.0,  false, "Sensor" ),                    

                //датчик кавитации
                new FacilityTestDoubleInRange(Facility, "CavitationLevel", 0.0, 10.0,  false, "Sensor" ),


                // 6.6.Насос высокого давления -"Аварвия"(A10.4.11 = 0) Если сигнал не в норме, то остановить дальнейшую работу установки с выводом сообщения.
                new FacilityTestDiscrete(Facility, "HighPressurePumpAlarm", false, false),

                // 6.7.Насос предварительный - "Аварвия"(A10.4.10 = 0) Если сигнал не в норме, то остановить дальнейшую работу установки с выводом сообщения.
                new FacilityTestDiscrete(Facility, "PreliminaryPumpAlarm", false, false),

                // 6.8.Насос циркуляционный охлаждающего контура - "Аварвия"(М3, A10.2.9 = 0).Если сигнал не в норме, то остановить дальнейшую работу установки с выводом сообщения.
                new FacilityTestDiscrete(Facility, "CoolingCircuitPumpAlarm", false, false),

                // 6.9.Вентилятор теплообменника  #1 - "Аварвия"  (М4, A10.2.10=0) Если сигнал не в норме, то остановить дальнейшую работу установки с выводом сообщения.
                new FacilityTestDiscrete(Facility, "HeatExchangerFan1Alarm", false, false),

                // 6.10.Вентилятор теплообменника  #2 - "Аварвия"  (М5, A10.2.11=0) Если сигнал не в норме, то остановить дальнейшую работу установки с выводом сообщения.
                new FacilityTestDiscrete(Facility, "HeatExchangerFan2Alarm", false, false)                
            };


            // 6.11.При остановке работы установки Включить красный цвет сигнальной колонны(Сигнальная колонна, красный  -"Включить" A10.4.2 = 1 )
            // 6.12.выполняются действия п.7

            #endregion

        }*/

        

        public Stage(string id) : base (id)
        {
            //Parameters.PropertyChanged += OnParametersChanged;
            BuildFlippedValues();
           
        }

        /// <summary>
        /// Заполнение списка параметров для отображения
        /// </summary>
        protected virtual void BuildFlippedValues()
        {

        }

        /// <summary>
        /// Идентификатор отображаемой для стадии страницы
        /// </summary>
        public string ContentID
        {
            get;
            protected set;
        }

        /// <summary>
        ///Стадия владелец (которая вызвала выполнение)
        /// </summary>
        public Stage Owner
        {
            get;
            private set;
        }

       

        #region СТАДИИ

        /// <summary>
        /// Текущая стадия
        /// </summary>
        public static Stage Current { get; private set; }

        #endregion


        #region СОБЫТИЯ и ОбРАБОТЧИКИ
       /*

        
        /// <summary>
        /// Стадия начала этап подготовки к выполнению основного алгоритма
        /// </summary>
        public event EventHandler Preparing;

        protected void onStagePreparing()
        {
            //текущая стадия
            Current = this;

            //флаг занятости
            IsBusy = true;

            //включаем мигание зеленой лампой
            LampControl.SpeedGreen = BlinkSpeed.Normal;

            //сообщение
            string msg = string.Format("{0} ", App.Localize("msgCommonStagePreparing"));
            AddMessage(msg);

            ///Запускаем задачу смены активного параметра            
            //ParametersFlippingCanellation = new CancellationTokenSource();
            //ParametersFlippingTask = Task.Factory.StartNew(() => flipParams(ParametersFlippingCanellation.Token));

            //вызов обработчиков
            Preparing?.Invoke(this, EventArgs.Empty);
        }



        /// <summary>
        /// Стадия прошла этап подготовки к выполнению основного алгоритма
        /// </summary>
        public event OperationHandler Prepared;

        protected void onStagePrepared(OperationResult res)
        {

            string msg;
            switch (res)
            {
                case OperationResult.Suсcessful:
                    msg = string.Format("{0} ", App.Localize("StagePrepared"));
                    AddMessage(msg, BanchMessageType.Successfully);
                    break;

                case OperationResult.Failed:
                case OperationResult.Timeout:
                    msg = string.Format("{0} ", App.Localize("StageFailed"));
                    AddMessage(msg, BanchMessageType.Alarm);
                    break;

                case OperationResult.Breaked:
                case OperationResult.Skipped:
                    msg = string.Format("{0} ", App.Localize("StageBreaked"));
                    AddMessage(msg, BanchMessageType.Warning);
                    break;

                default:
                    msg = string.Format("{0} ", App.Localize("StagePrepared"));
                    break;
            }            
            Prepared?.Invoke(this, new OperationEventArgs(msg, res));
        }


        /// <summary>
        /// Стадия начала выполнение основного алгоритма 
        /// </summary>
        public event EventHandler Started;

        protected void onStageStarted()
        {
            string msg = string.Format("{0} ", App.Localize("StageStarted"));
            AddMessage(msg);

            Started?.Invoke(this, EventArgs.Empty);
        }
               
        /// <summary>
        /// Стадия завершила  выполнение основного алгоритма 
        /// </summary>
        public event OperationHandler Completed;
        protected void onStageCompleted(OperationResult res)
        {
            string msg = string.Format("{0} ", App.Localize("StageCompleted"));
            AddMessage(msg);
            Completed?.Invoke(this, new OperationEventArgs(msg, res));
        }

        /// <summary>
        /// Начато завершение стадии
        /// </summary>
        public event EventHandler Finalizing;

        protected void onStageFinalizing()
        {
            string msg = string.Format("{0} ", App.Localize("StageFinalizing"));
            AddMessage(msg);

            Finalizing?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Сдадия завершена
        /// </summary>
        public event OperationHandler Finalized;

        protected void onStageFinalized(OperationResult res)
        {
            string msg;
            switch (res)
            {
                case OperationResult.Suсcessful:
                    msg = string.Format("{0} ", App.Localize("StageFinalized"));
                    AddMessage(msg, BanchMessageType.Successfully);
                    break;

                case OperationResult.Failed:
                case OperationResult.Timeout:
                    msg = string.Format("{0} ", App.Localize("StageFailed"));
                    AddMessage(msg, BanchMessageType.Alarm);
                    break;                
                
                case OperationResult.Breaked:
                case OperationResult.Skipped:
                    msg = string.Format("{0} ", App.Localize("StageBreaked"));
                    AddMessage(msg, BanchMessageType.Warning);
                    break;                
                    
                default:
                    msg = string.Format("{0} ", App.Localize("StageFinalized"));
                    break;
            }
            
            Finalized?.Invoke(this, new OperationEventArgs(msg, res));

            //снимаем флаг выполнения
            IsBusy = false;

            //сбрасываем текущую стадию на владельца
            Current = Owner;

            //сбрасываем владельца
            Owner = null;

            
        }

        /// <summary>
        /// Возникла аварийная ситуация во время выполнения стадии
        /// </summary>
        public event TestHandler Alarm;

        protected void onStageAlarm(FacilityTestUnitBase testUnit)
        {
            //TODO:добавить логирование события
            string msg = string.Format("{0:20} | {1}", testUnit.Title, testUnit.TroubleShooting);
            AddMessage(msg, BanchMessageType.Alarm);

            Alarm?.Invoke(this, new TestEventArgs(testUnit));                
        }

        /// <summary>
        /// Возникла предаварийная ситуация во время выполнения стадии
        /// </summary>
        public event TestHandler Warning;

        protected void onStageWarning(FacilityTestUnitBase testUnit)
        {
            //TODO:добавить логирование события
            string msg = string.Format("{0} : {1}", testUnit.Title, testUnit.TroubleShooting);
            AddMessage(msg, BanchMessageType.Warning);

            Warning?.Invoke(this, new TestEventArgs(testUnit));
        } 
        
        */

       #endregion


        /// <summary>
        /// Заданная (общая) продолжительность стадии
        /// </summary>
        public TimeSpan TotalDuration
        {
            get { return totalDuration; }
            set
            {
                if (totalDuration == value)
                    return;

                totalDuration = value;
                OnPropertyChanged("TotalDuration");
            }
        }
        private TimeSpan totalDuration;

        /// <summary>
        ///Продолжительность стадии (от времени начала)
        /// </summary>
        public TimeSpan Duration
        {
            get { return duration; }
            set
            {
                if (duration == value)
                    return;

                duration = value;
                OnPropertyChanged("Duration");
            }
        }
        private TimeSpan duration;


        /// <summary>
        ///Оставшееся время стадии
        /// </summary>
        public TimeSpan LeftTime
        {
            get { return leftTime; }
            set
            {
                if (leftTime == value)
                    return;

                leftTime = value;
                OnPropertyChanged("LeftTime");
            }
        }
        private TimeSpan leftTime;



        /// <summary>
        /// Время начала операции
        /// </summary>
        public DateTime StartTime
        {
            get { return startTime; }
            set
            {
                if (startTime == value)
                    return;

                startTime = value;
                OnPropertyChanged("StartTime");
            }
        }
        private DateTime startTime;


        /// <summary>
        ///Общие аврарийные тесты для любой стадии
        /// </summary>
        static public TestGroup CommonAlarmTests
        {
            get;
            private set;

        }


        /// <summary>
        /// Аварийные тесты специфичные для стадии
        /// </summary>
        public TestGroup StageAlarmTests
        {
            get;
            protected set;
        }


        /// <summary>
        /// Предупредительные тесты специфичные для стадии
        /// </summary>
        public TestGroup StageWarningTests
        {
            get;
            protected set;
        }       


        public bool IsBusy
        {
            get
            {
                return isBusy;
            }
            protected set
            {
                if (isBusy == value)
                    return;

                isBusy = value;
                OnPropertyChanged("IsBusy");
            }
        }
        protected bool isBusy = false;

        protected static TimeSpan OperationDelay
        {
            get;
            private set;
        } = TimeSpan.FromMilliseconds(1000);

        #region ПОЛЯ

        protected static CancellationTokenSource stop = null;

        public static Facility Facility
        {
            get;
            private set;
        }

        /*
        public static ProcessParameters Parameters
        {
            get;
            private set;
        }
        */
        #endregion



        #region ФУНКЦИИ

        /// <summary>
        /// Операции при подготовке к стадии
        /// </summary>
        /// <returns></returns>
        //protected abstract OperationResult Prepare();


        /// <summary>
        /// Операции после окончания стадии
        /// </summary>
        /// <returns></returns>
        //protected abstract OperationResult Finalize();

       
        /*
        private void flipParams(CancellationToken cancel)
        {
            //не задан список отображаемызх параметров
            if (FlippedParameters == null)
                return;

            //пустой список отображаемых параметров
            if (FlippedParameters.Count == 0)
                return;

            
            IEnumerator<FlippedParameter> ape = FlippedParameters.GetEnumerator();
            ape.MoveNext();
            ActiveParameter = ape.Current;

            DateTime tms = DateTime.Now;
            while (true)
            {
                if (cancel.IsCancellationRequested)
                    break;

                TimeSpan showTime = DateTime.Now - tms;

                //меняем отображаемый параметр
                if (showTime >= ActiveParameter.FlipTime)
                {
                    if (ape.MoveNext())
                        ActiveParameter = ape.Current;
                    else
                        ape.Reset();
                    
                    tms = DateTime.Now;
                }

                Thread.Sleep(200);
            }

            ActiveParameter = FlippedParameters[0];
        }
        
        private void finalization(OperationResult r)
        {
            //обработчик перед выполнением завершения стадии
            onStageFinalizing();

            //завершение
            Finalize();

            //останавливаем смену активного парамертра
//             ParametersFlippingCanellation.Cancel();
//             ParametersFlippingTask.Wait();

            //обработчик после выполнением завершения стадии
            onStageFinalized(r);
        }


        //private CancellationTokenSource ParametersFlippingCanellation = null;

        //private Task ParametersFlippingTask = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner">Cтадия инициализировавшая выполнение</param>
        /// <returns></returns>
        public OperationResult Do(Stage owner = null)
        {

            //запоминаем владельца стадии
            Owner = owner;

            //инициализация токена прерывания задачи по инициативе пользователя
            stop = new CancellationTokenSource();            

            
            ///вызываем обработчики начала подготовки
            onStagePreparing();

            ///подготовка к выполнению стадии
            OperationResult r = Prepare();

            //проверяем результат выполнения подготовки
            if (r != OperationResult.Suсcessful)
            {
                finalization(r);
                return r;
            }

            //вызываем обработчик завершения подготовки подготовки к выполнению
            onStagePrepared(r);

            ///запускаем основную задачу стадии и ожидаем ее окончания
            Task<OperationResult> main = Task.Factory.StartNew<OperationResult>(() => Main(stop.Token));

            //обрабатываем события начала выполнения основного алгоритма стадии
            onStageStarted();

            main.Wait();

            //обрабатываем события окончания выполнения основного алгоритма стадии
            onStageCompleted(main.Result);

            
            //заканчиваем стадию
            finalization(main.Result);
            
            //возвращаем результат выполнения основной задачи
            return main.Result;
        }





        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract OperationResult Main(CancellationToken cancel);


        /// <summary>
        ///Остановить выполнение стадии
        /// </summary>
        public static void Break()
        {
            if (stop != null)
                stop.Cancel();
        }


        private bool checkAlarmGroup(FacilityTestGroup group)
        {
            foreach (FacilityTestUnitBase ct in group)
            {
                if (!ct.Check())
                {
                    //если тест настроен чтобы останавливаться после провальной проверки
                    //выдаем сообщение
                    if (ct.ContinueWhenFailed == false)
                    {
                        //TODO:СООБЩЕНИЕ И РЕАКЦИЯ В СЛУЧАЕ ЕСЛИ НАСТРОЕНА ОСТАНОВКА ДАЛЬНЕЙШИХ ТЕСТОВ ПРИ ПРОВАЛЬНОЙ ПРОВЕРКИ
                        onStageAlarm(ct);
                        return false;
                    }


                    //проверка на количество провальных проверок
                    //если  первая провальная проверка выдаем предупреждение
                    if (ct.Retries == 1)
                    {
                        //TODO:СООБЩЕНИЕ И РЕАКЦИЯ В СЛУЧАЕ ЕСЛИ ПРОВЕРКА ПРОВАЛЕНА В ПЕРВЫЙ РАЗ
                        onStageWarning(ct);
                        continue;

                    }
                    //если превышает максимально заданный прекращаем тесты выдаем сообщение 
                    if (ct.Retries >= Parameters.MaxTestRetries)
                    {

                        //TODO:СООБЩЕНИЕ И РЕАКЦИЯ В СЛУЧАЕ ЕСЛИ  ПРЕВЫШЕНО ЧИСЛО ПРОВАЛЬНЫХ ПРОВЕРОК
                        onStageAlarm(ct);
                        return false;
                    }

                }
            }

            return true;
        }

        /// <summary>
        /// Выполнить проверку аварийных тестов 
        /// </summary>
        /// <returns></returns>
        protected bool CheckAlarms()
        {
            //проверка общих аварийных тестов
            if (!checkAlarmGroup(CommonAlarmTests))
                return false;

            //проверка аварийных тестов специфичных для стадии
            if (!checkAlarmGroup(StageAlarmTests))
                return false;

            return true;
            
        }

        protected bool CheckWarnings()
        {
            int warnings = 0;
            foreach (FacilityTestUnitBase ct in StageWarningTests)
            {
                if (!ct.Check())
                {
                    
                    //проверка на количество провальных проверок
                    //если  первая провальная проверка продолжаем
                    if (ct.Retries == 1)
                        continue;

                    //если превышает максимально заданный выдаем сообщение 
                    if (ct.Retries >= Parameters.MaxTestRetries)
                    {
                        warnings++;
                        onStageWarning(ct);
                    }
                }
            }

            return warnings == 0;
        }



        #endregion

        #region ФУНКЦИИ ПРОВЕРКИ ПРЕДАВАРИЙНЫХ СОСТОЯНИЙ
        /*
        protected bool CheckWarningPressureAfterHighPressurePump()
        {
            return Facility.PressureAfterHighPressurePump < 18.0;
        }


        protected bool CheckWarningPressureAfterCavitator()
        {
            return Facility.PressureAfterCavitator < 6.0;
        }

        protected bool CheckWarningTemperatureBeforeHeatExchanger()
        {
            return Facility.TemperatureBeforeHeatExchanger < 55.0;
        }

        protected bool CheckWarningTemperatureAfterHeatExchanger()
        {
            return Facility.TemperatureAfterHeatExchanger < 45.0;
        }

        protected bool CheckWarningTemperatureCircuitWater()
        {
            return Facility.TemperatureCircuitWater < 40.0;
        }

        protected bool CheckWarningPressureBeforeFilter()
        {
            return Facility.PressureBeforeFilter < 0.6;
        }
        protected bool CheckWarningPressureAfterFilter()
        {
            return Facility.PressureAfterFilter < 0.6;
        }

        protected bool CheckWarningPressureDropAtFilter()
        {
            double dP = Math.Abs(Facility.PressureBeforeFilter - Facility.PressureAfterFilter);
            return dP < 0.4;
        }
        */
        #endregion


        #region ФУНКЦИИ ПРОВЕРКИ АВАРИЙНЫХ СОСТОЯНИЙ
/*
        protected bool CheckAlarmPressureAfterHighPressurePump()
        {
            return Facility.PressureAfterHighPressurePump < 22.0;
        }


        protected bool CheckAlarmPressureAfterCavitator()
        {
            return Facility.PressureAfterCavitator < 8.0;
        }

        protected bool CheckAlarmTemperatureBeforeHeatExchanger()
        {
            return Facility.TemperatureBeforeHeatExchanger < 60.0;
        }

        protected bool CheckAlarmTemperatureAfterHeatExchanger()
        {
            return Facility.TemperatureAfterHeatExchanger < 50.0;
        }

        protected bool CheckAlarmTemperatureCircuitWater()
        {
            return Facility.TemperatureCircuitWater > 50.0;
        }

        protected bool CheckAlarmPressureBeforeFilter()
        {
            return Facility.PressureBeforeFilter < 0.8;
        }
        protected bool CheckAlarmPressureAfterFilter()
        {
            return Facility.PressureAfterFilter < 0.8;
        }

        protected bool CheckAlarmPressureDropAtFilter()
        {
            double dP = Math.Abs(Facility.PressureBeforeFilter - Facility.PressureAfterFilter);
            return dP < 0.6;
        }
        */
        #endregion
    }
}
