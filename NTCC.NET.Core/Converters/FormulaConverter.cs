using org.mariuszgromada.math.mxparser;

namespace NTCC.NET.Core.Converters
{
    /// <summary>
    /// Класс вторичного преобразователя по формуле заданной строкой
    /// </summary>
    public class FormulaConverter : BaseConverter
    {
        public FormulaConverter(string formula)
        {
            Formula = new Function(formula);
        }

        private Function Formula = null;


        /// <summary>
        /// Прямое преобразование x -> y
        /// </summary>
        /// <param name="x">Входная величина для преобразования</param>
        /// <returns>Сконвертированная величина</returns>
        public override double Convert(double x)
        {
            Argument v = new Argument("x", x);
            double y = (double)Formula.calculate(v);

            return y;
        }

        /// <summary>
        /// Обратное преобразование y -> x
        /// </summary>
        /// <param name="y">Входная величина для обратного преобразования</param>
        /// <returns>Сконвертированная в обратном направлении величина</returns>
        public override double ConvertBack(double y)
        {
            throw new System.NotSupportedException();
        }
    }
}
