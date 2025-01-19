using System;
using System.Threading;
using System.Threading.Tasks;
using Vosk;
using NAudio.Wave;
using Exeon.Services.IServices;

namespace Exeon.Services
{
    public class SpeechRecognitionService : ISpeechRecognitionService
    {
        private readonly Model _model = null!;
        private VoskRecognizer? _recognizer;
        private WaveInEvent? _waveIn;

        public event EventHandler<string> PartialRecognition = null!;
        public event EventHandler<string> FinalRecognition = null!;

        public SpeechRecognitionService(string modelPath)
        {
            _model = new Model(modelPath);
        }

        public async Task StartRecognitionAsync(CancellationToken token)
        {
            _waveIn = new WaveInEvent
            {
                WaveFormat = new WaveFormat(16000, 1)
            };

            _recognizer = new VoskRecognizer(_model, 16000.0f);

            _waveIn.DataAvailable += _waveIn_DataAvailable;

            _waveIn.StartRecording();

            try
            {
                while (!token.IsCancellationRequested)
                {
                    await Task.Delay(100, token);
                }
            }
            catch (OperationCanceledException)
            {
                // ...
            }
        }

        private void _waveIn_DataAvailable(object? sender, WaveInEventArgs e)
        {
            if (_recognizer!.AcceptWaveform(e.Buffer, e.BytesRecorded))
            {
                string result = _recognizer.Result();
                FinalRecognition?.Invoke(this, result);
            }
            else
            {
                string partialResult = _recognizer.PartialResult();
                PartialRecognition?.Invoke(this, partialResult);
            }
        }

        public void StopRecognition()
        {
            if (_waveIn != null)
            {
                _waveIn.DataAvailable -= _waveIn_DataAvailable;
                _waveIn.StopRecording();
                _waveIn.Dispose();
                _waveIn = null;
            }

            if (_recognizer != null)
            {
                _recognizer.Dispose();
                _recognizer = null;
            }
        }

    }

}
