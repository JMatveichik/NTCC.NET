using NTCC.NET.Core.Facility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace NTCC.NET.Core.Stages
{
    public class StageMain : StageBase
    {
        public StageMain(string id) : base(id)
        {

        }

        public List<StageBase> Stages
        {
            get;
            private set;
        } = new List<StageBase>();

        /// <summary>
        /// Действия при подготовке к стадии
        /// </summary>
        /// <returns></returns>
        public override StageResult Prepare()
        {
            throw new NotImplementedException();
            /*
            try
            {
                if (!Directory.Exists(configDir))
                    throw new IOException($"Не найдена директория для конфигурирования установки <{configDir}>");

                string xmlStagesPath = Path.Combine(configDir, "Stages.v2.xml");
                XDocument xmlDocument = XDocument.Load(xmlStagesPath);
                XElement xmlRoot = xmlDocument.Root;

                XElement xmlStage = xmlRoot.XPathSelectElement($"descendant::Stage[@ID='{ID}']");

                if (xmlStage == null)
                {
                    throw new IOException($"Не найдена конфигурация для стадии <{ID}>");
                }


                foreach (var xmlZone in xmlStage.Descendants("Zone"))
                {
                    string zoneID = xmlZone.Attribute("ID")?.Value;

                    var zone = ArtMonbatFacility.ReactorHeaters[zoneID];

                    double minWallTemperature   = parseDoubleAttribute(xmlZone, "MinWallTemperature");
                    double maxWallTemperature   = parseDoubleAttribute(xmlZone, "MaxWallTemperature");
                    double heaterPower         = parseDoubleAttribute(xmlZone, "HeaterPower");
                    double maxHeaterTemperature = parseDoubleAttribute(xmlZone, "MaxHeaterTemperature");

                    zone.SetupControl(  minWallTemperature,
                                        maxWallTemperature,
                                        heaterPower,
                                        maxHeaterTemperature);
                }

            }
            catch (Exception ex)
            {

                return StageResult.Failed;
            }

            return StageResult.Successful;
            */
        }

        protected override StageResult Finalization()
        {
            throw new NotImplementedException();
        }

        protected override StageResult Main(CancellationToken cancel)
        {
            throw new NotImplementedException();
        }
    }
}
