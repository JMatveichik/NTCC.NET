using System;
using System.Collections;
using System.Collections.Generic;


namespace NTCC.NET.Core.Facility
{
    // Определяем делегат и событие с новым классом аргументов
    public delegate void FacilityMessageEventHandler(object sender, FacilityMessageArgs agrs);

    public class FacilityMessageArgs : EventArgs
    {

        /// <summary>
        /// Тип сообщения
        /// </summary>
        public MessageType MessageType
        {
            get;
            protected set;
        }

        /// <summary>
        /// Текст сообщения
        /// </summary>
        public string Message
        {
            get;
            private set;
        }

        /// <summary>
        /// Время отправления сообщения
        /// </summary>
        public DateTime Time
        {
            get;
            private set;
        }

        /// <summary>
        /// Конструктор для аргумента сообщения от элемента установки
        /// </summary>
        /// <param name="messageText">Текст сообщения</param>
        /// <param name="messageType">Тип сообщения</param>
        public FacilityMessageArgs(string messageText, MessageType messageType = MessageType.Info)
        {
            Message = messageText;
            MessageType = messageType;
            Time = DateTime.Now;
        }
    }

    public class DiscreteDataEventArgs : EventArgs
    {
        public DiscreteDataEventArgs(BitArray data)
        {
            this.Data = new BitArray(data);
        }

        public BitArray Data
        {
            get; private set;
        }
    }

    public class AnalogDataEventArgs : EventArgs
    {
        public AnalogDataEventArgs(IEnumerable<double> data)
        {
            this.Data = new List<double>(data);
        }

        public List<double> Data
        {
            get;
            private set;
        }
    }
}
