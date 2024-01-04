using System.Windows.Input;

namespace NTCC.NET.Commands
{
    public class FacilityCommands
    {
        /// <summary>
        /// Команда выполнения начальной подготовки установки
        /// </summary>
        public static RoutedUICommand Init
        {
            get;
        } = new RoutedUICommand("Initialize facility", "Initialize", typeof(FacilityCommands), null);


        /// <summary>
        /// Команда выполнения стадии предварительного нагрева
        /// </summary>
        public static RoutedUICommand PreheatStart
        {
            get;
        } = new RoutedUICommand("Preheating reactor walls for oxidize temperature", "PreheatStart", typeof(FacilityCommands), null);

    }
}
