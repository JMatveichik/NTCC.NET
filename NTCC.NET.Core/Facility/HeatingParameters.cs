using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NTCC.NET.Core.Tools;

namespace NTCC.NET.Core.Facility
{
    public class HeatingParameters
    {
        public static HeatingParameters FromXml(XElement xmlZone)
        {
            HeatingParameters heatingParameters = new HeatingParameters();

            double minWallTemperature   = XmlHelper.ParseDoubleAttribute(xmlZone, "MinWallTemperature");
            double maxWallTemperature   = XmlHelper.ParseDoubleAttribute(xmlZone, "MaxWallTemperature");
            double heaterPower          = XmlHelper.ParseDoubleAttribute(xmlZone, "HeaterPower");
            double maxHeaterTemperature = XmlHelper.ParseDoubleAttribute(xmlZone, "MaxHeaterTemperature");

            heatingParameters.MinWallTemperature = minWallTemperature;
            heatingParameters.MaxWallTemperature = maxWallTemperature;
            heatingParameters.HeaterPower = heaterPower;
            heatingParameters.MaxHeaterTemperature = maxHeaterTemperature;

            return heatingParameters;
        }

        public HeatingParameters()
        {

        }

        public double MinWallTemperature
        {
            get; set;
        }

        public double MaxWallTemperature
        {
            get; set;
        }

        public double HeaterPower
        {
            get; set;
        }

        public double MaxHeaterTemperature
        {
            get; set;
        }
    }
}
