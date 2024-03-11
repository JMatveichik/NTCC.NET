using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCC.NET.ViewModels
{
  internal class CurrentTimeViewModel : ViewModelBase
  {
    private DateTime currentTime;
    public DateTime CurrentTime
    {
      get { return currentTime; }
      set
      {
        if (value == currentTime)
          return;

        currentTime = value;
        OnPropertyChanged();
      }
    }

    public CurrentTimeViewModel()
    {
      CurrentTime = DateTime.Now;
      Task.Run(() => UpdateTime());
    }

    private async void UpdateTime()
    {
      while (true)
      {
        await Task.Delay(1000);
        CurrentTime = DateTime.Now;
      }
    }
  }
}
