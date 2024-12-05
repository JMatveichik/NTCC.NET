using NTCC.NET.Core.Facility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCC.NET.Core.Monitors
{

  public enum MonitorStatus
  {
    /// <summary>
    /// Статус монитора - Норма  
    /// </summary>
    Normal,

    /// <summary>
    /// Статус монитора - Ошибка
    /// </summary>
    Alert,

    /// <summary>
    /// Статус монитора - Приостановлен
    /// </summary>
    Suspended
  
  }

  // Определяем делегат и событие с новым классом аргументов
  public delegate void MonitorEventEventHandler(object sender, MonitorEventArgs agrs);

  public class MonitorEventArgs : FacilityMessageArgs
  {

    /// <summary>
    /// Конструктор для аргумента сообщения от monitor установки
    /// </summary>
    /// <param name="messageText">Текст сообщения</param>
    /// <param name="status">Состояние монитора</param>
    public MonitorEventArgs(string messageText, MonitorStatus status) : base(messageText)
    {
      if (MonitorStatus.Normal == status)
      {
        MessageType = MessageType.Info;
      }
      else if (MonitorStatus.Alert == status)
      {
        MessageType = MessageType.Error;
      }
      else if (MonitorStatus.Suspended == status)
      {
        MessageType = MessageType.Warning;
      }
    }

  }

}
