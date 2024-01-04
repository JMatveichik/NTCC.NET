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
    /// Interaction logic for DataPointPreviewCtrl.xaml
    /// </summary>
    public partial class DataPointPreviewCtrl : UserControl
    {
        public DataPointPreviewCtrl()
        {
            InitializeComponent();
        }

        private void SwitchDiscreteOutput(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            DiscreteOutputDataPoint dataPoint = button.Tag as DiscreteOutputDataPoint;

            dataPoint.SetState(!dataPoint.State);
        }

        private void SetAnalogOutput(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            AnalogOutputDataPoint dataPoint = button.Tag as AnalogOutputDataPoint;

            dataPoint.WriteValue(dataPoint.ValueToSet);
        }
    }
}
