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
        /*
        protected override void On(CancelEventArgs e)
        {
            MainWindowViewModel mainViewModel = (MainWindowViewModel)DataContext;

            foreach (PageViewModel model in mainViewModel.Pages)
            {
                model.Stop();
            }

            base.OnClosing(e);
        }*/

    }
}
