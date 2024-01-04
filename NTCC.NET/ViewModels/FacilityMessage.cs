using NTCC.NET.Core.Facility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCC.NET.ViewModels
{
    internal class FacilityMessage
    {
        /// <summary>
        /// Конструктор для  сообщения от элемента установки
        /// </summary>
        /// <param name="messageText">Текст сообщения</param>
        /// <param name="messageType">Тип сообщения</param>
        public FacilityMessage(FacilityElement sender, string messageText, MessageType messageType, DateTime createTime)
        {
            SenderID = sender.ID;
            SenderTitle = sender.Title;
            SenderDescription = sender.Description;
            Message = messageText;
            MessageType = messageType;
            Time = createTime;
        }

        public FacilityMessage(FacilityElement sender, FacilityMessageArgs arg) :
            this(sender, arg.Message, arg.MessageType, arg.Time)
        {
        }

        /// <summary>
        /// Отправитель сообщения
        /// </summary>
        public string SenderID
        {
            get;
            private set;
        }

        /// <summary>
        /// Отправитель сообщения
        /// </summary>
        public string SenderTitle
        {
            get;
            private set;
        }

        /// <summary>
        /// Отправитель сообщения
        /// </summary>
        public string SenderDescription
        {
            get;
            private set;
        }

        /// <summary>
        /// Тип сообщения
        /// </summary>
        public MessageType MessageType
        {
            get;
            private set;
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
    }
}
