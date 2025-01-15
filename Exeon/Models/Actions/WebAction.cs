using System;
using System.Diagnostics;
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

        public override Task<ValueTuple<bool, string>> Execute()
        {
            try
            {
                Process.Start(new ProcessStartInfo { FileName = Uri, UseShellExecute = true });
                return Task.FromResult(ValueTuple.Create(true, $"Успішно відкрито веб-сторінку за посиланням: {Uri}."));
            }
            catch (Exception ex)
            {
                return Task.FromResult(ValueTuple.Create(false, ex.Message));
            }
        }
    }
}
