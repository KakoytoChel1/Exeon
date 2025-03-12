namespace Exeon.Services.IServices
{
    public interface IConfigurationService
    {
        T Get<T>(string key, T defaultValue = default!);
        void Set<T>(string key, T value);
    }
}
