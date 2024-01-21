using System.Windows.Input;

namespace NTCC.NET.Commands
{
    public class FacilityCommands
    {
        /// <summary>
        /// Команда запуска технологического цикла
        /// </summary>
        public static RoutedUICommand StartFullCycle
        {
            get;
        } = new RoutedUICommand("Start process", "StartFullCycle", typeof(FacilityCommands), null);


        /// <summary>
        /// Команда остановки технологического цикла
        /// </summary>
        public static RoutedUICommand StopFullCycle
        {
            get;
        } = new RoutedUICommand("Stop process", "StopFullCycle", typeof(FacilityCommands), null);

    }
}
