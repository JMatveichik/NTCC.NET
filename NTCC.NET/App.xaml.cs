using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using NTCC.NET.Core.Facility;
using NTCC.NET.Properties;
using NTCC.NET.ViewModels;

namespace NTCC.NET
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ArtMonbatFacility facility = ArtMonbatFacility.Instance;


            try
            {
                string configDir = Settings.Default.ConfigDirectory;
                facility.Initialize(configDir);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка инициализации ...", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        protected override void OnExit(ExitEventArgs e)
        {

            ArtMonbatFacility facility = ArtMonbatFacility.Instance;
            facility.Stop();

            base.OnExit(e);
        }
    }
}
