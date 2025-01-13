using System.Threading.Tasks;

namespace Exeon.Models.Actions
{
    public class WebAction : Action
    {
        public WebAction() { }

        public WebAction(string uri)
        {
            Uri = uri;
        }

        public string Uri { get; set; } = null!;

        public override Task Execute()
        {
            throw new System.NotImplementedException();
        }
    }
}
