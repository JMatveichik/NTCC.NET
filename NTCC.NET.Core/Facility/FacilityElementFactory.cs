using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NTCC.NET.Core.Facility
{
    public class FacilityElementFactory
    {

        private static Dictionary<string, DataPoint> extractDataPoints (XElement heatingGroupElement, IEnumerable<string> attributes)
        {
            string id = heatingGroupElement.Attribute("ID")?.Value;

            Dictionary<string, DataPoint> dataPoints = new Dictionary<string, DataPoint>();
            
            foreach (var attr in attributes)
            {
                string dataPointID = heatingGroupElement.Attribute(attr)?.Value;
                if (string.IsNullOrEmpty(dataPointID))
                    throw new ArgumentException($"Invalid attribute [{attr}] for heating group [{id}].");

                DataPoint dp = ArtMonbatFacility.DataPoints[dataPointID];
                if (dp == null)
                    throw new ArgumentException($"Data point [{dataPointID}] not found in data set.");

                dataPoints.Add(attr, dp);
            }

            return dataPoints;
        }

        public static ReactorHeatingElement CreateHeatingElement(XElement heatingElement)
        {
            string id = heatingElement.Attribute("ID")?.Value;
            string desc = heatingElement.Attribute("Description")?.Value;
            string power = heatingElement.Attribute("NominalPower")?.Value;

            double nominalPower;
            double.TryParse(power, out nominalPower);

            ReactorHeatingElement element = new ReactorHeatingElement(id, nominalPower)
            {
                Description = desc
            };

            Dictionary<string, DataPoint> dataPoints = extractDataPoints(heatingElement, ReactorHeatingElement.PropertiesList);
            element.Setup(dataPoints);

            return element;
        }

        public static ReactorHeatingZone CreateHeatingZone(XElement xmlHeatingZoneElement)
        {
            string id   = xmlHeatingZoneElement.Attribute("ID")?.Value;
            string desc = xmlHeatingZoneElement.Attribute("Description")?.Value;

            ReactorHeatingZone zone = new ReactorHeatingZone(id)
            {
                Description = desc,
            };

            Dictionary<string, DataPoint> dataPoints = extractDataPoints(xmlHeatingZoneElement, ReactorHeatingZone.PropertiesList);
            zone.Setup(dataPoints);

            foreach (var element in xmlHeatingZoneElement.Descendants("HeatingElement"))
            {
                // Теперь у вас есть объект sensor соответствующего типа
                ReactorHeatingElement heatingElement = FacilityElementFactory.CreateHeatingElement(element) ;
                if (heatingElement == null)
                    throw new ArgumentNullException("Ошибка при создании объекта наргревательного элемента");

                zone.HeatingElements.Add(heatingElement);
            }

            return zone;
        }        
    }
}
