﻿using NTCC.NET.Core.Facility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveCharts;
using LiveCharts.Wpf;
using System.Windows.Media;
using System.Threading;

namespace NTCC.NET.ViewModels
{
    internal class FacilityViewModel : PageViewModel
    {

        static Dictionary<ReactorHeatingZone, SeriesCollection> series = new Dictionary<ReactorHeatingZone, SeriesCollection>();  
        
        public FacilityViewModel()
        {
            var zones = ArtMonbatFacility.ReactorZones.Items.Values.ToList();

            foreach (ReactorHeatingZone zone in zones)
            {
                HeatingZones.Add(zone);

                SeriesCollection zoneCollection = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Температура стенки, °C",
                        Values = new ChartValues<double> { },
                        Fill= new SolidColorBrush(Colors.Transparent),
                        PointGeometry = null,                        
                    },
                    new LineSeries
                    {
                        Title = "Температура нагревателей (макс.), °C",
                        Values = new ChartValues<double> { },
                        Fill= new SolidColorBrush(Colors.Transparent),
                        PointGeometry = null
                    },
                    
                    new StepLineSeries
                    {
                        Title = "Максимальная температура, °C",
                        Values = new ChartValues<double> { },
                        Fill= new SolidColorBrush(Colors.Transparent),
                        PointGeometry = null,
                        StrokeThickness = 1
                    },
                    new StepLineSeries
                    {
                        Title = "Минимальная температура, °C",
                        Values = new ChartValues<double> {  },
                        Fill= new SolidColorBrush(Colors.Transparent),
                        PointGeometry = null,
                        StrokeThickness = 1
                    },

                    new StepLineSeries
                    {
                        Title = "Температура отключения нагревателей, °C",
                        Values = new ChartValues<double> { },
                        Fill= new SolidColorBrush(Colors.Transparent),
                        PointGeometry = null,
                        StrokeThickness = 1
                    },

                    new StepLineSeries
                    {
                        Title = "Поддерживаемая мощность, %",
                        Values = new ChartValues<double> {  },
                        Fill= new SolidColorBrush(Colors.Transparent),
                        PointGeometry = null,
                        StrokeThickness = 1,
                        ScalesYAt = 1
                    },

                    new StepLineSeries
                    {
                        Title = "Текущая мощность, %",
                        Values = new ChartValues<double> { },
                        Fill= new SolidColorBrush(Colors.Transparent),
                        PointGeometry = null,
                        StrokeThickness = 2,
                        ScalesYAt = 1
                    },
                };

                series.Add(zone, zoneCollection);
            }

            SelectedHeatingZone = zones[0];

            // Создаем новый поток и передаем метод, который будет выполняться в потоке
            collectionThread = new Thread(() => UpdateSeriesFunction(stop));
            collectionThread.Start();
        }

        public SeriesCollection SelectedSeriesCollection { get; set; }

        public ObservableCollection<ReactorHeatingZone> HeatingZones
        {
            get;
            private set;
        } = new ObservableCollection<ReactorHeatingZone>();

        public ReactorHeatingZone SelectedHeatingZone
        {
            get => selectedHeatingZone;
            set
            {
                if (value == selectedHeatingZone)
                    return;

                selectedHeatingZone = value;
                OnPropertyChanged();
                OnPropertyChanged("IsHeatingZoneSelected");

                SelectedSeriesCollection = series[selectedHeatingZone];
                OnPropertyChanged("SelectedSeriesCollection");
            }
        }

        ReactorHeatingZone selectedHeatingZone = null;

        public bool IsHeatingZoneSelected
        {
            get => (selectedHeatingZone == null) ? false : true;
        }

        // Флаг для сигнализации о завершении потока
        //private bool stopCollectionThread = false;
        
        // Создаем токен отмены
        CancellationTokenSource stop = new CancellationTokenSource();

        // Создаем новый поток и передаем метод, который будет выполняться в потоке
        private Thread collectionThread = null;

        // Метод, который будет выполняться в отдельном потоке
        void UpdateSeriesFunction(CancellationTokenSource stop)
        {
            try
            {
                while (true)
                {
                    stop.Token.ThrowIfCancellationRequested();

                    if (stop.IsCancellationRequested)
                        break;

                    Thread.Sleep(3000);

                    foreach (ReactorHeatingZone zone in ArtMonbatFacility.ReactorZones.Items.Values)
                    {

                        SeriesCollection zoneCollection = series[zone];

                        zoneCollection[0].Values.Add(zone.WallTemperature.Value);

                        double heatingElementsMaxTemperature = zone.HeatingElements.Max(heatingElement => heatingElement.Temperature.Value);
                       
                        zoneCollection[1].Values.Add(heatingElementsMaxTemperature);

                        zoneCollection[2].Values.Add(zone.MaxTargetWallTemperature);

                        zoneCollection[3].Values.Add(zone.MinTargetWallTemperature);

                        zoneCollection[4].Values.Add(zone.MaxHeaterTemperature);

                        zoneCollection[5].Values.Add(zone.MaxPowerLevel);

                        zoneCollection[6].Values.Add(zone.DutyWrite.Value);


                        bool clearFirst = zoneCollection[0].Values.Count > 60;
                        if (clearFirst)
                        {
                            foreach (var serie in zoneCollection)
                                serie.Values.RemoveAt(0);
                        }
                    }
                }
            }
            catch (OperationCanceledException) 
            {                
            }
        }

        public override void Stop()
        {
            // Останавливаем поток обновления графики
            stop.Cancel();

            // Ожидаем завершение потока
            collectionThread.Join(0);
        }
    }
}
