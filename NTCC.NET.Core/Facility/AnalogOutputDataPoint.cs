using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCC.NET.Core.Facility
{
    public class AnalogOutputDataPoint : AnalogDataPoint
    {
        public AnalogOutputDataPoint(string id) : base(id)
        {
            Group = "Аналоговые выходы";
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
                    Device.AnalogOutputUpdate += AnalogOutputsUpdate;

                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Показания датчика преобразованные в физическую величину
        /// </summary>
        public double ValueToSet
        {
            get => curValueToSet;
            set
            {
                if (curValueToSet == value)
                    return;

                curValueToSet = value;
                OnPropertyChanged();
            }
        }

        private double curValueToSet = 0.0f;

        /// <summary>
        /// Событие обновления состояния аналоговых входов устройства измерения
        /// </summary>
        /// <param name="sender">Устройство отправившее уведомление об обновлении аналоговых входов</param>
        /// <param name="e">Данные аналоговых входов</param>
        private void AnalogOutputsUpdate(object sender, AnalogDataEventArgs e)
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
                Debug.WriteLine($" AnalogOutputsUpdate : { ex.Message } for {ID} at channel {ListenedChannel} data size [{e.Data.Count}]");
            }
        }
      

        public void WriteValue (double val)
        {
            Device.SetAnalogOutput(ListenedChannel, val);
            OnTick($"Задание  параметра {ID}  ({val})", MessageType.Debug);
    }
    }
}
