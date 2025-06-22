using Exeon.Models;
using Exeon.Models.Actions;
using Exeon.Models.Commands;
using Exeon.ViewModels.Tools;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Action = Exeon.Models.Actions.Action;

namespace Exeon.ViewModels
{
    public class AppState : ObservableObject
    {
        private const double APP_VERSION = 1.1;
        public CustomCommand? OriginalCommandState { get; set; }

        public AppState() { }

        public async Task InitializeDataBase()
        {
            await ApplicationContext.InitializeDatabasePathAsync();
            ApplicationContext = new ApplicationContext();

            // Загружаем команды с отсортированными действиями на уровне базы данных
            var commands = ApplicationContext.CustomCommands.OrderBy(c => c.OrderIndex) // Сортируем команды
                .Include(c => c.Actions.OrderBy(a => a.OrderIndex)) // Сортируем действия
                .Include(c => c.TriggerCommands)
                .ToList();

            CustomCommands = new ObservableCollection<CustomCommand>(commands);

            IsSpeechModelWarningVisible = false;
        }

        #region Properties

        public ObservableCollection<CustomCommand> CustomCommands { get; set; } = null!;

        public ApplicationContext ApplicationContext { get; private set; } = null!;

        // Удобство для титулки
        public string AppTitleText
        {
            get { return $"Exeon {APP_VERSION} beta"; }
        }

        private bool _isSidePanelButtonsEnabled;
        public bool IsSidePanelButtonsEnabled
        {
            get { return _isSidePanelButtonsEnabled; }
            set { _isSidePanelButtonsEnabled = value; OnPropertyChanged(); }
        }

        private bool _isSpeechModelWarningVisible;
        public bool IsSpeechModelWarningVisible
        {
            get { return _isSpeechModelWarningVisible; }
            set { _isSpeechModelWarningVisible = value; OnPropertyChanged(); }
        }

        private bool _isSpeechModelInitializingFailed;
        public bool IsSpeechModelInitializingFailed
        {
            get { return _isSpeechModelInitializingFailed; }
            set { _isSpeechModelInitializingFailed = value; OnPropertyChanged(); }
        }

        private CustomCommand? _selectedModifyingCustomCommand;
        /// <summary>
        /// Stores a reference to the selected instance.
        /// </summary>
        public CustomCommand? SelectedModifyingCustomCommand
        {
            get { return _selectedModifyingCustomCommand; }
            set { _selectedModifyingCustomCommand = value; OnPropertyChanged(); }
        }

        private bool _isCommandRunning;
        public bool IsCommandRunning
        {
            get { return _isCommandRunning; }
            set { _isCommandRunning = value; OnPropertyChanged(); }
        }

        private double _commandExecutionProgress;
        public double CommandExecutionProgress
        {
            get { return _commandExecutionProgress; }
            set { _commandExecutionProgress = value; OnPropertyChanged(); }
        }

        private bool _isListening = false;
        public bool IsListening
        {
            get { return _isListening; }
            set { _isListening = value; OnPropertyChanged(); }
        }

        private bool _isApproximateModeOn = false;
        public bool IsApproximateModeOn
        {
            get { return _isApproximateModeOn; }
            set { _isApproximateModeOn = value; OnPropertyChanged(); }
        }
        #endregion

        // Создаем копию данных, таким образом избегая tracking'а со стороны DbContext
        // Метод нужен, дабы учитывать разносортность типов Actions
        public Action CloneAction(Action action)
        {
            return action switch
            {
                FileAction fileAction => new FileAction(fileAction.PathToFile) { Id = fileAction.Id },
                WebAction webAction => new WebAction(webAction.Uri) { Id = webAction.Id },
                PauseAction pauseAction => new PauseAction(pauseAction.DelayInSeconds) { Id = pauseAction.Id },
                SystemBrightnessAction brightnessAction => new SystemBrightnessAction(brightnessAction.BrightnessLevel) { Id = brightnessAction.Id },
                SystemSoundAction soundAction => new SystemSoundAction(soundAction.SoundLevel) { Id = soundAction.Id },
                _ => throw new NotSupportedException($"Unsupported action type: {action.GetType().Name}")
            };
        }
    }
}
