using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahApps.Metro.IconPacks;

namespace NTCC.NET.ViewModels
{
    internal class PageViewModel : ViewModelBase
    {
        private PackIconBase _icon = null;

        public PackIconBase PageIcon
        {
            get => _icon;
            set
            {
                _icon = value;
                OnPropertyChanged();
            }
        }
    }
}
