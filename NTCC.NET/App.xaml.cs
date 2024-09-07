using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using NTCC.NET.Core.Facility;
using NTCC.NET.Core.Stages;
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
      AppLogger.Info("Запуск приложения...");
      //new CustomMessageBox("Запуск приложения ...", Dialogs.MessageType.Info, MessageButtons.Ok).ShowDialog();

      Duplicate  = new Mutex(true, ResourceAssembly.GetName().Name);

      if (!Duplicate.WaitOne())
      {
        AppLogger.Info("Приложение уже запущено. Закрытие копии приложения.");
        isDuplicate = true;
        Current.Shutdown();
        return;
      }

      try
      {
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

        string configDir = Settings.Default.ConfigDirectory;
        AppLogger.Info($"Загрузка конфигурации элементов установки <{configDir}>");

        ArtMonbatFacility.Instance.Initialize(configDir);

        //устанавливаем обработчик подтверждения действий пользователя из рабочего цикла
        StageBase.UserConfirmation = new DialogUserConfirmation();

        RegisterLoggingSources();
        
      }
      catch (Exception ex)
      {
        AppLogger.Error(ex.Message);

        bool? Result = new CustomMessageBox(ex.Message, Dialogs.MessageType.Error, MessageButtons.Ok).ShowDialog();
        Current.Shutdown();
        return;
      }
      
      
      new MainWindow().Show();
      ShutdownMode = ShutdownMode.OnLastWindowClose;

      base.OnStartup(e);
    }

    private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
      string message = $"Необработанная ошибка приложения : {e.ExceptionObject.ToString()}";
      
      AppLogger.Error(message);
      bool? Result = new CustomMessageBox(message, Dialogs.MessageType.Error, MessageButtons.Ok).ShowDialog();

      Current.Shutdown();
    }

    protected Mutex Duplicate;
    protected bool isDuplicate = false;

    protected override void OnExit(ExitEventArgs e)
    {
      if (!isDuplicate)
      {
        ArtMonbatFacility.Instance.Stop();
      }
      base.OnExit(e);
    }

    private static NLog.Logger AppLogger = NLog.LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Регистрация источников логирования
    /// </summary>
    private void RegisterLoggingSources()
    {
      AppLogger.Debug($"Регистрация обработчиков для логирования состояния точек данных...");

      //регистрируем обработчик логирования для точек данных 
      foreach (var dataPoint in ArtMonbatFacility.DataPoints.Items.Values)
      {
        RegisterLogSource(dataPoint);
      }

      AppLogger.Debug($"Регистрация обработчиков для логирования состояния устройств управления и сбора данных...");

      //регистрируем обработчик логирования для устройств
      foreach (var device in ArtMonbatFacility.Devices.Items.Values)
      {
        RegisterLogSource(device);
      }

      AppLogger.Debug($"Регистрация обработчиков для логировани протекания технологического цикла...");

      //регистрируем обработчик логирования для стадий
      foreach (var stage in ArtMonbatFacility.Stages.Items.Values)
      {
        RegisterLogSource(stage);
      }

      AppLogger.Debug($"Регистрация обработчиков для логирования состояния точек данных зон нагрева...");

      //регистрируем обработчик логирования для зон нагрева
      foreach (var heatingZone in ArtMonbatFacility.ReactorZones.Items.Values)
      {
        RegisterLogSource(heatingZone);
      }

      AppLogger.Debug($"Регистрация обработчиков для логирования состояния подогревателя газа...");

      //регистрируем обработчик логирования для подогревателя газа
      RegisterLogSource(ArtMonbatFacility.GasHeater);

      AppLogger.Debug($"Регистрация обработчиков для логирования работы заслонки выгрузки ...");

      //регистрируем обработчик логирования для управления заслонкой
      RegisterLogSource(ArtMonbatFacility.Damper);

      AppLogger.Debug($"Регистрация обработчиков для логирования работы скребка ...");

      //регистрируем обработчик логирования для управления скребком
      RegisterLogSource(ArtMonbatFacility.Scrapper);

    }
    /// <summary>
    /// Регистрация обработчика логирования для элемента управления
    /// </summary>
    /// <param name="facilityElement">Элемент установки</param>
    public static void RegisterLogSource(FacilityElement facilityElement)
    {
      facilityElement.Tick += OnFacilityElementMessage;
    }

    /// <summary>
    /// Обработчик сообщений от элемента управления
    /// </summary>
    /// <param name="sender">Отправитель сообщения</param>
    /// <param name="args"></param>
    private static void OnFacilityElementMessage(object sender, FacilityMessageArgs args)
    {
      switch (args.MessageType)
      {
        case NTCC.NET.Core.Facility.MessageType.Info:
          AppLogger.Info(args.Message);
          break;

        case NTCC.NET.Core.Facility.MessageType.Trace:
          AppLogger.Trace(args.Message);
          break;

        case NTCC.NET.Core.Facility.MessageType.Debug:
          AppLogger.Debug(args.Message);
          break;

        case NTCC.NET.Core.Facility.MessageType.Warning:
          AppLogger.Warn(args.Message);
          break;

        case NTCC.NET.Core.Facility.MessageType.Success:
          AppLogger.Info(args.Message);
          break;

        case NTCC.NET.Core.Facility.MessageType.Error:
          AppLogger.Error(args.Message);
          break;

        case NTCC.NET.Core.Facility.MessageType.Exception:
          AppLogger.Fatal(args.Message);
          break;
      }
    }
  }
}
