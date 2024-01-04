using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;


namespace NTCC.NET.Core.Facility
{
    
    public class FacilityElement : INotifyPropertyChanged
    {
        public FacilityElement(string id)
        {
            ID = id;      
        }

        /// <summary>
        ///Событие элемента установки
        /// </summary>
        public event FacilityMessageEventHandler Tick;


        /// <summary>
        /// Вызов обработчиков
        /// </summary>
        /// <param name="message"></param>
        protected void OnTick(string message, MessageType messageType)
        {
            Tick?.Invoke(this, new FacilityMessageArgs(message, messageType));
        }


        /// <summary>
        /// Идентификатор элемента установки ()
        /// </summary>
        public string ID
        {
            get => id;
            private set
            {
                if (value == id)
                    return;

                id = value;
                OnPropertyChanged();
            }
        }
        private string id = "";


        ///<summary>
        ///Локализованная строка названия элемента установки
        ///</summary>
        public string Title
        {
            get => title;
            set
            {
                if (value == title)
                    return;

                title = value;
                OnPropertyChanged();
            }
        }
        private string title;

        /// <summary>
        /// Локализованная строка описание элемента установки
        /// </summary>        
        public string Description
        {
            get => description;
            set
            {
                if (value == description)
                    return;

                description = value;
                OnPropertyChanged();
            }
        }
        private string description;


        /// <summary>
        /// Локализованная строка   возможных неисправностей элемента установки
        /// </summary>
        public string TroubleShooting
        {
            get => troubleShooting;
            set
            {
                if (troubleShooting == value)
                    return;

                troubleShooting = value;
                OnPropertyChanged();
            }
        }
        private string troubleShooting;

       
        public event PropertyChangedEventHandler PropertyChanged;
        
        /// <summary>
        /// Реализация интерфейса INotifiPropertyChanged
        /// </summary>
        /// <param name="prop"></param>
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
