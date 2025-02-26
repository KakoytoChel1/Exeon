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
        public CustomCommand? OriginalCommandState { get; set; }

        public AppState()
        {
            ApplicationContext = new ApplicationContext();

            // Загружаем команды с отсортированными действиями на уровне базы данных
            var commands = ApplicationContext.CustomCommands
                .Include(c => c.Actions.OrderBy(a => a.OrderIndex)) // Сортируем действия
                .ToList();

            CustomCommands = new ObservableCollection<CustomCommand>(commands);
        }


        #region Properties

        public ObservableCollection<CustomCommand> CustomCommands { get; set; }

        public ApplicationContext ApplicationContext { get; private set; }


        private bool _isSidePanelButtonsEnabled;
        public bool IsSidePanelButtonsEnabled
        {
            get { return _isSidePanelButtonsEnabled; }
            set { _isSidePanelButtonsEnabled = value; OnPropertyChanged(); }
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
        #endregion

        // Создаем копию данных, таким образом избегая tracking'а со стороны DbContext
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

        public async Task<bool> CanAddNewCustomCommand(string commandText)
        {
            bool result = true;

            await Task.Run(() =>
            {
                var command = CustomCommands.FirstOrDefault(com => com.Command.ToLower() == commandText);

                if (command != null)
                    result = false;
            });

            return result;
        }

        public async Task<bool> CanModifyCustomCommand(int id, string commandText)
        {
            bool result = true;

            await Task.Run(() =>
            {
                var command = CustomCommands.FirstOrDefault(com => com.Id != id && com.Command.ToLower() == commandText);

                if (command != null)
                    result = false;
            });

            return result;
        }
    }
}
