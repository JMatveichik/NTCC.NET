using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using NTCC.NET.Commands;
using NTCC.NET.Core.Facility;
using NTCC.NET.Core.Stages;
using NTCC.NET.ViewModels;
using NTCC.NET.Dialogs;
using static System.Net.Mime.MediaTypeNames;


namespace NTCC.NET
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : MetroWindow
  {
    public MainWindow()
    {
      InitializeComponent();
      DataContext = new MainWindowViewModel();

      this.Closing += OnClosing;

      this.CommandBindings.Add(new CommandBinding(FacilityCommands.SetAnalogOutputValue, SetAnalogOutputValueExecuted, SetAnalogOutputValueCanExecuted));
      this.CommandBindings.Add(new CommandBinding(FacilityCommands.SwitchDiscreteOutputValue, SwitchDiscreteOutputExecuted, SwitchDiscreteOutputCanExecuted));

      this.CommandBindings.Add(new CommandBinding(FacilityCommands.HeatingZoneParameters, HeatingZoneParametersExecuted, HeatingZoneParametersCanExecute));
      this.CommandBindings.Add(new CommandBinding(FacilityCommands.StageParameters, StageParametersExecuted, StageParametersCanExecute));
      this.CommandBindings.Add(new CommandBinding(FacilityCommands.GasHeaterParameters, GasHeaterParametersExecuted, GasHeaterParametersCanExecute));


    }



    private void StopFullCycleCanExecuted(object sender, CanExecuteRoutedEventArgs e)
    {
      if (ArtMonbatFacility.FullCycle.CurrentStage == null ||
          ArtMonbatFacility.FullCycle.CurrentStage == ArtMonbatFacility.FullCycle)
      {
        e.CanExecute = false;
        return;
      }
      e.CanExecute = true;
    }

    private void StopFullCycleExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      string message = $"Вы уверены, что хотите остановить технологический цикл ?";

      bool? Result = new CustomMessageBox(message, Dialogs.MessageType.Confirmation, MessageButtons.YesNo).ShowDialog();
      if (Result.Value)
      {
        ArtMonbatFacility.FullCycle.CurrentStage.Stop();
      }
    }




    private void SkipCurrentStageCanExecuted(object sender, CanExecuteRoutedEventArgs e)
    {
      if (ArtMonbatFacility.FullCycle.CurrentStage == null ||
          ArtMonbatFacility.FullCycle.CurrentStage == ArtMonbatFacility.FullCycle)
      {
        e.CanExecute = false;
        return;
      }
      e.CanExecute = true;
    }

    private void SkipCurrentStageExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      StageBase currentStage = ArtMonbatFacility.FullCycle.CurrentStage;
      string message = $"Вы уверены, что хотите пропустить стадию {currentStage.Description} технологический цикл ?";

      bool? Result = new CustomMessageBox(message, Dialogs.MessageType.Confirmation, MessageButtons.YesNo).ShowDialog();
      if (Result.Value)
      {
        currentStage.Skip();
      }
    }



    private void HeatingZoneParametersCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = true;
    }

    private void HeatingZoneParametersExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      ReactorHeatingZone zone = e.Parameter as ReactorHeatingZone;

      if (zone != null)
      {
        HeatingZoneParametersDialog dialog = new HeatingZoneParametersDialog(zone);
        dialog.ShowDialog();
      }
    }

    private void StageParametersCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = true;
    }

    private void StageParametersExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      StageBase stage = e.Parameter as StageBase;

      if (stage != null)
      {
        StageParametersDialog dialog = new StageParametersDialog(stage);
        dialog.ShowDialog();
      }
    }

    private void GasHeaterParametersCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = true;
    }

    private void GasHeaterParametersExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      GasHeater heater = e.Parameter as GasHeater;

      if (heater != null)
      {
        GasHeaterParametersDialog dialog = new GasHeaterParametersDialog();
        dialog.ShowDialog();
      }
    }


    private void SwitchDiscreteOutputCanExecuted(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = true;
    }

    private void SwitchDiscreteOutputExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      DiscreteOutputDataPoint datapoint = e.Parameter as DiscreteOutputDataPoint;
      if (datapoint != null)
      {
        bool curState = datapoint.State;
        datapoint.SetState(!curState);
      }
    }

    private void SetAnalogOutputValueCanExecuted(object sender, CanExecuteRoutedEventArgs e)
    {
      AnalogOutputDataPoint datapoint = e.Parameter as AnalogOutputDataPoint;
      if (datapoint == null)
      {
        e.CanExecute = false;
        return;
      }

      if (datapoint.Value == datapoint.ValueToSet ||
          datapoint.Value < datapoint.MinValue ||
          datapoint.Value > datapoint.MaxValue)
      {
        e.CanExecute = false;
        return;
      }
      e.CanExecute = true;
    }

    private void SetAnalogOutputValueExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      AnalogOutputDataPoint datapoint = e.Parameter as AnalogOutputDataPoint;
      if (datapoint != null)
      {
        datapoint.WriteValue(datapoint.ValueToSet);
      }
    }

    private void OnClosing(object sender, CancelEventArgs e)
    {
      string message = $"Вы уверены, что хотите закрыть приложение?";

      bool? Result = new CustomMessageBox(message, Dialogs.MessageType.Confirmation, MessageButtons.YesNo).ShowDialog();
      if (!Result.Value)
      {
        e.Cancel = true;
        return;
      }

      //останавливаем потоки в моделях отображения
      MainWindowViewModel mainViewModel = (MainWindowViewModel)DataContext;
      foreach (PageViewModel model in mainViewModel.Pages)
      {
        model.Stop();
      }

      //останваливаем установку
      ArtMonbatFacility.Instance.Stop();
    }

  }
}
