using System;
using NAudio.CoreAudioApi;

namespace Exeon.Models.Tools
{
    public class SystemSoundController
    {
        private readonly MMDeviceEnumerator _deviceEnumerator;
        private readonly MMDevice _defaultDevice;

        public SystemSoundController()
        {
            _deviceEnumerator = new MMDeviceEnumerator();
            _defaultDevice = _deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
        }

        public void SetVolume(float volumeLevel)
        {
            if (volumeLevel < 0.0f || volumeLevel > 1.0f)
                throw new ArgumentOutOfRangeException(nameof(volumeLevel), "Volume level must be between 0.0 and 1.0");

            _defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar = volumeLevel;
        }

        public float GetVolume()
        {
            return _defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar;
        }
    }
}
