using System;
using System.Threading.Tasks;

namespace Exeon.Models.Actions
{
    public class PauseAction : Action
    {
        public PauseAction(long delay)
        {
            DelayInSeconds = delay;
        }

        public long DelayInSeconds { get; set; }

        public override Task Execute()
        {
            throw new System.NotImplementedException();
        }
    }
}
