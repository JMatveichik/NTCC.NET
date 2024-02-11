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
    }

    private void btnClose_Click(object sender, RoutedEventArgs e)
    {
      this.DialogResult = false;
      this.Close();
    }

  }
}
