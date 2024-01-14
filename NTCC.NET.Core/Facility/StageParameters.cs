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

            foreach (var xmlZone in xmlStage.Descendants("Zone"))
            {
                string zoneID = xmlZone.Attribute("ID")?.Value;
                HeatingParameters zoneHeatingParameters = HeatingParameters.FromXml(xmlZone);

                //add zone heating parameters
                parameters.StageHeatingParameters.Add(zoneID, zoneHeatingParameters);
            }

            return parameters;
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

    }
}
