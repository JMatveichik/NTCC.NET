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
      e.CanExecute = true;
    }

    private void StopFullCycleExecuted(object sender, ExecutedRoutedEventArgs e)
    {
    }

    private void StartFullCycleCanExecuted(object sender, CanExecuteRoutedEventArgs e)
    {
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
