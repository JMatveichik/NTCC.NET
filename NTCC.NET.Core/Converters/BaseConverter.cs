using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace NTCC.NET.Core.Converters
{
    public abstract class BaseConverter
    {

        public BaseConverter()
        {
        }

        public char LiteralX { get; set; } = 'x';
        public char LiteralY { get; set; } = 'y';
         
        /// <summary>
        /// Прямое преобразование x -> y
        /// </summary>
        /// <param name="x">Входная величина для преобразования</param>
        /// <returns>Сконвертированная величина</returns>
        public abstract double Convert(double x);

        /// <summary>
        /// Обратное преобразование y -> x
        /// </summary>
        /// <param name="y">Входная величина для обратного преобразования</param>
        /// <returns>Сконвертированная в обратном направлении величина</returns>
        public abstract double ConvertBack(double y);

    }
}
