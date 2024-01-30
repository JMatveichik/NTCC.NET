using NTCC.NET.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NTCC.NET.Core.Facility
{
  public class StageParameters
  {
    private StageParameters()
    {
    }

    public static StageParameters FromXml(XElement xmlStage)
    {
      StageParameters parameters = new StageParameters();
      parameters.StageHeatingParameters = new Dictionary<string, HeatingParameters>();

      parameters.Duration = XmlHelper.ParseDoubleAttribute(xmlStage, "Duration", 0.0);
      parameters.FlowRate = XmlHelper.ParseDoubleAttribute(xmlStage, "FlowRate", 0.0);
      parameters.PurgePropaneLine = XmlHelper.ParseBoolAttribute(xmlStage, "PurgePropaneLine", false);
      parameters.AverageTemperature = XmlHelper.ParseDoubleAttribute(xmlStage, "AverageTemperature", 0.0);

      parameters.PassCount = (int)XmlHelper.ParseDoubleAttribute(xmlStage, "PassCount", 15.0);
      parameters.CoolingTime = XmlHelper.ParseDoubleAttribute(xmlStage, "CoolingTime", 10.0);
      parameters.OneWayTimeout = XmlHelper.ParseDoubleAttribute(xmlStage, "OneWayTimeout", 5.0);


      foreach (var xmlZone in xmlStage.Descendants("Zone"))
      {
        string zoneID = xmlZone.Attribute("ID")?.Value;
        HeatingParameters zoneHeatingParameters = HeatingParameters.FromXml(xmlZone);

        //add zone heating parameters
        parameters.StageHeatingParameters.Add(zoneID, zoneHeatingParameters);
      }

      return parameters;
    }

    public int PassCount 
    { 
      get; 
      private set; 
    }

    
    public double CoolingTime 
    { 
      get; 
      private set; 
    }

		public double OneWayTimeout 
    {  
      get; 
      private set; 
    }

    public Dictionary<string, HeatingParameters> StageHeatingParameters
    {
      get;
      private set;
    }

    public double FlowRate
    {
      get;
      private set;
    }

    public double Duration
    {
      get;
      private set;
    }

    public bool PurgePropaneLine
    {
      get;
      private set;
    }
    public double AverageTemperature
    {
      get;
      private set;
    }
  }
}
