using Exeon.Models.Actions;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Exeon.ViewModels.Tools
{
    public class ActionTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? FileActionTemplate { get; set; }
        public DataTemplate? WebActionTemplate { get; set; }
        public DataTemplate? PauseActionTemplate { get; set; }
        public DataTemplate? BrightnessActionTemplate { get; set; }
        public DataTemplate? SoundActionTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return item switch
            {
                FileAction => FileActionTemplate!,
                WebAction => WebActionTemplate!,
                PauseAction => PauseActionTemplate!,
                SystemBrightnessAction => BrightnessActionTemplate!,
                SystemSoundAction => SoundActionTemplate!,
                _ => base.SelectTemplateCore(item, container)
            };
        }
    }
}
