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

      this.CommandBindings.Add(new CommandBinding(FacilityCommands.StartFullCycle, StartFullCycleExecuted, StartFullCycleCanExecuted));
      this.CommandBindings.Add(new CommandBinding(FacilityCommands.StopFullCycle, StopFullCycleExecuted, StopFullCycleCanExecuted));

      this.CommandBindings.Add(new CommandBinding(FacilityCommands.SetAnalogOutputValue, SetAnalogOutputValueExecuted, SetAnalogOutputValueCanExecuted));
      this.CommandBindings.Add(new CommandBinding(FacilityCommands.SwitchDiscreteOutputValue, SwitchDiscreteOutputExecuted, SwitchDiscreteOutputCanExecuted));

      this.CommandBindings.Add(new CommandBinding(FacilityCommands.HeatingZoneParameters, ExecuteHeatingZoneParameters, HeatingZoneParametersCanExecute));
      this.CommandBindings.Add(new CommandBinding(FacilityCommands.SwitchHeatingZonePower, ExecuteSwitchHeatingZonePower, SwitchHeatingZonePowerCanExecute));
      
    }

    private void SwitchHeatingZonePowerCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = true;
    }

    /// <summary>
    /// Включить.выключить питание зоны нагрева
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void ExecuteSwitchHeatingZonePower(object sender, ExecutedRoutedEventArgs e)
    {
      ReactorHeatingZone zone = e.Parameter as ReactorHeatingZone;
     
      if (zone != null)
      {
        bool curState = zone.PowerState.State;
        string message = curState ? $"Вы уверены, что хотите выключить питание {zone.Description}?" :
                                    $"Вы уверены, что хотите включить питание {zone.Description}?";

        bool? Result = new CustomMessageBox(message, Dialogs.MessageType.Confirmation, MessageButtons.YesNo).ShowDialog();

        ///если пользователь подтвердил действие 
        ///задаем нулевую мощность и включаем/выключаем зону
        if (Result.Value)
        {
          zone.DutyWrite.WriteValue(0.0);
          zone.Run.SetState(!curState);
        }
      }
    }

    private void HeatingZoneParametersCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = true;
    }

    private void ExecuteHeatingZoneParameters(object sender, ExecutedRoutedEventArgs e)
    {
      ReactorHeatingZone zone = e.Parameter as ReactorHeatingZone;

      if (zone != null)
      {
        HeatingZoneParametersDialog dialog = new HeatingZoneParametersDialog(zone);
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

    private void StopFullCycleCanExecuted(object sender, CanExecuteRoutedEventArgs e)
    {
      //TODO : проверить возможность остановки процесса
      e.CanExecute = true;
    }

    private void StopFullCycleExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      StageMain fullCycleStage = ArtMonbatFacility.FullCycle;
      fullCycleStage.Stop();
    }

    private void StartFullCycleCanExecuted(object sender, CanExecuteRoutedEventArgs e)
    {
      StageMain fullCycleStage = ArtMonbatFacility.FullCycle;
      if (fullCycleStage == null)
      {
        e.CanExecute = false;
        return;
      }

      if (fullCycleStage.State == StageState.Wait || fullCycleStage.State == StageState.Completed)
      {
        e.CanExecute = true;
        return;
      }

      e.CanExecute = true;
    }

    private void StartFullCycleExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      StageMain fullCycleStage = ArtMonbatFacility.FullCycle;
      Task.Factory.StartNew<StageResult>(() => fullCycleStage.Do());
    }

    protected override void OnClosed(EventArgs e)
    {
      MainWindowViewModel mainViewModel = (MainWindowViewModel)DataContext;

      foreach (PageViewModel model in mainViewModel.Pages)
      {
        model.Stop();
      }

      base.OnClosed(e);
    }

  }
}
