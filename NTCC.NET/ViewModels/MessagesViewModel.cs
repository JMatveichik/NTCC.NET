using NTCC.NET.Core.Facility;
using NTCC.NET.Core.Stages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace NTCC.NET.ViewModels
{

  /// <summary>
  /// Класс для фильтрации сообщений по типу
  /// </summary>
  internal class MessageTypeVisibility : ViewModelBase
  {
    public MessageType Type      
    { 
      get => _type; 
      set
      {
        _type = value;
        OnPropertyChanged();
      } 
    }
    private MessageType _type;

    public bool        IsVisible 
    { 
      get => _isVisible; 
      set
      {
        _isVisible = value;
        OnPropertyChanged();
      } 
    }
    bool _isVisible;
  }
  internal class MessageTypeVisibilityObservableCollection : ObservableCollection<MessageTypeVisibility>
  {
      protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
      {
        if (e.OldItems != null)
        {
          foreach (MessageTypeVisibility item in e.OldItems)
          {
            // Отписка от уведомлений об изменении объектов, которые были удалены
            item.PropertyChanged -= ItemPropertyChanged;
          }
        }

        if (e.NewItems != null)
        {
          foreach (MessageTypeVisibility item in e.NewItems)
          {
            // Подписка на уведомления об изменении объектов, которые были добавлены
            item.PropertyChanged += ItemPropertyChanged;
          }
        }

        base.OnCollectionChanged(e);
      }

      public event PropertyChangedEventHandler CollectionItemChanged = delegate { };
      
      private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
      {
        // Обработка нотификации об изменении объекта в коллекции
        CollectionItemChanged?.Invoke(sender, e);
      }
   }

  internal class MessagesViewModel : PageViewModel
  {

    private readonly object Sync = new object();

    public MessagesViewModel()
    {

      RegisterAllMessageSources();

      // Подписываемся на событие изменения коллекции для обновления фильтра
      ShowMessgeType.CollectionItemChanged += MessgeTypeVisibilityChanged;

      // Подписываемся на событие изменения коллекции для обновления важных сообщений
      MessagesList.CollectionChanged += MessageListChanged;
    }

    /// <summary>
    /// Регистрация обработчика информационных сообщений для элементов установки
    /// </summary>
    /// <param name="facilityElement">Элемент установки</param>
    public void RegisterMessageSource(FacilityElement facilityElement, bool isRegister)
    {
      if (isRegister)
        facilityElement.Tick += OnFacilityElementMessage;
      else
        facilityElement.Tick -= OnFacilityElementMessage;
    }


    private void RegisterAllMessageSources()
    {
      //Подписка на сообщения от устройств
      RegisterDeviceMessages(true);

      //Подписка на сообщения от зон нагрева
      RegisterHeatingZonesMessages(true);

      //Подписка на сообщения от стадий
      RegisterStageMessages(true);

      //Подписка на сообщения от элементов установки
      RegisterFacilityElementsMessages(true);
    }

    public void UnregisterAllMessageSources()
    {
      //Подписка на сообщения от устройств
      RegisterDeviceMessages(false);

      //Подписка на сообщения от зон нагрева
      RegisterHeatingZonesMessages(false);

      //Подписка на сообщения от стадий
      RegisterStageMessages(false);

      //Подписка на сообщения от элементов установки
      RegisterFacilityElementsMessages(false);
    }

    /// <summary>
    /// Регистрация обработчика информационных сообщений для устройств
    /// </summary>
    private void RegisterHeatingZonesMessages(bool isRegister)
    {
      foreach (ReactorHeatingZone zone in ArtMonbatFacility.ReactorZones.Items.Values)
      {
        RegisterMessageSource(zone, isRegister);
      }
    }

    /// <summary>
    /// Регистрация обработчика информационных сообщений для устройств
    /// </summary>
    private void RegisterDeviceMessages(bool isRegister)
    {
      foreach (AcquisitionDeviceBase device in ArtMonbatFacility.Devices.Items.Values)
      {
        RegisterMessageSource(device, isRegister);
      }
    }

    /// <summary>
    /// Регистрация обработчика информационных сообщений для стадий
    /// </summary>
    private void RegisterStageMessages(bool isRegister)
    {
      //регистрируем обработчик логирования для основной стадий
      RegisterMessageSource(ArtMonbatFacility.FullCycle, isRegister);

      //регистрируем обработчик логирования для стадий
      foreach (StageBase stage in ArtMonbatFacility.FullCycle.Stages)
      {
        RegisterMessageSource(stage, isRegister);
      }
    }

    /// <summary>
    /// Регистрация обработчика информационных сообщений для элементов установки
    /// </summary>
    private void RegisterFacilityElementsMessages(bool isRegister)
    {
      //регистрируем обработчик логирования для подогревателя газа
      RegisterMessageSource(ArtMonbatFacility.GasHeater, isRegister);

      //регистрируем обработчик логирования для управления заслонкой
      RegisterMessageSource(ArtMonbatFacility.Damper, isRegister);

      //регистрируем обработчик логирования для управления скребком
      RegisterMessageSource(ArtMonbatFacility.Scrapper, isRegister);

    }


    /// <summary>
    /// Изменен список сообщений
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MessageListChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (e.NewItems != null)
      {
        foreach (FacilityMessage item in e.NewItems)
        {
          if (item.MessageType == MessageType.Warning ||
              item.MessageType == MessageType.Error ||
              item.MessageType == MessageType.Exception)
          {
            ImportantMessagesList.Insert(0, item);
          }
        }
      }

      BadgeValue = ImportantMessagesList.Count;
      ApplyFilter();
    }

    /// <summary>
    /// Пометить сообщение как прочитанное
    /// </summary>
    /// <param name="message"></param>
    public void MarkMessageAsReaded(FacilityMessage message)
    {
      if (message == null)
      {
        ImportantMessagesList.Clear();
      }
      else if (ImportantMessagesList.Contains(message))
      {
        ImportantMessagesList.Remove(message);
      }

      BadgeValue = ImportantMessagesList.Count;
    }

    private void MessgeTypeVisibilityChanged(object sender, PropertyChangedEventArgs e)
    {
      ApplyFilter();
    }

    private void OnFacilityElementMessage(object sender, FacilityMessageArgs args)
    {
      lock (Sync)
      {
        FacilityElement element = (FacilityElement)sender;
        FacilityMessage message = new FacilityMessage(element, args);

        if (element == null || message == null)
          return;

        if (App.Current == null)
          return;

        App.Current.Dispatcher.Invoke((Action)delegate
        {
          MessagesList?.Insert(0, message);
        });
      }
    }

    public void Clear()
    {
      lock (Sync)
      {
        MessagesList.Clear();
      }
    }

    public override void Stop()
    {
    }

    /// <summary>
    /// Список сообщений
    /// </summary>
    public ObservableCollection<FacilityMessage> MessagesList
    {
      get;
      private set;
    } = new ObservableCollection<FacilityMessage>();


    /// <summary>
    /// Список важных сообщений
    /// </summary>
    public ObservableCollection<FacilityMessage> ImportantMessagesList
    {
      get;
      private set;
    } = new ObservableCollection<FacilityMessage>();

    /// <summary>
    /// Словарь для фильтрации сообщений по типу
    /// </summary>
    public MessageTypeVisibilityObservableCollection ShowMessgeType
    {
      get;
      set;
    } = new MessageTypeVisibilityObservableCollection
    {
      new MessageTypeVisibility { Type = MessageType.Trace,     IsVisible = false },
      new MessageTypeVisibility { Type = MessageType.Debug,     IsVisible = false },
      new MessageTypeVisibility { Type = MessageType.Info,      IsVisible = true  },
      new MessageTypeVisibility { Type = MessageType.Success,   IsVisible = true  },      
      new MessageTypeVisibility { Type = MessageType.Warning,   IsVisible = true  },
      new MessageTypeVisibility { Type = MessageType.Error,     IsVisible = true  },
      new MessageTypeVisibility { Type = MessageType.Exception, IsVisible = true  }
    };

    /// <summary>
    /// Метод для фильтрации
    /// </summary>
    /// <param name="item">Элемент списка</param>
    /// <returns>true - если элемент должен отображаться; false - если элемент не должен отображаться </returns>
    private bool UserFilter(object item)
    {
      FacilityMessage msg = item as FacilityMessage;

      // Если мы не можем привести элемент к типу FacilityMessage, 
      // или если его типа нет в словаре, не показываем элемент
      if (msg == null) 
        return false;

      // Найдите соответствующий MessageTypeVisibility для этого типа сообщения и проверьте IsVisible
      var messageTypeVisibility = ShowMessgeType.FirstOrDefault(mt => mt.Type == msg.MessageType);
      
      return messageTypeVisibility != null && messageTypeVisibility.IsVisible;
      
      
    }

    // Пример использования
    public void ApplyFilter()
    {
      ICollectionView view = CollectionViewSource.GetDefaultView(MessagesList);
      view.Filter = UserFilter;
    }

  }
}
