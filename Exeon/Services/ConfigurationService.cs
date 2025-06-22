using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;
using Exeon.Services.IServices;

namespace Exeon.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private const string ConfigFileName = "appSettings.json";
        private Dictionary<string, string> _settings = new();
        private readonly StorageFolder _localFolder = ApplicationData.Current.LocalFolder;

        public ConfigurationService()
        {
            // Вызов LoadAsync не может быть в конструкторе — используйте InitAsync отдельно.
        }

        public async Task InitAsync()
        {
            if (await FileExistsAsync(ConfigFileName))
            {
                await LoadConfigurationAsync();
            }
            else
            {
                _settings = new Dictionary<string, string>
                {
                    { "SpeechModelPath", "" },
                    { "UILanguage", "UA" },
                    { "StartPageTag", "Guide" },
                    { "IsApproximateModeOn", "false" }
                };
                await SaveConfigurationAsync();
            }
        }

        public T Get<T>(string key, T defaultValue = default!)
        {
            if (_settings.TryGetValue(key, out var value))
            {
                try
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch
                {
                    return defaultValue;
                }
            }
            return defaultValue;
        }

        public async void Set<T>(string key, T value)
        {
            _settings[key] = value?.ToString() ?? "";
            await SaveConfigurationAsync();
        }

        private async Task LoadConfigurationAsync()
        {
            try
            {
                var file = await _localFolder.GetFileAsync(ConfigFileName);
                var json = await FileIO.ReadTextAsync(file);
                _settings = JsonSerializer.Deserialize<Dictionary<string, string>>(json)
                            ?? new Dictionary<string, string>();
            }
            catch
            {
                _settings = new Dictionary<string, string>();
            }
        }

        private async Task SaveConfigurationAsync()
        {
            try
            {
                var json = JsonSerializer.Serialize(_settings, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                var file = await _localFolder.CreateFileAsync(ConfigFileName, CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(file, json);
            }
            catch
            {
                // ...
            }
        }

        private async Task<bool> FileExistsAsync(string fileName)
        {
            try
            {
                await _localFolder.GetFileAsync(fileName);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
