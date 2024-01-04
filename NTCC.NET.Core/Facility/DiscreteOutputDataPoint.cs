using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCC.NET.Core.Facility
{
    public class DiscreteOutputDataPoint : DiscreteDataPoint
    {

        public DiscreteOutputDataPoint(string id) : base (id)
        {
            Group = "Дискретные выходы";
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
                    Device.DiscreteOutputUpdate += DiscreteOutputsUpdate;

                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Обработчик события обновления состояния дискретных входов устройства
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DiscreteOutputsUpdate(object sender, DiscreteDataEventArgs e)
        {
            try
            {
                if (sender == Device)
                {
                    State = e.Data[ListenedChannel];
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($" DiscreteOutputUpdate : { ex.Message } for {ID} at channel {ListenedChannel} data size {e.Data.Count}");
            }
        }

       
        public void SetState(bool state)
        {
            Device.SetDiscreteOutput(ListenedChannel, state);
        }
    }
}
