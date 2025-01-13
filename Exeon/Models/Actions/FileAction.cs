using System.Threading.Tasks;

namespace Exeon.Models.Actions
{
    public class FileAction : Action
    {
        public FileAction() { }

        public FileAction(string pathToFile)
        {
            PathToFile = pathToFile;
        }

        public string PathToFile { get; set; } = null!;

        public override Task Execute()
        {
            throw new System.NotImplementedException();
        }
    }
}
