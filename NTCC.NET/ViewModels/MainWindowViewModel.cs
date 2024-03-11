using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ControlzEx;
using MahApps.Metro.IconPacks;

namespace NTCC.NET.ViewModels
{
  internal class MainWindowViewModel : ViewModelBase
  {

    public MainWindowViewModel()
    {
      this.AssemblyTitle = $"{Assembly.GetExecutingAssembly().GetName().Name} v.{Assembly.GetExecutingAssembly().GetName().Version.ToString()}";
    }
    static Style iconStyle = (Style)Application.Current.Resources["MetroToolBoxIconStyle32"];

    private List<ViewModelBase> _pages = new List<ViewModelBase>()
        {

            new StagesViewModel()
            {
                Title = "Stages",
                Description = "Description",
                PageIcon = new PackIconMaterialDesign()
                {
                    Kind = PackIconMaterialDesignKind.TimeToLeave,
                    Style = iconStyle

                }
            },

             new TablesViewModel()
            {
                Title = "Tables",
                Description = "Description",
                PageIcon = new PackIconMaterialDesign()
                {
                    Kind = PackIconMaterialDesignKind.List,
                    Style = iconStyle
                }
            },

            new FacilityViewModel()
            {
                Title = "Система нагрева",
                Description = "Description",
                PageIcon = new PackIconMaterial()
                {
                    Kind = PackIconMaterialKind.Radiator,
                    Style = iconStyle
                }
            },
            new MessagesViewModel()
            {
                Title = "Messages",
                Description = "Description",
                PageIcon = new PackIconMaterialDesign()
                {
                    Kind = PackIconMaterialDesignKind.Message,
                    Style = iconStyle
                }
            },
            /*new SettingViewModel()
            {
                Title = "Settings",  
                Description = "Description",
                PageIcon = new PackIconMaterialDesign()
                {
                    Kind = PackIconMaterialDesignKind.Settings,
                    Style = iconStyle
                }
            },*/
           
        };

    public string AssemblyTitle { get; }


    public List<ViewModelBase> Pages
    {
      get => _pages;
      private set
      {
        _pages = value;
        OnPropertyChanged();
      }
    }
  }
}
