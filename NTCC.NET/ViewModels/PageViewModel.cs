using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahApps.Metro.IconPacks;

namespace NTCC.NET.ViewModels
{
  abstract class PageViewModel : ViewModelBase
  {
    public PackIconBase _icon = null;

    public abstract void Stop();

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
