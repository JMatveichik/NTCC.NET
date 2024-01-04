using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace NTCC.NET.Core.Converters
{
    /// <summary>
    /// Класс вторичного преобразователя по линейному закону. 
    /// </summary>
    public class LinearConverter : BaseConverter
    {

        public LinearConverter(double x1, double y1, double x2, double y2, string name, string description)
        {
            K = (y2 - y1) / (x2 - x1);
            B = -K * x1 + y1;
            LiteralX = 'x';
            LiteralY = 'y';
        }

        public LinearConverter(double k, double b, string name, string description)
        {
            K = k;
            B = b;
            LiteralX = 'x';
            LiteralY = 'y';
        }

        /// <summary>
        /// Прямое преобразование x -> y
        /// </summary>
        /// <param name="x">Входная величина для преобразования</param>
        /// <returns>Сконвертированная величина</returns>
        public override double Convert(double x)
        {
            return K * x + B;
        }

        /// <summary>
        /// Обратное преобразование y -> x
        /// </summary>
        /// <param name="y">Входная величина для обратного преобразования</param>
        /// <returns>Сконвертированная в обратном направлении величина</returns>
        public override double ConvertBack(double y)
        {
            return (y - B) / K;
        }


        /// <summary>
        /// Cтроковое представление формуулы прямой y = k*x + b
        /// </summary>
        /// <returns>Строку формулы прямой</returns>

        public override string ToString()
        {
            return string.Format("{0} = {1}{2} {3} {4}", LiteralY, K, LiteralX, B < 0 ? "":"+", B);
        }


        /// <summary>
        /// Коэффициент наклона прямой y = k*x + b
        /// </summary>
        public double K
        {
            get;
            set;            
        }       


        /// <summary>
        /// Смещение прямой относительно нулевой точки y = k*x + b
        /// </summary>
        public double B
        {
            get;
            set;
        }
    }
}
