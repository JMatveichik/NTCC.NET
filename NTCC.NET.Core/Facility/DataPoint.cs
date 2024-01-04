

using System.Xml.Linq;

namespace NTCC.NET.Core.Facility
{
    public abstract class DataPoint : FacilityElement
    {
        /// <summary>
        /// Базовый класс датчика подключенного к устройству сбора информации
        /// </summary>
        /// <param name="id"></param>
        public DataPoint(string id) : base(id)
        {
            
        }

        public string Group
        {
            get;
            set;
        }
        /// <summary>
        /// Устройcтво к которому подключен датчик
        /// </summary>
        public abstract AcquisitionDeviceBase Device
        {
            get;
            set;
        }
        protected AcquisitionDeviceBase device = null;

        
        /// <summary>
        /// Канал к которому подключен датчик
        /// </summary>
        public int ListenedChannel
        {
            get => listenedChannel;
            set
            {
                if (listenedChannel == value)
                    return;

                listenedChannel = value;
                OnPropertyChanged();
            }
        }
        private int listenedChannel = -1;

    }
}