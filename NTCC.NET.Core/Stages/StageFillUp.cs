using System;
using System.Threading;

namespace Dispergator.Common.Stages
{
    /// <summary>
    /// Класс стадии заправки рабочей емкости пастой
    /// </summary>

    public class StageFillUp : StageBase
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        public StageFillUp(string id) : base(id)
        {
            ContentID = "FILL";
        }


        
        private double fillVolume = 80.0;

        /// <summary>
        /// Заполняемый объем воды
        /// </summary>
        public double FillVolume
        {
            get => fillVolume;
            set
            {
                if (fillVolume == value)
                    return;

                fillVolume = value;
                OnPropertyChanged();                
            }
            
        }



        #region Реализация абстрактных функций

        /// <summary>
        /// Подготовка к стадии заполнения 
        /// </summary>
        /// <returns></returns>

        protected override StageResult Prepare()
        {
            throw new NotImplementedException();
            /*
            //2.1.	Включить зеленый цвет сигнальной колонны (Сигнальная колонна , зеленый  - "Включить" A10.4.0=1)            
            //2.4.	Проверить наличие минимального уровня жидкости в рабочей емкости (Минимальный уровень в рабочей емкости A10.4.13 = 1). 
            //      При наличии сигнала минимального уровня жидкости перейти к п.2.7
            //2.5.	При превышении жидкости минимального уровня включить желтый цвет сигнальной колонны (Сигнальная колонна , желтый - "Включить" A10.4.1 = 1) 
            ///     выдать сообщение и предложить оператору выбрать дальнейшие действия : слить остаток дисперсии в выходную емкость или в отходы.
            ///     
            //2.5.1.	При сливе в выходную емкость , выполнить стадию 5 «Слив». При успешном завершении выдать сообщение о опорожнении рабочей емкости и перейти к п.2.6  

            if (Facility.MinimumLevel == true)
            {
                onStageTick(App.Localize("msgFillUpTankNotEmpty"));

                if (Stage.Drain.Do(this) != StageState.Suсcessful)
                    return StageState.Failed;
            }

            //2.2.	Выдать сообщение о начале стадии «Заполнение емкости» с заданием необходимого количества заливаемой воды 80-200л (шаг 10л) и с выбором оператором Пропустить или Продолжить
            ValueInputWnd confirm = null;
            bool? doFill = null;
            App.Current.Dispatcher.Invoke(() =>
            {

                confirm = new ValueInputWnd
                {
                    Owner = App.Current.MainWindow,
                    Value = 120.0,
                    MinimalValue = 80.0,
                    MaximalValue = 200.0,
                    Step = 10.0,
                    Units = App.Localize("UnitsVolume"),
                    Caption = Title,
                    Message = App.Localize("msgFillUpConfirm")
                };
                confirm.ShowDialog();

                doFill = confirm.DialogResult;
            }
            );

            FillVolume = confirm.Value;

            //2.3.	Если оператор выбрал Пропустить, то программа переходит к следующей стадии  3 «Перемешивание пасты», а если Продолжить, то далее
            if (doFill == false)
            {
                onStageTick(App.Localize("msgCommonStageSkip"));
                return StageState.Skipped;
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
            /*
            //2.7.	Выдать сообщение о начале заполнении емкости водой с указанием текущего количества залитой воды. 
            MessageBoxResult res = MessageBoxResult.OK;
            App.Current.Dispatcher.Invoke(() =>
            {
                res = RadMessageBox.Show(
                            App.Current.MainWindow,
                            string.Format(App.Localize("msgFillUpStep1"), FillVolume),
                            Title,
                            MessageBoxButton.OKCancel,
                            MessageBoxImage.Warning);
            }
            );

            if (res == MessageBoxResult.Cancel)
                return StageState.Skipped;


            //2.8.	Открыть клапан подачи воды в рабочую емкость YA1 ( Клапан подачи воды - "Открыть"  А10.2.5 = 1)
            //Facility.WaterInputValve = true;
            //Facility.ResetWaterFlow();

            StartTime = DateTime.Now;

            //2.9.	В процессе заполнения проверять 

            while (true)
            {

                //Facility.LastMessage = string.Format(App.Localize("msgFillUpVolume"), Facility.WaterInputVolume);

                //2.9.1.	Максимальный уровень жидкости в рабочей емкости (Максимальный уровень в рабочей емкости - "Уровень максимальный" A10.4.12). 
                ///         При появлении сигнала (A10.4.12=1)  выдать сообщение о переполнении емкости и спросить оператора о остановке работы или продолжении работы. 
                ///         Если оператор выбрал продолжить, то перейти к п.2.10, иначе  остановить работу установки и выдать сообщение об ошибке .
                if (Facility.MaximumLevel == true)
                {
                    onStageTick(App.Localize("msgFillUpMaximumLevelReached"));
                    return StageState.Suсcessful;
                }
                    

                //2.9.2.	Ожидать заданного в п.2.2 значения количества заливаемой воды (Расход заливаемой воды А10.5.2 (1ед = 10л)) . 
                //          Если количество заливаемой воды не достигло заданного значения, то п.2.9.1. Если достигло, то далее
//                 if (Facility.WaterInputVolume >= FillVolume)
//                 {
//                     onStageTick(string.Format( App.Localize("msgFillUpFillVolumeReached"), Facility.WaterInputVolume));
//                     return OperationResult.Suсcessful;
//                 }
                    

                //для тестирования
                //Facility.WaterInputVolume += 0.5;

                //остановлено оператором
                if (cancel.IsCancellationRequested)
                {
                    onStageTick(App.Localize("msgStopByOperator"));
                    return StageState.Breaked;
                }

                //продолжительность стадии 
                Duration = DateTime.Now - StartTime;

                Thread.Sleep(TimeSpan.FromMilliseconds(200));
            }
            */
        }
       

        /// <summary>
        /// Функция завершения стадии заполнения
        /// </summary>
        /// <returns></returns>
        protected override StageResult Finalization()
        {
            throw new NotImplementedException();

            /*
            Facility.InputValve = false;

            //2.10.	Выдать сообщение о наполненности ёмкости до заданного значения (указать литры в сообщении). 
            //      ????Если работа продолжается после появления сигнала максимального уровня жидкости в рабочей емкости (Максимальный уровень в рабочей емкости - "Уровень максимальный" A10.4.12), 
            ///     ??? то количество литров указать равным 200 литрам.

            //2.11.	Выдать сообщение о необходимости добавки пасты в количестве из расчета 10-20 литров пасты на 100 литров воды .
            App.Current.Dispatcher.Invoke(() =>
                {
                    RadMessageBox.Show(App.Localize("msgFillUpAddPaste"), 
                                App.Localize("Warning"), 
                                MessageBoxButton.OK, 
                                MessageBoxImage.Warning);
                }
            );

            //2.12.	Оператор или насос добавляет необходимое количество пасты.

            //2.13.	После добавки пасты продолжить программу по подтверждению оператора.             
            return StageState.Suсcessful;
            */
        }
        #endregion

        /*
        protected override void BuildFlippedValues()
        {
            ///////////////////////////////////////////////
            Binding b = new Binding("FillVolume")
            {
                Source = this,
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };

            /////////////////////////////////////////////////////////            
            FlippedParameter fv = new FlippedParameter()
            {
                Title = App.Localize("ValueFilledVolume"),
                Description = App.Localize("ValueFilledVolumeInfo"),
                Units = App.Localize("ValueFilledVolumeUnits"),
                ListenPropertyName = "WaterInputVolume",
                ListenPropertyOwner = Facility,
                MinimalValue = 0.0,
                MaximalValue = 200.0,
                Ranges = {
                    GaugeRangeBuilder.BuildRange(0.0, b, Colors.DarkGreen, 0.02) ,
                    GaugeRangeBuilder.BuildRange(b, 200.0, Colors.DarkGray, 0.02)                    
                }
            };            


            FlippedParameters.Add(fv);

            Facility.PropertyChanged += fv.OnValueChanged;

            ActiveParameter = FlippedParameters[0];
         
        }
        */
    }


}

