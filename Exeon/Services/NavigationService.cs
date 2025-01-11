using Exeon.Services.IServices;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System;

namespace Exeon.Services
{
    public class NavigationService : INavigationService
    {
        private Frame? _frame { get; set; }

        public void InitializeFrame(Frame? frame)
        {
            _frame = frame ?? throw new ArgumentNullException("Provided instance of the Frame is null.");
            _frame.CacheSize = 1;
        }

        public void ChangePage<TPage>(SlideNavigationTransitionEffect transitionEffect = SlideNavigationTransitionEffect.FromBottom) where TPage : Page
        {
            if (_frame == null)
                throw new ArgumentNullException("The instance of the root frame is null.");
            else
                _frame.Navigate(typeof(TPage), null, new SlideNavigationTransitionInfo() { Effect = transitionEffect });
        }

        public void ChangePage<TPage>(Frame? frame, SlideNavigationTransitionEffect transitionEffect = SlideNavigationTransitionEffect.FromBottom) where TPage : Page
        {
            if (frame != null)
            {
                frame.CacheSize = 4;
                frame.Navigate(typeof(TPage), null, new SlideNavigationTransitionInfo() { Effect = transitionEffect });
            }
        }

        public void GoBack()
        {
            if (_frame == null)
                throw new ArgumentNullException("The instance of the root frame is null.");
            else
                _frame.GoBack();
        }

        public void GoBack(Frame? frame)
        {
            if(frame != null)
            {
                frame.CacheSize = 1;
                frame.GoBack();
            }
        }
    }
}
