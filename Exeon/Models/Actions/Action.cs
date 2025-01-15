using Exeon.Models.Commands;
using System;
using System.Threading.Tasks;

namespace Exeon.Models.Actions
{
    public abstract class Action
    {
        public int Id { get; set; }

        public int OrderIndex { get; set; }

        public CustomCommand RootCommand { get; set; } = null!;
        public int RootCommandId { get; set; }

        public abstract Task<ValueTuple<bool, string>> Execute();
    }
}
