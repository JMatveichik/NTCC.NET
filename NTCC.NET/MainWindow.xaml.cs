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
using NTCC.NET.Commands;
using NTCC.NET.Core.Facility;
using NTCC.NET.Core.Stages;
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

            this.CommandBindings.Add(new CommandBinding(FacilityCommands.StartFullCycle, StartFullCycleExecuted, StartFullCycleCanExecuted));
            this.CommandBindings.Add(new CommandBinding(FacilityCommands.StopFullCycle, StopFullCycleExecuted, StopFullCycleCanExecuted));
        }

        private void StopFullCycleCanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void StopFullCycleExecuted(object sender, ExecutedRoutedEventArgs e)
        {
        }

        private void StartFullCycleCanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void StartFullCycleExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            StageMain fullCycleStage = ArtMonbatFacility.FullCycle;
            Task.Factory.StartNew<StageResult>(() => fullCycleStage.Do());
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

    }
}
