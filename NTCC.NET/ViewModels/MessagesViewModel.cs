using NTCC.NET.Core.Facility;
using NTCC.NET.Core.Stages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCC.NET.ViewModels
{
  internal class MessagesViewModel : PageViewModel
  {

    private readonly object Sync = new object();

    public MessagesViewModel()
    {
      ArtMonbatFacility facility = ArtMonbatFacility.Instance;

      foreach (ReactorHeatingZone zone in ArtMonbatFacility.ReactorZones.Items.Values)
      {
        zone.Tick += OnHeatingZoneMessage;
      }

      ArtMonbatFacility.FullCycle.Tick += OnHeatingZoneMessage;
      foreach (StageBase stage in ArtMonbatFacility.FullCycle.Stages)
      {
        stage.Tick += OnHeatingZoneMessage;
      }
    }

    private void OnHeatingZoneMessage(object sender, FacilityMessageArgs args)
    {
      lock (Sync)
      {
        FacilityElement element = (FacilityElement)sender;
        FacilityMessage message = new FacilityMessage(element, args);

        App.Current.Dispatcher.Invoke((Action)delegate
        {
          MessagesList.Insert(0, message);
        });
      }
    }

    public void Clear()
    {
      lock (Sync)
      {
        MessagesList.Clear();
      }
    }

    public override void Stop()
    {
    }

    public ObservableCollection<FacilityMessage> MessagesList
    {
      get;
      private set;
    } = new ObservableCollection<FacilityMessage>();

  }
}
