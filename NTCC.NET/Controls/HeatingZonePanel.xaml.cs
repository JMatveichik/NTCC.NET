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

namespace NTCC.NET.Controls
{
  /// <summary>
  /// Interaction logic for HeatingZonePanel.xaml
  /// </summary>
  public partial class HeatingZonePanel : UserControl
  {
    public HeatingZonePanel()
    {
      InitializeComponent();
    }

    private void SwitchHeaterPower(object sender, RoutedEventArgs e)
    {
      Button button = (Button)sender;
      DiscreteOutputDataPoint powerSwitchDataPoint = (DiscreteOutputDataPoint)button.Tag;

      powerSwitchDataPoint.SetState(!powerSwitchDataPoint.State);
    }

    private void SwitchHeatingControl(object sender, RoutedEventArgs e)
    {
      Button button = (Button)sender;
      ReactorHeatingZone heatingZone = (ReactorHeatingZone)button.Tag;

      if (heatingZone.IsControlStarted)
        heatingZone.StopControl();
      else
        heatingZone.StartControl();
    }
  }
}
