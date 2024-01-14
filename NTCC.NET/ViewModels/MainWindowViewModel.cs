using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ControlzEx;
using MahApps.Metro.IconPacks;

namespace NTCC.NET.ViewModels
{
    internal class MainWindowViewModel : ViewModelBase
    {
        static Style iconStyle = (Style)Application.Current.Resources["MetroToolBoxIconStyle32"];

        private List<ViewModelBase> _pages = new List<ViewModelBase>()
        {
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
            new SettingViewModel()
            {
                Title = "Settings",  
                Description = "Description",
                PageIcon = new PackIconMaterialDesign()
                {
                    Kind = PackIconMaterialDesignKind.Settings,
                    Style = iconStyle
                }
            },
           
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
        };

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
