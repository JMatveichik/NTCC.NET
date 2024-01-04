using Dispergator.Common.Devices;
using Dispergator.Common.Facility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Dispergator.Common.Stages
{
    /// <summary>
    /// Класс стадии подготовки установки к запуску
    /// </summary>

    public class StageInitialize : StageBase
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        public StageInitialize(string id) : base(id)
        {
            ContentID = "INIT";
        }


        #region РЕАЛИЗАЦИЯ ФУНКЦИЙ ШАБЛОННОГО МЕТОДА


        /// <summary>
        /// Подготовка 
        /// </summary>
        /// <returns></returns>
        protected override StageResult Prepare()
        {
            
                Tests = new ObservableCollection<TestUnitBase>() {

                    
                    #region СОЕДИНЕНИЕ
                        new TestGroup("GroupConnect", false)
                                    {
                                        new TestAction("A101", DevicesSet.Instance["A101"].Connect, true),
                                        new TestAction("A102", DevicesSet.Instance["A102"].Connect, true),
                                        new TestAction("A103", DevicesSet.Instance["A103"].Connect, true),
                                        new TestAction("A104", DevicesSet.Instance["A104"].Connect, true),
                                        new TestAction("A105", DevicesSet.Instance["A105"].Connect, true),
                                        new TestAction("A106", DevicesSet.Instance["A106"].Connect, true),
                                        new TestAction("A107", DevicesSet.Instance["A107"].Connect, true),
                                        new TestAction("I001", DevicesSet.Instance["I001"].Connect, true),
                                        new TestAction("I002", DevicesSet.Instance["I002"].Connect, true),
                                        new TestAction("I003", DevicesSet.Instance["I003"].Connect, true),
                                        new TestAction("I004", DevicesSet.Instance["I004"].Connect, true)    

                                    },
                    #endregion

                    #region ОБЩИЕ
                    new TestGroup("GroupCommon", false)
                            {
                                //Подготовка элементов установки
                                //new TestAction("PreparePumps", true, "PreparePumps"),                                    

                                // Дверь ШУ - "Закрыта" A10.4.14(A10.4.14 = 1). Если сигнал не в норме, то остановить дальнейшую работу установки с выводом сообщения.
                                new TestDiscrete(DGD.Door, false, true) ,                    

                                // Входное сетевое напряжение в норме -"Норма"(A10.2.8 = 1). Если сигнал не в норме, то остановить дальнейшую работу установки с выводом сообщения.
                                new TestDiscrete(DGD.InputPower, false, true),                    

                                // Кнопка Аварийный Стоп -"Нажата"(A10.2.13 = 1) Если есть сигнал, то остановить дальнейшую работу установки с выводом сообщения.
                                new TestDiscrete(DGD.EmergencyButton, false, true),

                                // Внешний сигнал Аварийный Стоп - "Включен"(A10.2.15 = 1) Если есть сигнал, то остановить дальнейшую работу установки с выводом сообщения.
                                new TestDiscrete(DGD.ExternalEmergency,  false, true)
                            },
                        #endregion

                    #region ДАТЧИКИ
                            new TestGroup("GroupSensors", false)
                            {
                                //датчик давления перед фильтром
                                new TestDoubleInRange(DGD.PressureBeforeFilter, 0.0, 1.0, true),

                                //датчик давления после фильтра 
                                new TestDoubleInRange(DGD.PressureAfterFilter, 0.0, 1.0, true),

                                //датчик давления после насоса высокого давления
                                new TestDoubleInRange(DGD.PressureAfterHighPressurePump, 0.0, 25.0, true),
                
                                //датчик давления после кавитатора
                                new TestDoubleInRange(DGD.PressureAfterCavitator, 0.0, 10.0, true),
                    
                                //датчик температуры перед теплообменником
                                new TestDoubleInRange(DGD.TemperatureBeforeHeatExchanger, -50.0, 250.0, true),
                
                                //датчик температуры после теплообменника
                                new TestDoubleInRange(DGD.TemperatureAfterHeatExchanger, -50.0, 250.0, true),
                    
                                //датчик температуры воды охлаждающего контура
                                new TestDoubleInRange(DGD.TemperatureCircuitWater, -50.0, 250.0, true),

                                //датчик кавитации
                                new TestDoubleInRange(DGD.CavitationLevel, 0.0, 10.0, true),
                            },
                    #endregion

                    #region НАСОСЫ
                            new TestGroup("GroupPumps", false)
                            {
                                // Насос высокого давления -"Аварвия"(A10.4.11 = 0) Если сигнал не в норме, то остановить дальнейшую работу установки с выводом сообщения.
                                new TestPump(DGD.PumpHighPressure, false),

                                // Насос предварительный - "Аварвия"(A10.4.10 = 0) Если сигнал не в норме, то остановить дальнейшую работу установки с выводом сообщения.
                                new TestPump(DGD.PumpPreliminary, false),

                                // Насос циркуляционный охлаждающего контура - "Аварвия"(М3, A10.2.9 = 0).Если сигнал не в норме, то остановить дальнейшую работу установки с выводом сообщения.
                                new TestPump(DGD.PumpCircuit, false),

                                // Вентилятор теплообменника  #1 - "Аварвия"  (М4, A10.2.10=0) Если сигнал не в норме, то остановить дальнейшую работу установки с выводом сообщения.
                                new TestFan(DGD.HeatExchangerFan1, false),

                                // Вентилятор теплообменника  #2 - "Аварвия"  (М5, A10.2.11=0) Если сигнал не в норме, то остановить дальнейшую работу установки с выводом сообщения.
                                new TestFan(DGD.HeatExchangerFan2, false)

                            },
#endregion

                    #region КРАНЫ
                            new TestGroup("GroupTaps", false)
                            {
                                //закрыть входной клапан 
                                new TestAction(DGD.ValveInput.ID, DGD.ValveInput.Off, true) ,

                                //закрыть выходной клапан
                                new TestAction(DGD.ValveOutput.ID, DGD.ValveOutput.Off, true),

                                //закрыть циркуляционный кран
                                new TestAction(DGD.TapCircuit.ID, DGD.TapCircuit.Close, true)
                                {
                                    IsAsyncCheck = true
                                },

                                //закрыть кран рабочего контура
                                new TestAction(DGD.TapWork.ID, DGD.TapWork.Close, true)
                                {
                                    IsAsyncCheck = true
                                },

                                //закрыть кран слива готового продукта
                                new TestAction(DGD.TapOutput.ID, DGD.TapOutput.Close, true)
                                {
                                    IsAsyncCheck = true
                                }
                            }
#endregion
                };

                foreach(TestGroup group in Tests)
                {
                    group.Started += TestUnitStarted;
                    group.Passed += TestUnitPassed;
                    group.Failed += TestUnitFailed;

                    foreach (TestUnitBase unit in group)
                    {
                        unit.Started += TestUnitStarted;
                        unit.Passed += TestUnitPassed;
                        unit.Failed += TestUnitFailed;
                    }
                }

            OnPropertyChanged("Tests");
            return StageResult.Successful;
            
        }


        /// <summary>
        /// Основная функция
        /// </summary>
        /// <param name="cancel"></param>
        /// <returns></returns>
        protected override StageResult Main(CancellationToken cancel)
        {
            try
            { 
                foreach (TestUnitBase unit in Tests)
                {
                    CurrentTest = unit;

                    Task<bool> ct = Task.Factory.StartNew<bool>(() => unit.Check());
                    ct.Wait();

                    Thread.Sleep(500);


                    if (ct.Result != true)
                    {
                        if (FailedTests == null)
                            FailedTests = new List<TestUnitBase>();

                        onStageAlarm(unit);
                        FailedTests.Add(unit);
                        continue;
                    }

                    string msg = string.Format("| {0} | {1} | {2}", unit.Title, unit.Description, Localize("Checked"));
                    Debug.WriteLine(msg);

                    //onStageTick(msg);
                }
            }
            catch(Exception ex)
            {
                return StageResult.Excepted;
            }

            return FailedTests == null ? StageResult.Successful : StageResult.Failed;

        }


        /// <summary>
        /// Функция окончания процесса подготовки
        /// </summary>
        /// <returns></returns>
        protected override StageResult Finalization()
        {
            throw new NotImplementedException();
            //return StageState.Suсcessful;
        }


        #endregion


        public List<TestUnitBase> FailedTests
        {
            get;
            private set;
        }

      
        private void TestUnitFailed(object sender, EventArgs ea)
        {
            onStageAlarm((TestUnitBase)sender);            
        }

        private void TestUnitPassed(object sender, EventArgs ea)
        {
            TestUnitBase unit = (TestUnitBase)sender;
            //string msg = string.Format("| {0:20} | {1} ", unit.Title, unit.Result);
            //onStageTick(msg);
            
        }

        private void TestUnitStarted(object sender, EventArgs ea)
        {
            TestUnitBase unit = (TestUnitBase)sender;
            //string msg = string.Format("| {0:20} | {1} ", unit.Title, App.Localize("Started"));
            //onStageTick(msg);
        }

        public ObservableCollection<TestUnitBase> Tests
        {
            get;
            private set;
        }

        public TestUnitBase CurrentTest
        {
            get => currentTest;
            private set
            {
                if (value == currentTest)
                    return;

                currentTest = value;
                OnPropertyChanged();
            }
        }
        private TestUnitBase currentTest = null;

        

    }
}
