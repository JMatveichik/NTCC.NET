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
  /// Interaction logic for GasHeaterInlineCtrl.xaml
  /// </summary>
  public partial class GasHeaterInlineCtrl : UserControl
  {
    public GasHeaterInlineCtrl()
    {
      InitializeComponent();
      DataContext = ArtMonbatFacility.GasHeater;
    }

    private void OnCardMouseEnter(object sender, MouseEventArgs e)
    {
      GasHeaterQuickControl.Visibility = Visibility.Visible;
    }
    private void OnCardMouseLeave(object sender, MouseEventArgs e)
    {
      GasHeaterQuickControl.Visibility = Visibility.Collapsed;
    }

  }
}
