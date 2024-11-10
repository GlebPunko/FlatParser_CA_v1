using FlatParser_CA_v1.Models;
using System.Text.Json;

namespace FlatParser_CA_v1.Helpers
{
    public class ConfigReader
    {
        private readonly string _configFilePath;

        public ConfigReader(string configFilePath)
        {
            _configFilePath = configFilePath;
        }

        public Config ReadConfig()
        {
            string jsonString = File.ReadAllText(_configFilePath);
            return JsonSerializer.Deserialize<Config>(jsonString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
    }
}
