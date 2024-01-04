using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCC.NET.Core.Facility
{
    public class ReactorHeatingElement : FacilityElement
    {
        public static List<string> PropertiesList = new List<string>()
        {
            "Temperature", "Current"
        };

        public ReactorHeatingElement(string id, double nominalPower) : base(id)
        {
            NominalPower = nominalPower;
        }

        /// <summary>
        /// Связывание точек данных со свойствами объекта
        /// </summary>
        /// <param name="dataPoints"></param>
        public void Setup(Dictionary<string, DataPoint> dataPoints)
        {
            Temperature = (AnalogDataPoint)dataPoints["Temperature"];
            Current     = (AnalogDataPoint)dataPoints["Current"];
        }

        /// <summary>
        /// Номинальная  мощность нагревательного элемента, кВт
        /// </summary>
        public double NominalPower
        {
            get;
            private set;
        }

        /// <summary>
        ////Ток нагревательного элемента
        /// </summary>
        public AnalogDataPoint Current
        {
            get;
            private set;
        }

        /// <summary>
        ////Температура нагревательного элемента
        /// </summary>
        public AnalogDataPoint Temperature
        {
            get;
            private set;
        }
    }
}
