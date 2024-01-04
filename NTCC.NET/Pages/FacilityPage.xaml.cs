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

namespace NTCC.NET.Pages
{
    /// <summary>
    /// Interaction logic for FacilityPage.xaml
    /// </summary>
    public partial class FacilityPage : UserControl
    {
        public FacilityPage()
        {
            InitializeComponent();   
            
        }

        private void OnHeaterStartControl(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            ReactorHeatingZone hz = (ReactorHeatingZone)button.Tag;
            hz.StartControl();
        }

        private void OnHeaterStopControl(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            ReactorHeatingZone hz = (ReactorHeatingZone)button.Tag;
            hz.StopControl();
        }
    }
}
