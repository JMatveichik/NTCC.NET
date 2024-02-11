
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NTCC.NET.Core.Facility
{
  public class DiscreteDataPoint : DataPoint
  {
    /// <summary>
    /// Класс дискретного датчика
    /// </summary>
    /// <param name="name"></param>
    public DiscreteDataPoint(string name) : base(name)
    {
      Group = "Дискретные входы";
      StateStringsMap = stateDefaultStringsMap;
      StateString = stateStringsMap[false];
    }

    /// <summary>
    /// Устройство к которому подключен дискретный датчик
    /// </summary>
    public override AcquisitionDeviceBase Device
    {
      get => device;
      set
      {
        if (device == value)
          return;

        device = value;

        if (Device != null)
          Device.DiscreteInputsUpdate += DiscreteInputsUpdate;

        OnPropertyChanged();
      }
    }

    /// <summary>
    /// Обработчик события обновления состояния дискретных входов устройства
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DiscreteInputsUpdate(object sender, DiscreteDataEventArgs e)
    {
      try
      {
        if (sender == Device)
        {
          State = e.Data[ListenedChannel];
        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine($" DiscreteInputsUpdate : {ex.Message} for {ID} at channel {ListenedChannel} data size {e.Data.Count}");
      }
    }

    /// <summary>
    /// Текущее состояние датчика
    /// </summary>
    public bool State
    {
      get => state;
      protected set
      {
        if (state == value)
          return;

        state = value;
        OnPropertyChanged();

        StateString = stateStringsMap[state];
      }
    }

    private bool state = false;

    /// <summary>
    /// Текущее состояние датчика
    /// </summary>
    public string StateString
    {
      get => stringState;
      private set
      {
        if (stringState == value)
          return;

        stringState = value;
        OnPropertyChanged();
      }
    }

    private string stringState = "";

    /// <summary>
    /// Строковое представление состояния
    /// </summary>
    public Dictionary<bool, string> StateStringsMap
    {
      get => stateStringsMap;
      set
      {
        if (stateStringsMap == value)
          return;

        stateStringsMap = value;

        if (value == null)
          stateStringsMap = stateDefaultStringsMap;

        OnPropertyChanged();
      }
    }

    private Dictionary<bool, string> stateStringsMap = new Dictionary<bool, string>();

    private static Dictionary<bool, string> stateDefaultStringsMap = new Dictionary<bool, string>()
    {
        { false, "OFF"},
        { true , "ON" }
    };

    /// <summary>
    /// Получить строковое значение состояния
    /// </summary>
    /// <param name="state">Состояние дискретной точки данных</param>
    /// <returns>Строковое представление состояния</returns>
    public string GetStateString(bool state)
    {
      return StateStringsMap[state];
    }
  }
}