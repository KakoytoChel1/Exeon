using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;

namespace Exeon.Services.IServices
{
    public interface INavigationService
    {
        void InitializeFrame(Frame? frame);
        void ChangePage<TPage>(SlideNavigationTransitionEffect transitionEffect = SlideNavigationTransitionEffect.FromBottom) where TPage : Page;

        void ChangePage<TPage>(Frame? frame, SlideNavigationTransitionEffect transitionEffect = SlideNavigationTransitionEffect.FromBottom) where TPage : Page;

        void GoBack();
        void GoBack(Frame? frame);
    }
}
