using MaterialDesignExtensions.Model;
using NTCC.NET.Core.Facility;
using NTCC.NET.Core.Stages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NTCC.NET.ViewModels
{
    class StagesViewModel : PageViewModel
    {
        public StagesViewModel()
        {
            Stages = new ObservableCollection<IStep>();

            foreach (StageBase stage in ArtMonbatFacility.Stages.Items.Values)
            {
                IStep step = new Step() { Header = new StepTitleHeader() {
                    SecondLevelTitle = stage.Title,
                    FirstLevelTitle = stage.Description}};

                Stages.Add(step);
                stage.Tick += OnStageTick;
            }

            // Создаем новый поток и передаем метод, который будет выполняться в потоке            
            collectionThread = new Thread(() => UpdateStageFunction(stop));
            collectionThread.Start();
        }

        private void OnStageTick(object sender, FacilityMessageArgs agrs)
        {
            //throw new NotImplementedException();
        }

        // Создаем токен отмены
        CancellationTokenSource stop = new CancellationTokenSource();

        // Создаем новый поток и передаем метод, который будет выполняться в потоке
        private Thread collectionThread = null;


        public IStep CurrentStage
        {
            get => currentStage;
            set 
            {
                if (currentStage == value)
                    return;

                currentStage = value;
                OnPropertyChanged();
            }
        }
        private IStep currentStage = null;

        // Метод, который будет выполняться в отдельном потоке
        void UpdateStageFunction(CancellationTokenSource stop)
        {
            try
            {
                int indexStage = -1;
                while (true)
                {
                    stop.Token.ThrowIfCancellationRequested();

                    if (stop.IsCancellationRequested)
                        break;

                    Thread.Sleep(3000);

                    if (indexStage < Stages.Count - 1)                    
                        indexStage++;
                    else
                        indexStage = -1;

                    CurrentStage = indexStage == -1 ? null : Stages[indexStage];
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

        /// <summary>
        /// Represents the step view items.
        /// </summary>
        private ObservableCollection<IStep> m_stepViewItems;

        /// <summary>
        /// Gets or sets the step view items.
        /// </summary>
        public ObservableCollection<IStep> Stages
        {
            get
            {
                return m_stepViewItems;
            }
            set
            {
                m_stepViewItems = value;
                OnPropertyChanged();
            }
        }
    }
}
