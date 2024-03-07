using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using NTCC.NET.Core.Facility;
using NTCC.NET.Dialogs;
using NTCC.NET.Properties;
using NTCC.NET.ViewModels;

namespace NTCC.NET
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    protected override void OnStartup(StartupEventArgs e)
    {
      Duplicate  = new Mutex(true, ResourceAssembly.GetName().Name);

      if (!Duplicate.WaitOne())
      {
        isDuplicate = true;
        Current.Shutdown();
        return;
      }

      try
      {
        string configDir = Settings.Default.ConfigDirectory;
        ArtMonbatFacility.Instance.Initialize(configDir);

        ArtMonbatFacility.FullCycle.ContinueCycleConfirmation = new DialogUserConfirmation("Перейти на следующий цикл ? ");
      }
      catch (Exception ex)
      {
        bool? Result = new CustomMessageBox(ex.Message, Dialogs.MessageType.Error, MessageButtons.Ok).ShowDialog();
        Current.Shutdown();
        return;
      }
      
      
      new MainWindow().Show();
      ShutdownMode = ShutdownMode.OnLastWindowClose;

      base.OnStartup(e);
    }

    protected Mutex Duplicate;
    protected bool isDuplicate = false;

    protected override void OnExit(ExitEventArgs e)
    {

      if (!isDuplicate)
      {
        ArtMonbatFacility facility = ArtMonbatFacility.Instance;
        facility.Stop();
      }
      base.OnExit(e);
    }
  }
}
