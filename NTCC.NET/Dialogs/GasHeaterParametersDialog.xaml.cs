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
  /// Interaction logic for GasHeaterParametersDialog.xaml
  /// </summary>
  public partial class GasHeaterParametersDialog : Window
  {
    public GasHeaterParametersDialog()
    {
      InitializeComponent();
      DataContext = ArtMonbatFacility.GasHeater;

      this.CommandBindings.Add(new CommandBinding(FacilityCommands.SwitchHeatingZonePower, SwitchHeatingZonePowerExecuted, SwitchHeatingZonePowerCanExecute));
      this.CommandBindings.Add(new CommandBinding(FacilityCommands.SwitchHeatingZoneControl, SwitchHeatingZoneControlExecuted, SwitchHeatingZoneControlCanExecute));

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
      GasHeater heater = e.Parameter as GasHeater;

      if (heater != null)
      {
        bool curState = heater.HeaterState.State;
        string message = curState ? $"Вы уверены, что хотите выключить питание {heater.Description}?" :
                                    $"Вы уверены, что хотите включить питание {heater.Description}?";

        bool? Result = new CustomMessageBox(message, Dialogs.MessageType.Confirmation, MessageButtons.YesNo).ShowDialog();

        ///если пользователь подтвердил действие 
        ///задаем нулевую мощность и включаем/выключаем зону
        if (Result.Value)
        {
          heater.HeaterState.SetState(!curState);
        }
      }
    }

    private void SwitchHeatingZoneControlCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = true;
    }

    private void SwitchHeatingZoneControlExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      GasHeater heater = e.Parameter as GasHeater;

      if (heater != null)
      {
        if (heater.IsControlStarted)
        {
          string message = $"Вы уверены, что хотите прекратить автоматический контроль температуры  {heater.Description}?";

          bool? Result = new CustomMessageBox(message, Dialogs.MessageType.Confirmation, MessageButtons.YesNo)
            .ShowDialog();

          ///если пользователь подтвердил действие выключаем контроль зоны
          if (Result.Value)
          {
            heater.StopControl();
          }
        }
        else
        {
          heater.StartControl();
        }
      }
    }

    private void btnCloseClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = false;
      this.Close();
    }

    private void btnApplyClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = false;
      this.Close();
    }

    private void btnSaveClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = false;
      this.Close();
    }
  }
}
