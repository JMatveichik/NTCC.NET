using MaterialDesignExtensions.Model;
using NTCC.NET.Core.Facility;
using NTCC.NET.Core.Stages;
using NTCC.NET.Notifications;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using ToastNotifications;
using ToastNotifications.Core;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using ToastNotifications.Messages;

namespace NTCC.NET.ViewModels
{
  class StagesViewModel : PageViewModel
  {
    #region notifier configuration

    private Notifier notifier;

    public Notifier CreateNotifier(Corner corner, NotificationLifetimeType lifetime)
    {
      notifier?.Dispose();
      notifier = null;

      return new Notifier(cfg =>
      {
        cfg.PositionProvider = CreatePositionProvider(corner);
        cfg.LifetimeSupervisor = CreateLifetimeSupervisor(lifetime);
        cfg.Dispatcher = Application.Current.Dispatcher;
        cfg.DisplayOptions.TopMost = TopMost.GetValueOrDefault();
      });
    }

    public void ChangePosition(Corner corner, NotificationLifetimeType lifetime)
    {
      notifier = CreateNotifier(corner,  lifetime);
    }

    private void MainWindowOnClosing(object sender, CancelEventArgs cancelEventArgs)
    {
      notifier.Dispose();
    }

    private static INotificationsLifetimeSupervisor CreateLifetimeSupervisor(NotificationLifetimeType lifetime)
    {
      if (lifetime == NotificationLifetimeType.Basic)
        return new CountBasedLifetimeSupervisor(MaximumNotificationCount.FromCount(7));

      return new TimeAndCountBasedLifetimeSupervisor(TimeSpan.FromSeconds(5),
          MaximumNotificationCount.UnlimitedNotifications());
    }

    private static IPositionProvider CreatePositionProvider(Corner corner)
    {
      return new PrimaryScreenPositionProvider(corner, 5, 5);
    }

    public bool? FreezeOnMouseEnter { get; set; } = true;
    public bool? ShowCloseButton { get; set; } = false;
    public bool? TopMost { get; set; } = true;

    #endregion


    #region notifier messages

    internal void ShowWarning(string message)
    {
      notifier.ShowWarning(message, CreateOptions());
      RememberMessage(message);
    }

    internal void ShowSuccess(string message)
    {
      notifier.ShowSuccess(message, CreateOptions());
      RememberMessage(message);
    }

    public void ShowInformation(string message)
    {
      notifier.ShowInformation(message, CreateOptions());
      RememberMessage(message);
    }

    public void ShowError(string message)
    {
      notifier.ShowError(message, CreateOptions());
      RememberMessage(message);
    }

    private string _lastMessage = "";
    private void RememberMessage(string message)
    {
      if (_messageCounter % 3 == 0)
      {
        _lastMessage = message;
      }
    }

    private int _messageCounter = 0;

    private MessageOptions CreateOptions()
    {
      return new MessageOptions
      {
        FreezeOnMouseEnter = FreezeOnMouseEnter.GetValueOrDefault(),
        ShowCloseButton = ShowCloseButton.GetValueOrDefault(),
        Tag = ++_messageCounter % 2
      };
    }

    #endregion

    public StagesViewModel()
    {
      notifier = CreateNotifier(Corner.TopRight, NotificationLifetimeType.TimeBased);
      Application.Current.MainWindow.Closing += MainWindowOnClosing;

      FullCycle.Tick += OnStageTick;
      foreach (StageBase stage in FullCycle.Stages)
      {
        stage.Tick += OnStageTick;
      }
    }

    private void OnStageTick(object sender, FacilityMessageArgs args)
    {
      string message = args.Message;

      switch (args.MessageType)
      {
        case MessageType.Info:
          ShowInformation(message);
          break;
        case MessageType.Debug:
          ShowInformation(message);
          break;
        case MessageType.Warning:
          ShowWarning(message);
          break;
        case MessageType.Error:
          ShowError(message);
          break;
        case MessageType.Exception:
          ShowError(message);
          break;
        default:
          ShowInformation(message);
          break;
      }
    }


    public StageMain FullCycle
    {
      get;
      private set;
    } = ArtMonbatFacility.FullCycle;

    public FaciliitySet<DataPoint> DataPoints
    {
      get;
      private set;
    } = ArtMonbatFacility.DataPoints;

    public FaciliitySet<ReactorHeatingZone> ReactorZones
    {
      get;
      private set;
    } = ArtMonbatFacility.ReactorZones;
   
    public override void Stop()
    {
    }

  }
}
