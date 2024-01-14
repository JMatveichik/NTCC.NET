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
        public static FaciliitySet<DataPoint> DataPoints
        {
            get;
        } = new FaciliitySet<DataPoint>();

        /// <summary>
        /// Reactor heating zones set
        /// </summary>
        public static FaciliitySet<ReactorHeatingZone> ReactorHeaters
        {
            get;
        } = new FaciliitySet<ReactorHeatingZone>();

        /// <summary>
        /// Facility stages set
        /// </summary>
        public static FaciliitySet<StageBase> Stages
        {
            get;
        } = new FaciliitySet<StageBase>();

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
        public static FaciliitySet<AcquisitionDeviceBase> Devices
        {
            get;
        } = new FaciliitySet<AcquisitionDeviceBase>();

        #endregion

        /// <summary>
        /// Директория конфигурационных файлов
        /// </summary>
        public static string ConfigDirectory 
        {
            get;
            private set;
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
            catch(Exception ex)
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

            // Запускаем поток ping
            pingThread.Start();
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
                if (!ReactorHeaters.Add(heatingZone))
                {
                    ReactorHeatingZone duplicate = ReactorHeaters[heatingZone.ID];
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
            
        }

        


        // Флаг для сигнализации о завершении потока
        private static volatile bool stopPingThread = false;

        // Создаем новый поток и передаем метод, который будет выполняться в потоке
        private Thread pingThread = new Thread(new ThreadStart(PingFunction));

        // Метод, который будет выполняться в отдельном потоке
        static void PingFunction()
        {
            DiscreteOutputDataPoint pingDataPoint = (DiscreteOutputDataPoint)DataPoints["UNIT.PING"];

            while (!stopPingThread)
            {
                Thread.Sleep(3000);
                pingDataPoint.SetState(true);
                //Debug.WriteLine("Ping ... ");
            }
        }

        public void Stop()
        {
            foreach(var device in Devices.Items.Values)
            {
                device.Stop();
            }

            // Останавливаем поток ping
            stopPingThread = true;
            pingThread.Join();

        }

       
    }
}
