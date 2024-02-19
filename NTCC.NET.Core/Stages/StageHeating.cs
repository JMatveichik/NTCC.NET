using NTCC.NET.Core.Facility;
using NTCC.NET.Core.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NTCC.NET.Core.Stages
{
  public class StageHeating : StageBase
  {

    public StageHeating(string id) : base(id)
    {
      
    }

    public override StageResult Prepare()
    {
      OnTick($"Подготовка стадии  {Title}.", MessageType.Warning);

      //получаем среднюю температуру стенок реактора
      AverageTemperature = getAverageTemperature();

      //задание параметров прогрева
      SetupHeating();

      //если задан расход для стадии прогрева задаем проток воздуха
      //на стадии прогрева возможен только проток воздуха!!!
      if (StageParameters.FlowRate > 0.0)
      {
        //открыть клапан YA5 подачи воздуха в камеру синтеза
        DataPointHelper.SetDiscreteParameter(this, "YA05.OPN", true, (int)OperationDelay.TotalMilliseconds);

        //открыть клапан YA14 подачи азот/воздуха в камеру синтеза
        DataPointHelper.SetDiscreteParameter(this, "YA14.OPN", true, (int)OperationDelay.TotalMilliseconds);

        //задать расход воздуха в камеру синтеза
        DataPointHelper.SetAnalogParameter(this, "MD400C.SETPOINT.WR", StageParameters.FlowRate, (int)OperationDelay.TotalMilliseconds);

        //ожидаем установление расхода воздуха 5 секунд
        DataPointHelper.WaitAnalogParameterSet(this, "MD400C.MEASSURE", StageParameters.FlowRate, TimeSpan.FromSeconds(5.0));
      }

      return StageResult.Successful;
    }

    protected override StageResult Finalization()
    {
      OnTick($"Завершение стадии  {Title}.", MessageType.Warning);

      //если был задан расход для стадии прогрева задаем проток воздуха 0.0
      if (StageParameters.FlowRate > 0.0)
      {
        //задать расход воздуха в камеру синтеза
        DataPointHelper.SetAnalogParameter(this, "MD400C.SETPOINT.WR", ZERRO, (int)OperationDelay.TotalMilliseconds);

        //ожидаем установление расхода воздуха 5 секунд
        DataPointHelper.WaitAnalogParameterSet(this, "MD400C.MEASSURE", ZERRO, TimeSpan.FromSeconds(5.0));

        //закрыть клапан YA14 подачи азот/воздуха в камеру синтеза
        DataPointHelper.SetDiscreteParameter(this, "YA14.OPN", false, (int)OperationDelay.TotalMilliseconds);

        //звкрыть клапан YA5 подачи воздуха в камеру синтеза
        DataPointHelper.SetDiscreteParameter(this, "YA05.OPN", false, (int)OperationDelay.TotalMilliseconds);
      }

      return StageResult.Successful;
    }

    /// <summary>
    /// Средняя температура стенок реактора с учетом заданных зон
    /// </summary>
    public double AverageTemperature
    {
      get => averageTemperature;
      private set
      {
        if (value == averageTemperature)
          return;

        averageTemperature = value;
        OnPropertyChanged();
      }
    }
    private double averageTemperature = 0.0;


    /// <summary>
    /// Получаем среднюю температуру стенок реактора по заданным для стадии зонам  
    /// </summary>
    /// <returns> Средняя температура по заданным зонам</returns>
    private double getAverageTemperature()
    {

      //выбираем зоны по которым вычисляется средняя температура
      var activeZonesIDs = StageParameters.StageHeatingParameters.Where(z => z.Value.UseWhenAverageTemperatureCalc == true).Select(z => z.Key).ToList();
      
      //вычисляем среднюю температуру по заданным зонам
      return ArtMonbatFacility.GetAverageTemperature(activeZonesIDs);
    }

#if DEBUG
    public void traceHeatingParameters()
    {
      StringBuilder sb1 = new StringBuilder();
      sb1.AppendLine(new string('-', 30));

      foreach (var zone in StageParameters.StageHeatingParameters)
      {
        sb1.AppendLine($"Зона: {zone.Key}  Use in calc : {zone.Value.UseWhenAverageTemperatureCalc}");
      }
      Debug.WriteLine(sb1);
      return;

      StringBuilder sb = new StringBuilder();
      sb.AppendLine($"Параметры стадии {Title}:");
      sb.AppendLine(new string('-', 20));
      sb.AppendLine($"Продолжительность: {StageParameters.Duration}");
      sb.AppendLine($"Расход воздуха: {StageParameters.FlowRate}");
      sb.AppendLine($"Промывка линии пропана: {StageParameters.PurgePropaneLine}");
      sb.AppendLine($"Средняя температура: {StageParameters.AverageTemperature}");
      
      foreach (var zone in StageParameters.StageHeatingParameters)
      {
        sb.AppendLine($"Зона: {zone.Key:20}  Use in calc : {zone.Value.UseWhenAverageTemperatureCalc}");
        sb.AppendLine(new string('-', 20));
        sb.AppendLine($"Минимальная температура стенок: {zone.Value.MinWallTemperature}");
        sb.AppendLine($"Максимальная температура стенок: {zone.Value.MaxWallTemperature}");
        sb.AppendLine($"Мощность нагревателя: {zone.Value.HeaterPower}");
        sb.AppendLine($"Максимальная температура нагревателя: {zone.Value.MaxHeaterTemperature}");
        sb.AppendLine($"Использовать при вычислении средней температуры: {zone.Value.UseWhenAverageTemperatureCalc}");
      }

      Debug.Write(sb);

    } 
#endif
    

    /// <summary>
    /// Выполнение стадии прогрева
    /// </summary>
    /// <param name="stop">Остановить садию (технологический цикл)</param>
    /// <param name="skip">Пропустить сдаию (продолжить технологический цикл)</param>
    /// <returns></returns>
    protected override StageResult Main(CancellationToken stop, CancellationToken skip)
    {
      OnTick($"Начато выполнение стадии {Title}.", MessageType.Warning);
      StartTime = DateTime.Now;

      //ожидаем пока средняя температура стенок реактора по заданным зонам
      //превысит заданную в параметрах стадии прогрева
      while (AverageTemperature < StageParameters.AverageTemperature)
      {
        Thread.Sleep((int)ThreadDelay.TotalMilliseconds);

        //проверяем на остановку стадии пользователем
        if (stop.IsCancellationRequested)
          return StageResult.Stopped;

        //проверяем на пропуск стадии пользователем
        if (skip.IsCancellationRequested)
          return StageResult.Skipped;

        //получаем среднюю температуру стенок реактора
        AverageTemperature = getAverageTemperature();

        //обновляем продолжительность выполнения стадии
        Duration = DateTime.Now - StartTime;
      }

      return StageResult.Successful;
    }
  }
}
