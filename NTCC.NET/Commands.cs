﻿using System.Windows.Input;

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
    /// Пропустить текущую стадию
    /// </summary>
    public static RoutedUICommand SkipCurrentStage
    {
      get;
    } = new RoutedUICommand("Skip current stage", "SkipCurrentStage", typeof(FacilityCommands), null);


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

    /// <summary>
    /// Команда задания параметоров зоны реактора
    /// </summary>
    public static RoutedUICommand HeatingZoneParameters
    {
      get;
    } = new RoutedUICommand("Setup heating zone parameter", "HeatingZoneParameters", typeof(FacilityCommands), null);


    /// <summary>
    /// Команда задания параметоров зоны реактора
    /// </summary>
    public static RoutedUICommand StageParameters
    {
      get;
    } = new RoutedUICommand("Setup stage parameters", "StageParameters", typeof(FacilityCommands), null);

    /// <summary>
    /// Команда задания параметоров подогревателя газа
    /// </summary>
    public static RoutedUICommand GasHeaterParameters
    {
      get;
    } = new RoutedUICommand("Setup gas heater parameters", "GasHeaterParameters", typeof(FacilityCommands), null);

    /// <summary>
    /// Команда включения\выключения питания зоны
    /// </summary>
    public static RoutedUICommand SwitchHeatingZonePower
    {
      get;
    } = new RoutedUICommand("Switch heating zone power", "SwitchHeatingZonePower", typeof(FacilityCommands), null);

    /// <summary>
    /// Команда включения\выключения контроля автоматического контроля температуры зоны
    /// </summary>
    public static RoutedUICommand SwitchHeatingZoneControl
    {
      get;
    } = new RoutedUICommand("Switch heating zone control state", "SwitchHeatingZoneControl", typeof(FacilityCommands), null);

    /// <summary>
    /// Команда очиски непрочитанных сообщений
    /// </summary>
    public static RoutedUICommand MarkAllMessagesAsReaded
    {
      get;
    } = new RoutedUICommand("Mark all messages as readed", "MarkAllMessagesAsReaded", typeof(FacilityCommands), null);

    /// <summary>
    /// Команда очистки списка сообщений
    /// </summary>
    public static RoutedUICommand ClearMessageList
    {
      get;
    } = new RoutedUICommand("Remove messages list", "ClearMessageList", typeof(FacilityCommands), null);

    /// <summary>
    /// Команда включения\выключения управление заслонкой
    /// </summary>
    public static RoutedUICommand SwitchDamperState
    {
      get;
    } = new RoutedUICommand("On/off damper", "SwitchDamperState", typeof(FacilityCommands), null);

  }
}
