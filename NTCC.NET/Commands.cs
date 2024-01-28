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

    /// <summary>
    /// Команда задания установления аналоговой величины
    /// </summary>
    public static RoutedUICommand SetAnalogOutputValue
    {
      get;
    } = new RoutedUICommand("Set analog output data point value", "SetAnalogOutputValue", typeof(FacilityCommands), null);

    /// <summary>
    /// Команда задания установления дискретной величины
    /// </summary>
    public static RoutedUICommand SwitchDiscreteOutputValue
    {
      get;
    } = new RoutedUICommand("Switch discrete output value", "SwitchDiscreteOutputValue", typeof(FacilityCommands), null);


  }
}
