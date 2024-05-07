using NTCC.NET.Core.Stages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NTCC.NET.Core.Facility
{
  public class ArtMonbatFacility
  {
    private ArtMonbatFacility()
    {
    }

    public static ArtMonbatFacility Instance
    {
      get;
    } = new ArtMonbatFacility();

    #region DataPoints

    /// <summary>
    /// DataPoints set for facility
    /// </summary>
    public static FacilitySet<DataPoint> DataPoints
    {
      get;
    } = new FacilitySet<DataPoint>();

    /// <summary>
    /// Reactor heating zones set
    /// </summary>
    public static FacilitySet<ReactorHeatingZone> ReactorZones
    {
      get;
    } = new FacilitySet<ReactorHeatingZone>();

    /// <summary>
    /// Facility stages set
    /// </summary>
    public static FacilitySet<StageBase> Stages
    {
      get;
    } = new FacilitySet<StageBase>();

    /// <summary>
    /// Main stage contain logic to process all stages 
    /// </summary>
    public static StageMain FullCycle
    {
      get;
    } = new StageMain("STG.MAIN");

    /// <summary>
    /// Devices set
    /// </summary>
    public static FacilitySet<AcquisitionDeviceBase> Devices
    {
      get;
    } = new FacilitySet<AcquisitionDeviceBase>();

    /// <summary>
    /// Управление заслонкой
    /// </summary>
    public static PeriodicalSwitcher Damper
    {
      get;
      private set;
    } = null;

    /// <summary>
    /// Подтверждение связи со шкафом управления
    /// </summary>
    public static PeriodicalSwitcher Ping
    {
      get;
      private set;
    } = null;

    /// <summary>
    /// Управление подогревателем газа
    /// </summary>
    public static GasHeater GasHeater
    {
      get;
      private set;
    } = null;

    /// <summary>
    /// Управление скребком
    /// </summary>
    public static Scrapper Scrapper
    {
      get;
      private set;
    } = null;

    /// <summary>
    /// Предоставление информации о средней температуре стенок реактора
    /// </summary>
    public static ReactorAverageTemperature AverageTemperatureProvider
    {
      get;
      private set;
    } = null; 

    #endregion

    /// <summary>
    /// Директория конфигурационных файлов
    /// </summary>
    public static string ConfigDirectory
    {
      get;
      private set;
    }

    /// <summary>
    /// Получение средней температуры стенок реактора
    /// </summary>
    /// <param name="zonesNames">Список зон реактора, по которым вычисляется среднее значение</param>
    /// <returns>Средняя температура по заданным зонам реактора</returns>
    public double GetAverageTemperature(IEnumerable<string> zonesNames = null)
    {
      List<ReactorHeatingZone> zones = null;
      List<ReactorHeatingZone> allZones = ReactorZones.Items.Values.ToList();

      //если зоны реактора не заданы, то берем все зоны
      if (zonesNames == null)
      {
        zones = allZones;
      }
      //иначе берем только заданные зоны
      else
      {
        zones = allZones.Where(z => zonesNames.Contains(z.ID)).ToList();
      }

      double averageTemperature = zones.Average(zone => zone.WallTemperature.Value);
      return averageTemperature;
    }


    public void Initialize(string configDir)
    {
      if (!Directory.Exists(configDir))
        throw new IOException($"Не найдена директория для конфигурирования установки <{configDir}>");

      ConfigDirectory = configDir;

      try
      {
        //инициализация устройств управления/измерения
        string xmlDevicesPath = Path.Combine(configDir, "Devices.v1.xml");
        string xsdDevicesPath = Path.Combine(configDir, "Devices.v1.xsd");
        initializeDevices(xmlDevicesPath, xsdDevicesPath);

        //инициализация точек данных
        string xmlDataPointPath = Path.Combine(configDir, "DataPoints.v5.xml");
        string xsdDataPointPath = Path.Combine(configDir, "DataPoints.v5.xsd");
        initializeDataPoints(xmlDataPointPath, xsdDataPointPath);

        //инициализация нагревателей
        string xmlHeatingPath = Path.Combine(configDir, "HeatingGroups.v2.xml");
        string xsdHeatingPath = Path.Combine(configDir, "HeatingGroups.v2.xsd");
        initializeHeaters(xmlHeatingPath, xsdHeatingPath);

        //инициализация компонентов установки
        string xmlFacilityComponent = Path.Combine(configDir, "FacilityComponents.v2.xml");
        string xsdFacilityComponent = Path.Combine(configDir, "FacilityComponents.v2.xsd");
        initializeElements(xmlFacilityComponent, xsdFacilityComponent);

        //инициализация стадий выполнения техпроцесса
        string xmlStages = Path.Combine(configDir, "Stages.v2.xml");
        string xsdStages = Path.Combine(configDir, "Stages.v2.xsd");
        initializeStages(xmlStages, xsdStages);
      }
      catch (Exception ex)
      {
        //обработка ошибки инициализации системы
        throw ex;
      }
    }

    /// <summary>
    /// Инициализация устройств измерения и контроля  
    /// </summary>
    /// <param name="xmlConfigPath">Путь к конфигурационному файлу XML</param>
    /// <param name="xsdSchemaPath">Путь к схеме XSD</param>
    private void initializeDevices(string xmlConfigPath, string xsdSchemaPath = "")
    {
      XDocument xmlDocument = XDocument.Load(xmlConfigPath);

      foreach (var xmlElement in xmlDocument.Descendants("Device"))
      {
        // Теперь у вас есть объект sensor соответствующего типа
        AcquisitionDeviceBase device = DeviceFactory.CreateDevice(xmlElement);
        if (!Devices.Add(device))
        {
          AcquisitionDeviceBase duplicate = Devices[device.ID];
          throw new ArgumentException($"Обнаружен дубликат для устройств контроля и измерения : {duplicate.ID}-{duplicate.Title}");
        }
      }

      foreach (AcquisitionDeviceBase device in Devices.Items.Values)
      {
        if (!device.Connect())
          throw new Exception($"Ошибка соединение с устройством [{device.Title}] адрес {device.ConnectionString}");
      }
    }

    /// <summary>
    /// Загружаем точки данных из конфигурации XML
    /// </summary>
    private void initializeDataPoints(string xmlConfigPath, string xsdSchemaPath = "")
    {

      XDocument xmlDocument = XDocument.Load(xmlConfigPath);

      foreach (var xmlElement in xmlDocument.Descendants("DataPoint"))
      {
        // Теперь у вас есть объект sensor соответствующего типа
        DataPoint dataPoint = DataPointFactory.CreateDataPoint(xmlElement);
        if (!DataPoints.Add(dataPoint))
        {
          DataPoint duplicate = DataPoints[dataPoint.ID];
          throw new ArgumentException($"Обнаружен дубликат для точки данных : {duplicate.ID}-{duplicate.Title}");
        }
      }

    }

    /// <summary>
    /// Инициализация зон нагрева реактора
    /// </summary>
    /// <param name="xmlConfigPath">Путь к конфигурационному файлу XML</param>
    /// <param name="xsdSchemaPath">Путь к схеме XSD</param>
    private void initializeHeaters(string xmlConfigPath, string xsdSchemaPath = "")
    {
      XDocument xmlDocument = XDocument.Load(xmlConfigPath);

      foreach (var xmlElement in xmlDocument.Descendants("ReactorZone"))
      {
        ReactorHeatingZone heatingZone = FacilityElementFactory.CreateHeatingZone(xmlElement);
        if (!ReactorZones.Add(heatingZone))
        {
          ReactorHeatingZone duplicate = ReactorZones[heatingZone.ID];
          throw new ArgumentException($"Обнаружен дубликат для зоны нагрева: {duplicate.ID}-{duplicate.Title}");
        }
      }
    }

    /// <summary>
    /// Инициализация стадий автоматического управления  технологическим процессом 
    /// </summary>
    /// <param name="xmlConfigPath">Путь к конфигурационному файлу XML</param>
    /// <param name="xsdSchemaPath">Путь к схеме XSD</param>
    private void initializeStages(string xmlConfigPath, string xsdSchemaPath = "")
    {
      XDocument xmlDocument = XDocument.Load(xmlConfigPath);

      foreach (var xmlElement in xmlDocument.Descendants("Stage"))
      {
        // Теперь у вас есть объект стадии соответствующего типа
        StageBase stage = StageFactory.CreateStage(xmlElement);

        if (!Stages.Add(stage))
        {
          StageBase duplicate = Stages[stage.ID];
          throw new ArgumentException($"Обнаружен дубликат для стадии : {duplicate.ID}-{duplicate.Title}");
        }
        //добавляем стадию к основному циклу
        FullCycle.Stages.Add(stage);
      }

    }

    /// <summary>
    /// Инициализация элементов управления
    /// </summary>
    /// <param name="xmlConfigPath">Путь к конфигурационному файлу XML</param>
    /// <param name="xsdSchemaPath">Путь к схеме XSD</param>
    private void initializeElements(string xmlConfigPath, string xsdSchemaPath = "")
    {
      //TODO: Move setup elements to XML

      //инициализация подержания связи со шкафом управления
      Ping = new PeriodicalSwitcher("ELEM.PING");
      Ping.SetupControl("UNIT.PING", TimeSpan.FromSeconds(3.0), TimeSpan.FromSeconds(0.0));
      Ping.StartControl();

      Scrapper = new Scrapper("ELEM.SCRAPPER");
      Scrapper.SetupControl("YA01.2", "YA01.1", "CS02", "CS01", "YA02", "YA08.OPN");
  
      //инициализация управления заслонкой
      Damper = new PeriodicalSwitcher("ELEM.DAMPER");
      Damper.SetupControl("YA03", TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(1000));

      //инициализация управления подогревателем газа
      GasHeater = new GasHeater("ELEM.GAS.HEATER");
      GasHeater.SetupControl("TE20", "TE21", "EK08.RUN");

      //инициализация получения средней температуры стенок реактора
      AverageTemperatureProvider = new ReactorAverageTemperature("ELEM.AVERAGE.TEMPERATURE");
      AverageTemperatureProvider.StartControl();
    }


    public void Stop()
    {
      //останавливаем текущую стадию 
      FullCycle.CurrentStage?.Stop();

      //останавливаем контроль нагревателей
      if (ReactorZones != null)
      {
        foreach (var zone in ReactorZones.Items.Values)
        {
          zone?.StopControl();
          zone?.Run.SetState(false);
        }
      }

      //останавливаем поток вычисления средней температуры
      AverageTemperatureProvider?.StopControl();

      //останавливаем нагреватель газа
      GasHeater?.StopControl();

      //останавливаем управление заслонкой
      Damper?.StopControl();

      //останавливаем поток подтверждения связи со шкафом управления
      Ping?.StopControl();

      //останавливаем опрос устройств
      if (Devices != null)
      {
        foreach (var device in Devices.Items.Values)
        {
          device?.Stop();
        }
      }
    }
  }
}
