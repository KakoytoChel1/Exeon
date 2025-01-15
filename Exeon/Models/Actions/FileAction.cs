using System;
using System.Diagnostics;
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

        public override Task<ValueTuple<bool, string>> Execute()
        {
            try
            {
                Process.Start(new ProcessStartInfo { FileName = PathToFile, UseShellExecute = true });
                return Task.FromResult(ValueTuple.Create(true, $"Успішно відкрито файл за шляхом: {PathToFile}."));
            }
            catch (Exception ex)
            {
                return Task.FromResult(ValueTuple.Create(false, ex.Message));
            }
        }

    }
}
