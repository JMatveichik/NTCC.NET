using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NTCC.NET.Core.Tools
{
    public static class XmlHelper
    {
        public static double ParseDoubleAttribute(XElement xmlElem, string strAttribute, double? defaultValue = null)
        {
            string strValue = xmlElem.Attribute(strAttribute)?.Value;            

            if (string.IsNullOrEmpty(strValue))
            {
                if (defaultValue == null)
                    throw new IOException($"Не задан  параметр '{strAttribute}' для элемента <{xmlElem.Name}>");
                else
                    return defaultValue.Value;
            }

            double doubleValue = 0.0;
            if (!double.TryParse(strValue, out doubleValue))
                throw new IOException($"Ошибка задания параметра {strAttribute} = '{strValue}' для элемента <{xmlElem.Name}>");

            return doubleValue;
        }

        public static bool ParseBoolAttribute(XElement xmlElem, string strAttribute, bool? defaultValue = null)
        {
            string strValue = xmlElem.Attribute(strAttribute)?.Value;

            if (string.IsNullOrEmpty(strValue))
            {
                if (defaultValue == null)
                    throw new IOException($"Не задан  параметр '{strAttribute}' для элемента <{xmlElem.Name}>");
                else
                    return defaultValue.Value;
            }

            bool boolValue = false;
            if (!bool.TryParse(strValue, out boolValue))
                throw new IOException($"Ошибка задания параметра {strAttribute} = '{strValue}' для элемента <{xmlElem.Name}>");

            return boolValue;
        }
    }
}
