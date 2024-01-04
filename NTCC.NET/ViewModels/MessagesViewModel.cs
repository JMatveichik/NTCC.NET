using NTCC.NET.Core.Facility;
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

            foreach(ReactorHeatingZone zone in ArtMonbatFacility.ReactorHeaters.Items.Values)
            {
                zone.Tick += OnHeatingZoneMessage;
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
                    MessagesList.Add(message);
                });
            }
        }

        public void Clear()
        {
            lock(Sync)
            {
                MessagesList.Clear();
            }
        }

        public ObservableCollection<FacilityMessage> MessagesList
        {
            get;
            private set;
        } = new ObservableCollection<FacilityMessage>();

    }
}
