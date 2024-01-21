using NTCC.NET.Core.Facility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace NTCC.NET.Core.Stages
{
    

    public abstract class StageBase : FacilityElement
    {

        /// <summary>
        /// Конструктор
        /// </summary>  
        public StageBase(string id) : base(id)
        {

        }

       
        /// <summary>
        ///Стадия владелец (которая вызвала выполнение)
        /// </summary>
        public StageBase OwnerStage
        {
            get;
            private set;
        }


        /// <summary>
        /// Текущая стадия (основная выполняющаяся в данный момент)
        /// </summary>
        public static StageBase CurrentStage 
        { 
            get; 
            private set; 
        }

        /// <summary>
        /// Состояние стадии
        /// </summary>
        public StageState State
        {
            get;
            protected set;
        }

        public StageParameters StageParameters
        {
            get;
            set;
        }

        protected void SetupHeating()
        {
            List<ReactorHeatingZone> reactorHeatingZones = ArtMonbatFacility.ReactorZones.Items.Values.ToList();

            //задать параметры нагрева для всех зон
            foreach (var zone in reactorHeatingZones)
            {
                HeatingParameters stageHeatingParams = StageParameters.StageHeatingParameters[zone.ID];
                
                zone.SetupControl(  stageHeatingParams.MaxWallTemperature,
                                    stageHeatingParams.MinWallTemperature,
                                    stageHeatingParams.HeaterPower,
                                    stageHeatingParams.MaxHeaterTemperature);

            }
        }

        #region СОБЫТИЯ и ОбРАБОТЧИКИ
        protected void OnStageStep(StageState stageState)
        {
            //текущая стадия
            CurrentStage = this;

            StageState newState = stageState;
            StageState previousState = State;

            //Выставляем состояние стадии
            State = stageState;
            StageStep?.Invoke(this, new FacilityMessageArgs("Изменение состояния стадии : " , MessageType.Info));
        }

        public event FacilityMessageEventHandler StageStep;

                
      #endregion


        #region ВРЕМЕННЫЕ ХАРАКТЕРИСТИКИ СТАДИИ


        /// <summary>
        /// Заданная (общая) продолжительность стадии
        /// </summary>
        public TimeSpan TotalDuration
        {
            get => totalDuration;
            set
            {
                if (totalDuration == value)
                    return;

                totalDuration = value;
                OnPropertyChanged();
            }
        }
        private TimeSpan totalDuration = TimeSpan.FromSeconds(0);

        /// <summary>
        ///Продолжительность стадии (от времени начала)
        /// </summary>
        public TimeSpan Duration
        {
            get => duration;
            set
            {
                if (duration == value)
                    return;

                duration = value;
                OnPropertyChanged();
            }
        }
        private TimeSpan duration=TimeSpan.FromSeconds(0);


        /// <summary>
        ///Оставшееся время стадии
        /// </summary>
        public TimeSpan LeftTime
        {
            get => leftTime;
            set
            {
                if (leftTime == value)
                    return;

                leftTime = value;
                OnPropertyChanged();
            }
        }
        private TimeSpan leftTime;

        /// <summary>
        /// Время начала стадии
        /// </summary>
        public DateTime StartTime
        {
            get => startTime;
            set
            {
                if (startTime == value)
                    return;

                startTime = value;
                OnPropertyChanged();
            }
        }
        private DateTime startTime;

        /// <summary>
        /// Задержка между выполнение операций стадии (переключение состояния клапанов, насосов, и т.д.)
        /// </summary>
        protected static TimeSpan OperationDelay
        {
            get;
            private set;
        } = TimeSpan.FromMilliseconds(1000);

        #endregion


        #region ТЕСТЫ


        /// <summary>
        ///Общие аврарийные тесты для любой стадии
        /// </summary>
     /* public static TestGroup CommonAlarmTests
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
     */
           
        #endregion
     

        #region ПОЛЯ

        protected static CancellationTokenSource stop = null;
        
        #endregion
       

        /// <summary>
        /// Запуск стадиии на выполнение
        /// </summary>
        /// <param name="owner">Cтадия инициализировавшая выполнение данной</param>
        /// <returns></returns>
        public StageResult Do(StageBase owner = null)
        {
            //запоминаем владельца стадии
            OwnerStage = owner;

            //инициализация токена прерывания стадии по инициативе пользователя
            stop = new CancellationTokenSource();

            try
            {
                //вызываем обработчики начала подготовки
                OnStageStep(StageState.Prepearing);

                StageResult result = StageResult.Successful;

                //подготовка к выполнению стадии            
                result = Prepare();
                if (result != StageResult.Successful)
                    return result;

                //вызываем обработчики завершения подготовки стадии к выполнению
                OnStageStep(StageState.Prepeared);

                //запускаем основной алгоритм стадии
                Task<StageResult> main = Task.Factory.StartNew<StageResult>(() => Main(stop.Token));

                //вызываем обрабатчики события начала выполнения основного алгоритма стадии
                OnStageStep(StageState.Started);

                //ожидаем завершения основного алгоритма стадии
                main.Wait();

                switch (main.Result)
                {
                    case StageResult.Failed:
                        {
                            //вызов обработчиков некорректного завершения стадии
                            OnStageStep(StageState.Failed);
                            return main.Result;
                        }
                    case StageResult.Breaked:
                        {
                            //вызов обработчиков прерывания стадии по инициативе опрератора
                            OnStageStep(StageState.Breaked);
                            return main.Result;
                        }               
                
                }                            

                //вызываем обработчики события окончания выполнения основного алгоритма стадии
                OnStageStep(StageState.Complete);

                //обработчик перед выполнением завершения стадии
                OnStageStep(StageState.Finalizing);

                //выполнение завершение стадии
                result = Finalization();

                //обработчик после выполнением завершения стадии
                OnStageStep(StageState.Finalized);

                return result;

            }
            catch (Exception ex)
            {
                State = StageState.Excepted;
                OnTick($"Непредвиденное завершение стадии : {ex.Message}", MessageType.Exception);
                return StageResult.Excepted;
            }

        }


        #region АБСТРАКТНЫЕ ОПЕРАЦИИ ШАБЛОННОГО МЕТОДА


        /// <summary>
        /// Действия при подготовке к стадии
        /// </summary>
        /// <returns></returns>
        public abstract StageResult Prepare();       


        /// <summary>
        /// Операции после окончания стадии
        /// </summary>
        /// <returns></returns>
        protected abstract StageResult Finalization();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract StageResult Main(CancellationToken cancel);


        /// <summary>
        ///Остановить выполнение стадии
        /// </summary>
        public static void Break()
        {
            if (stop != null)
                stop.Cancel();
        }

        /*
        private bool checkAlarmGroup(TestGroup group)
        {
            foreach (TestUnitBase ct in group)
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
                    //Parameters.MaxTestRetries
                    if (ct.Retries >= 3 )
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
            foreach (TestUnitBase ct in StageWarningTests)
            {
                if (!ct.Check())
                {
                    
                    //проверка на количество провальных проверок
                    //если  первая провальная проверка продолжаем
                    if (ct.Retries == 1)
                        continue;

                    //если превышает максимально заданный выдаем сообщение 
                    //Parameters.MaxTestRetries
                    if (ct.Retries >= 3 )
                    {
                        warnings++;
                        onStageWarning(ct);
                    }
                }
            }

            return warnings == 0;
        }
        */
        #endregion

    }
}
