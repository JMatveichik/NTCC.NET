using NTCC.NET.Core.Converters;
using System;
using System.Diagnostics;

namespace NTCC.NET.Core.Facility
{
    public class AnalogDataPoint : DataPoint
    {
        /// <summary>
        /// Класс аналового датчика пердаставляющего измеренное значение некоторой ведичины
        /// </summary>
        /// <param name="name"></param>
        public AnalogDataPoint(string name) : base(name)
        {
            Group = "Аналоговые входы";
        }

        /// <summary>
        /// Устройство сбора информации к которому полдключен датчик
        /// </summary>
        public override AcquisitionDeviceBase Device
        {
            get => device;
            set
            {
                if (device == value)
                    return;

                device = value;

                if (Device != null)
                    Device.AnalogInputsUpdate += AnalogInputsUpdate;

                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Показания датчика преобразованные в физическую величину
        /// </summary>
        public double Value
        {
            get => curValue;
            protected set
            {
                if (curValue == value)
                    return;

                curValue = value;
                OnPropertyChanged();
            }
        }

        private double curValue = 0.0f;

        /// <summary>
        /// Показания датчика
        /// </summary>
        public double Signal
        {
            get => curSignal;
            protected set
            {
                if (curSignal == value)
                    return;

                curSignal = value;
                OnPropertyChanged();
            }
        }

        private double curSignal = 0.0f;

        /// <summary>
        /// Минимальное значение
        /// </summary>
        public double MinValue
        {
            get => minValue;
            set
            {
                if (minValue == value)
                    return;

                minValue = value;
                OnPropertyChanged();
            }
        }

        private double minValue = 0.0f;

        /// <summary>
        /// Максимальное значение
        /// </summary>
        public double MaxValue
        {
            get => maxValue;
            set
            {
                if (maxValue == value)
                    return;

                maxValue = value;
                OnPropertyChanged();
            }
        }

        private double maxValue = 100.0f;

        /// <summary>
        /// Единицы измерения
        /// </summary>
        public string Units
        {
            get => uints;
            set
            {
                if (uints == value)
                    return;

                uints = value;

                ValueFormatter = vf => vf + "," + value;
                OnPropertyChanged();
            }
        }

        private string uints = "";


        public Func<double, string> SignalFormatter { get; set; }

        public Func<double, string> ValueFormatter { get; set; }
         

        /// <summary>
        /// Вторичный преобразователь конвертирующий значение полученные от устройства измерения в еденицы пользователя
        /// </summary>
        public BaseConverter Converter
        {
            get => converter;
            set
            {
                if (value == converter)
                    return;

                converter = value;
                OnPropertyChanged();
            }
        }
        private BaseConverter converter = null;

        /// <summary>
        /// Событие обновления состояния аналоговых входов устройства измерения
        /// </summary>
        /// <param name="sender">Устройство отправившее уведомление об обновлении аналоговых входов</param>
        /// <param name="e">Данные аналоговых входов</param>
        private void AnalogInputsUpdate(object sender, AnalogDataEventArgs e)
        {
            try
            {
                if (sender == Device)
                {
                    //устанавливаем сигнал
                    Signal = e.Data[ListenedChannel];

                    //устанавливаем физическую величину
                    Value = (Converter != null) ? Converter.Convert(Signal) : Signal;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($" AnalogInputsUpdate : { ex.Message } for {ID} at channel {ListenedChannel} data size {e.Data.Count}");
            }
        }
    }
}