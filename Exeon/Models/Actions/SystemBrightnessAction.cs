using Exeon.Models.Tools;
using System;
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

        public override Task<ValueTuple<bool, string>> Execute()
        {
            try
            {
                SystemBrightnessController.SetBrightness(BrightnessLevel); 
                return Task.FromResult(ValueTuple.Create(true, $"Рівень яскравості встановлено на: {BrightnessLevel}%."));
            }
            catch (Exception ex)
            {
                return Task.FromResult(ValueTuple.Create(false, ex.Message));
            }
        }
    }
}
