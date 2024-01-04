using NTCC.NET.Core.Facility;
using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCC.NET.ViewModels
{
    internal class TablesViewModel : PageViewModel
    {
        public TablesViewModel()
        {
            Sensors = new ObservableCollection<DataPoint>();
            var dataPoints = ArtMonbatFacility.DataPoints.Items.Values.ToList();
            foreach (DataPoint dataPoint in dataPoints)
            {
                Sensors.Add(dataPoint);
            }
            SelectedDataPoint = dataPoints[0];
                
        }

        public ObservableCollection<DataPoint> Sensors
        {
            get;
            private set;
        }

        public DataPoint SelectedDataPoint 
        {
            get => selectedDataPoint;
            set {
                if (value == selectedDataPoint)
                    return;

                selectedDataPoint = value;
                OnPropertyChanged();
            }
        }

        DataPoint selectedDataPoint = null;
    }
}
