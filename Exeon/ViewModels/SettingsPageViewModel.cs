using Exeon.Services.IServices;
using Exeon.ViewModels.Tools;

namespace Exeon.ViewModels
{
    public class SettingsPageViewModel : ObservableObject
    {
        private readonly INavigationService _navigationService;
        private readonly AppState _appState;

        public SettingsPageViewModel(INavigationService navigationService, AppState appState)
        {
            _navigationService = navigationService;
            _appState = appState;
        }
    }
}
