using NTCC.NET.Core.Facility;
using System;
using System.Collections.Generic;
using System.IO;
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

        private Dictionary<DataPoint, string> stageInit
        {
            get;
            set;
        } = new Dictionary<DataPoint, string>();

        private Dictionary<DataPoint, string> stageFinalize
        {
            get;
            set;
        } = new Dictionary<DataPoint, string>();


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

        /*
        /// <summary>
        /// Событие возникающее при начале подготовки стадии к выполнению основного алгоритма
        /// </summary>
        public event EventHandler Preparing;

        /// <summary>
        /// Вызов обработчиков при начале подготовки стадии к выполнению основного алгоритма
        /// </summary>
        

        /// <summary>
        /// Событие возникающее при удачном завершении подготовки стадии к выполнению основного алгоритма
        /// </summary>
        public event EventHandler Prepared;

        /// <summary>
        /// Вызов обработчиков при успешном окончании подготовки стадии к выполнению
        /// </summary>
        protected void onStagePrepared()
        {
            //Выставляем состояние стадии
            State = StageState.Prepearing;

            Prepared?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Событие возникающее при неудачном завершении подготовки стадии к выполнению основного алгоритма
        /// </summary>
        public event EventHandler PrepareFailed;

        /// <summary>
        /// Вызов обработчиков события неудачного завершении подготовки стадии к выполнению основного алгоритма
        /// </summary>
        protected void onStagePrepareFailed()
        {
            //Выставляем состояние стадии
            State = StageState.PrepeareFailed;

            PrepareFailed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Событие возникающее при запуске стадии основного алгоритма на выполение
        /// </summary>
        public event EventHandler Started;
        
        /// <summary>
        /// Вызов  обработчиков события начала выполнения основного алгоритма стадии
        /// </summary>
        protected void onStageStarted()
        {
            //Выставляем состояние стадии
            State = StageState.Started;

            Started?.Invoke(this, EventArgs.Empty);
        }
               
        
        /// <summary>
        /// Событие возникающее при удачном завершении основного алгоритма стадии
        /// </summary>
        public event EventHandler Completed;
        
        /// <summary>
        /// Вызов обработчиков при удачном завершении  основного алгоритма 
        /// </summary>        
        protected void onStageCompleted()
        {
            //Выставляем состояние стадии
            State = StageState.Complete;

            Completed?.Invoke(this, EventArgs.Empty);
        }


        /// <summary>
        ///  Событие возникающее при завершении стадии по ошибке
        /// </summary>
        public event EventHandler Failed;

        protected void onStageFailed()
        {
            //выставляем состояние стадии
            State = StageState.Failed;

            //вызываем обработчики при ошибочном завершении стадии
            Failed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Событие возникающее перед началом завершения стадии.
        /// </summary>
        public event EventHandler Finalizing;

        /// <summary>
        /// Вызов обработчиков перед завершением стадии
        /// </summary>
        protected void onStageFinalizing()
        {
            //Выставляем состояние стадии
            State = StageState.Finalizing;

            Finalizing?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Событие возникающее при заверешии стадии
        /// </summary>
        public event EventHandler Finalized;


        /// <summary>
        /// Вызов обработчиков при завершении стадии
        /// </summary>
        protected void onStageFinalized()
        {
            //Выставляем состояние стадии
            State = StageState.Finalized;

            //сбрасываем текущую стадию на владельца
            Current = Owner;

            //сбрасываем владельца
            Owner = null;

            Finalized?.Invoke(this, EventArgs.Empty);
        }
        */


        /// <summary>
        /// Событие возникающее при появлении аварийной ситуации во время выполнения стадии
        /// </summary>
        //public event TestHandler Alarm;

        /// <summary>
        /// Вызов обработчиков при возникновении аварийной ситуации
        /// </summary>
        /// <param name="testUnit">Тест не прошедший проверку </param>
        /*protected void onStageAlarm(TestUnitBase testUnit)
        {
            Alarm?.Invoke(this, new TestEventArgs(testUnit));                
        }*/

        /// <summary>
        /// Событие позникающее при появлении предаварийной ситуация во время выполнения стадии
        /// </summary>
        //public event TestHandler Warning;

        /// <summary>
        /// Вызов обработчиков появления предаварийной ситуации во  время выполнения стадии
        /// </summary>
        /// <param name="testUnit">Тест не прошедший проверку </param>
        /*protected void onStageWarning(TestUnitBase testUnit)
        {
            Warning?.Invoke(this, new TestEventArgs(testUnit));
        } */
        
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
            //Owner = owner;

            //инициализация токена прерывания стадии по инициативе пользователя
            stop = new CancellationTokenSource();

            try
            {
                //вызываем обработчики начала подготовки
                OnStageStep(StageState.Prepearing);

                StageResult result = StageResult.Successful;

                //подготовка к выполнению стадии            
                //TODO : StageResult result = Prepare();
                //if (result != StageResult.Successful)
                //    return result;

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
                return StageResult.Excepted;
            }

        }

        #region АБСТРАКТНЫЕ ОПЕРАЦИИ ШАБЛОННОГО МЕТОДА

        double parseDoubleAttribute(XElement xmlElem, string strAttribute)
        {
            string strValue = xmlElem.Attribute(strAttribute)?.Value;
            
            if (string.IsNullOrEmpty(strValue))
                throw new IOException($"Не задан  параметр '{strAttribute}' для стадии <{ID}>");


            double doubleValue = 0.0;
            if (!double.TryParse(strValue, out doubleValue))
                throw new IOException($"Ошибка задания параметра {strAttribute} = '{strValue}' для стадии <{ID}>");

            return doubleValue;
        }

        /// <summary>
        /// Действия при подготовке к стадии
        /// </summary>
        /// <returns></returns>
        public  virtual StageResult Prepare(string configDir)
        {
            try
            {
                if (!Directory.Exists(configDir))
                    throw new IOException($"Не найдена директория для конфигурирования установки <{configDir}>");

                string xmlStagesPath = Path.Combine(configDir, "Stages.v2.xml");
                XDocument xmlDocument = XDocument.Load(xmlStagesPath);
                XElement xmlRoot = xmlDocument.Root;

                XElement xmlStage = xmlRoot.XPathSelectElement($"descendant::Stage[@ID='{ID}']");

                if (xmlStage == null)
                {
                    throw new IOException($"Не найдена конфигурация для стадии <{ID}>");
                }
                                

                foreach (var xmlZone in xmlStage.Descendants("Zone"))
                {
                    string zoneID = xmlZone.Attribute("ID")?.Value;

                    var zone = ArtMonbatFacility.ReactorHeaters[zoneID];

                    double minWallTemperature   = parseDoubleAttribute(xmlZone, "MinWallTemperature");
                    double maxWallTemperature   = parseDoubleAttribute(xmlZone, "MaxWallTemperature");
                    double hearterPower         = parseDoubleAttribute(xmlZone, "HearterPower");
                    double maxHeaterTemperature = parseDoubleAttribute(xmlZone, "MaxHeaterTemperature");

                    zone.SetupControl(minWallTemperature, 
                                        maxWallTemperature,
                                        hearterPower,
                                        maxHeaterTemperature);
                }

            }
            catch(Exception ex)
            {
                
                return StageResult.Failed;
            }

            return StageResult.Successful;
        }


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
