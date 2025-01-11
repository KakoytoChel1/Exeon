using System.Threading.Tasks;

namespace Exeon.Models.Actions
{
    public class SystemSoundAction : Action
    {
        public SystemSoundAction(int soundLevel)
        {
            SoundLevel = soundLevel; 
        }

        public int SoundLevel { get; set; }

        public override Task Execute()
        {
            throw new System.NotImplementedException();
        }
    }
}
