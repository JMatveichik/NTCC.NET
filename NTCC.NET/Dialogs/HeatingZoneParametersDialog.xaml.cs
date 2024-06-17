using NTCC.NET.Commands;
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
using System.Windows.Shapes;

namespace NTCC.NET.Dialogs
{
  /// <summary>
  /// Interaction logic for HeatingZoneParametersDialog.xaml
  /// </summary>
  public partial class HeatingZoneParametersDialog : Window
  {
    public HeatingZoneParametersDialog(ReactorHeatingZone zone)
    {
      InitializeComponent();
      DataContext = zone;

      this.CommandBindings.Add(new CommandBinding(FacilityCommands.SwitchHeatingZonePower, SwitchHeatingZonePowerExecuted, SwitchHeatingZonePowerCanExecute));
      this.CommandBindings.Add(new CommandBinding(FacilityCommands.SwitchHeatingZoneControl, SwitchHeatingZoneControlExecuted, SwitchHeatingZoneControlCanExecute));
    }

    private void btnCloseClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = false;
      this.Close();
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
    private void SwitchHeatingZonePowerExecuted(object sender, ExecutedRoutedEventArgs e)
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

    private void SwitchHeatingZoneControlCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = true;
    }

    private void SwitchHeatingZoneControlExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      ReactorHeatingZone zone = e.Parameter as ReactorHeatingZone;

      if (zone != null)
      {
        if (zone.IsControlStarted)
        {
          string message = $"Вы уверены, что хотите прекратить автоматический контроль температуры  {zone.Description}?";

          bool? Result = new CustomMessageBox(message, Dialogs.MessageType.Confirmation, MessageButtons.YesNo)
            .ShowDialog();

          ///если пользователь подтвердил действие выключаем контроль зоны
          if (Result.Value)
          {
            zone.StopControl();
          }
        }
        else
        {
          zone.StartControl();
        }
      }
    }

  }
}
