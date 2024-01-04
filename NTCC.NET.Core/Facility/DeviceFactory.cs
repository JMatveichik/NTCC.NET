using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NTCC.NET.Core.Facility
{
    public class DeviceFactory
    {
        public static AcquisitionDeviceBase CreateDevice(XElement deviceElement)
        {

            string type = deviceElement.Attribute("Type")?.Value;
            string id   = deviceElement.Attribute("ID")?.Value;
            string name = deviceElement.Attribute("Name")?.Value;
            string desc = deviceElement.Attribute("Description")?.Value;
            string connect = deviceElement.Attribute("Connection")?.Value;

            if (string.IsNullOrEmpty(connect))
                throw new ArgumentNullException($"Connection string not set for device : {id}");

            AcquisitionDeviceBase device = null;
            switch (type.ToUpper())
            {
                case "ART-MONBAT-01":

                    string registersMappingFile = deviceElement.Attribute("RegistersMappingFile")?.Value;
                    if (string.IsNullOrEmpty(registersMappingFile))
                        throw new ArgumentNullException($"Register mapping file not set for device : {id}");

                    string registersMappingFilePath = Path.Combine(ArtMonbatFacility.ConfigDirectory, registersMappingFile);
                    
                    device = new ArtMonbatDevice(id, registersMappingFilePath)
                    {
                        Title = name,
                        Description = desc,
                        ConnectionString = connect
                    };

                break;

                default:
                    throw new ArgumentException($"Unsupported device type: {type}");
            }

            return device;
        }
    }
}
