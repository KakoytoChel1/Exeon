using System;
using System.Threading;
using System.Threading.Tasks;

namespace Exeon.Services.IServices
{
    public interface ISpeechRecognitionService
    {
        event EventHandler<string> PartialRecognition;
        event EventHandler<string> FinalRecognition;

        bool IsInitialized { get; }

        Task InitializeSpeechModel(string pathToModel);
        Task StartRecognitionAsync(CancellationToken token);
        void StopRecognition();
    }

}
