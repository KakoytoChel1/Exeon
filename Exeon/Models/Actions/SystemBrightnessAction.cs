using System.Threading.Tasks;

namespace Exeon.Models.Actions
{
    public class SystemBrightnessAction : Action
    {
        public SystemBrightnessAction() { }

        public SystemBrightnessAction(int brightnessLevel)
        {
            BrightnessLevel = brightnessLevel;
        }

        public int BrightnessLevel { get; set; }

        public override Task Execute()
        {
            throw new System.NotImplementedException();
        }
    }
}
