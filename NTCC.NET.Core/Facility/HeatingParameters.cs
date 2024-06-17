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
      string zoneID = xmlZone.Attribute("ID")?.Value;
      
      heatingParameters.Zone = ArtMonbatFacility.ReactorZones[zoneID];

      double minWallTemperature = XmlHelper.ParseDoubleAttribute(xmlZone, "MinWallTemperature");
      double maxWallTemperature = XmlHelper.ParseDoubleAttribute(xmlZone, "MaxWallTemperature");
      double heaterPower = XmlHelper.ParseDoubleAttribute(xmlZone, "HeaterPower");
      double maxHeaterTemperature = XmlHelper.ParseDoubleAttribute(xmlZone, "MaxHeaterTemperature");
      bool useWhenAverageTemperatureCalc = XmlHelper.ParseBoolAttribute(xmlZone, "UseInAverageCalc", true);

      heatingParameters.MinWallTemperature = minWallTemperature;
      heatingParameters.MaxWallTemperature = maxWallTemperature;
      heatingParameters.HeaterPower = heaterPower;
      heatingParameters.MaxHeaterTemperature = maxHeaterTemperature;
      heatingParameters.UseWhenAverageTemperatureCalc = useWhenAverageTemperatureCalc;

      return heatingParameters;
    }

    public HeatingParameters()
    {

    }

    public ReactorHeatingZone Zone
    {
      get; 
      private set;
    }

    public bool UseWhenAverageTemperatureCalc
    {
      get; set;
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
