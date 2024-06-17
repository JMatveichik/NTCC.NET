using NTCC.NET.Core.Facility;
using System;
using System.Collections.Generic;
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
using NTCC.NET.Core.Stages;
using NTCC.NET.Commands;
using NTCC.NET.Dialogs;

namespace NTCC.NET.Pages
{
  /// <summary>
  /// Interaction logic for StagesPage.xaml
  /// </summary>
  public partial class StagesPage : UserControl
  {
    public StagesPage()
    {
      InitializeComponent();
      this.CommandBindings.Add(new CommandBinding(FacilityCommands.StartFullCycle, 
                                StartFullCycleExecuted, 
                                StartFullCycleCanExecute));

      this.CommandBindings.Add(new CommandBinding(FacilityCommands.StopFullCycle, 
                                StopFullCycleExecuted, 
                                StopFullCycleCanExecute));

      this.CommandBindings.Add(new CommandBinding(FacilityCommands.SkipCurrentStage, 
                                SkipCurrentStageExecuted, 
                                SkipCurrentStageCanExecute));

      this.CommandBindings.Add(new CommandBinding(FacilityCommands.SwitchDamperState,
                                SwitchDamperStateExecuted,
                                SwitchDamperStateCanExecute));
    }

    private void SwitchDamperStateCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = true;
    }

    private void SwitchDamperStateExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      var dumper = ArtMonbatFacility.Damper;
      if (dumper == null)
        return;

      if (dumper.IsControlStarted)
        dumper.StopControl();
      else 
        dumper.StartControl();
    }

    private void StopFullCycleCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      StageBase currentStage = ArtMonbatFacility.FullCycle.CurrentStage;

      if (currentStage == null ||
          currentStage.State == StageState.Wait ||
          currentStage.State == StageState.Completed)
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

    private void StartFullCycleCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      StageBase fullCycleStage = ArtMonbatFacility.FullCycle;

      if (fullCycleStage == null ||
          fullCycleStage.State == StageState.Wait ||
          fullCycleStage.State == StageState.Completed)
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

    private void SkipCurrentStageCanExecute(object sender, CanExecuteRoutedEventArgs e)
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
  }
}
