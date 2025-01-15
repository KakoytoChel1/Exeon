using Exeon.Models.Tools;
using System;
using System.Threading.Tasks;

namespace Exeon.Models.Actions
{
    public class SystemSoundAction : Action
    {
        public SystemSoundAction() { }

        public SystemSoundAction(int soundLevel)
        {
            SoundLevel = soundLevel; 
        }

        public int SoundLevel { get; set; }

        public override Task<ValueTuple<bool, string>> Execute()
        {
            try
            {
                SystemSoundController controller = new SystemSoundController();
                controller.SetVolume(SoundLevel / 100f);

                return Task.FromResult(ValueTuple.Create(true, $"Рівень звуку системи встановлено на: {SoundLevel}%."));
            }
            catch (Exception ex)
            {
                return Task.FromResult(ValueTuple.Create(false, ex.Message));
            }
        }
    }
}
