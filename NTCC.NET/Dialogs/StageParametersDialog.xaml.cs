using NTCC.NET.Core.Stages;
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
  /// Interaction logic for StageParametersDialog.xaml
  /// </summary>
  public partial class StageParametersDialog : Window
  {
    public StageParametersDialog(StageBase stage)
    {
      InitializeComponent();
      DataContext = stage;
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
