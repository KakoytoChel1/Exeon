using Exeon.Models.Commands;
using System.Threading.Tasks;

namespace Exeon.Models.Actions
{
    public abstract class Action
    {
        public int Id { get; set; }

        public CustomCommand RootCommand { get; set; } = null!;

        public abstract Task Execute();
    }
}
