using MaterialDesignExtensions.Model;
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
    class StagesViewModel : PageViewModel
    {
        public StagesViewModel()
        {
            Stages = new ObservableCollection<StageBase>();

            foreach (StageBase stage in ArtMonbatFacility.Stages.Items.Values)
            {
                Stages.Add( stage );
                stage.Tick += OnStageTick;
            }
        }

        private void OnStageTick(object sender, FacilityMessageArgs agrs)
        {
            //throw new NotImplementedException();
        }

        public override void Stop()
        {            
        }

        /// <summary>
        /// Represents the step view items.
        /// </summary>
        private ObservableCollection<StageBase> m_stepViewItems;

        /// <summary>
        /// Gets or sets the step view items.
        /// </summary>
        public ObservableCollection<StageBase> Stages
        {
            get
            {
                return m_stepViewItems;
            }
            set
            {
                m_stepViewItems = value;
                OnPropertyChanged();
            }
        }
    }
}
