using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NTCC.NET.Core.Stages;

namespace NTCC.NET.Core.Facility
{
    public class StageFactory
    {
        public static StageBase CreateStage(XElement stageElement)
        {
            string type = stageElement.Attribute("Type")?.Value;
            string id   = stageElement.Attribute("ID")?.Value;
            string name = stageElement.Attribute("Name")?.Value;
            string desc = stageElement.Attribute("Description")?.Value;
            
            if (string.IsNullOrEmpty(type))
                throw new ArgumentNullException($"Type not set for stage : {id}");

            StageBase stage = null;
            switch (type.ToUpper())
            {
                case "HEATING":
                    {
                        stage = new StageHeating(id);
                    }
                break;

                case "AIR":
                    {
                        stage = new StageAir(id);
                    }
                    break;
                case "NITRO":
                    {
                        stage = new StageNitro(id);
                    }
                    break;
                case "PROPANE":
                    {
                        stage = new StagePropane(id);
                    }
                    break;
                case "CLEAN":
                    {
                        stage = new StageClean(id);
                    }
                    break;


                default:
                    throw new ArgumentException($"Unsupported stage type: {type}");
            }

            try
            {
                stage.StageParameters = StageParameters.FromXml(stageElement);
            }
            catch(Exception ex)
            {

            }

            stage.TotalDuration = TimeSpan.FromMinutes(stage.StageParameters.Duration);
            stage.Description = desc;
            stage.Title = name;

            return stage;
        }
    }
}
