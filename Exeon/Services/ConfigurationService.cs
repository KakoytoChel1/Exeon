using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Exeon.Services.IServices;

namespace Exeon.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private const string ConfigFileName = "appSettings.json";
        private readonly string _configFilePath;
        private Dictionary<string, string> _settings = null!;

        public ConfigurationService()
        {
            var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var configFolder = Path.Combine(appDataFolder, "Exeon");
            if (!Directory.Exists(configFolder))
                Directory.CreateDirectory(configFolder);

            _configFilePath = Path.Combine(configFolder, ConfigFileName);

            if (!File.Exists(_configFilePath))
            {
                _settings = new Dictionary<string, string>() 
                {
                    { "SpeechModelPath", "" },
                    { "UILanguage", "UA" },
                    { "StartPageTag", "Chat" }
                };
                SaveConfiguration();
            }
            else
            {
                LoadConfiguration();
            }
        }

        /// <summary>
        /// Получает настройку по ключу с автоматическим преобразованием в указанный тип.
        /// Если настройки нет или преобразование не удалось, возвращается значение по умолчанию.
        /// </summary>
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

        /// <summary>
        /// Устанавливает настройку с заданным ключом и сохраняет изменения в файл.
        /// </summary>
        public void Set<T>(string key, T value)
        {
            _settings[key] = value?.ToString();
            SaveConfiguration();
        }

        /// <summary>
        /// Загружает настройки из JSON-файла.
        /// Если происходит ошибка десериализации – создается новый словарь.
        /// </summary>
        private void LoadConfiguration()
        {
            try
            {
                var json = File.ReadAllText(_configFilePath);
                _settings = JsonSerializer.Deserialize<Dictionary<string, string>>(json)
                            ?? new Dictionary<string, string>();
            }
            catch
            {
                _settings = new Dictionary<string, string>();
            }
        }

        /// <summary>
        /// Сохраняет текущие настройки в JSON-файл.
        /// </summary>
        private void SaveConfiguration()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(_settings, options);
            File.WriteAllText(_configFilePath, json);
        }
    }
}
