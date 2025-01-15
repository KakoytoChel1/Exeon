using System;
using System.Threading.Tasks;

namespace Exeon.Models.Actions
{
    public class PauseAction : Action
    {
        public PauseAction() { }

        public PauseAction(long delay)
        {
            DelayInSeconds = delay;
        }

        public long DelayInSeconds { get; set; }

        public override async Task<ValueTuple<bool, string>> Execute()
        {
            for (int i = 0; i < DelayInSeconds; i++)
            {
                await Task.Delay(1000);
            }
            return ValueTuple.Create(false, string.Empty);
        }
    }
}
