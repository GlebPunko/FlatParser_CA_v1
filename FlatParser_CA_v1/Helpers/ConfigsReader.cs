using FlatParser_CA_v1.Models;
using System.Text.Json;

namespace FlatParser_CA_v1.Helpers
{
    public class ConfigsReader
    {
        private readonly string _configFilePath;
        private readonly string _cursorBrestPath;

        public ConfigsReader(string configFilePath, string cursorBrestPath)
        {
            _configFilePath = configFilePath;
            _cursorBrestPath = cursorBrestPath;
        }

        public StoredConfigs ReadConfig()
        {
            string configJson = File.ReadAllText(_configFilePath);
            string cursorBrestJson = File.ReadAllText(_cursorBrestPath);

            JsonSerializerOptions options = new()
            {
                PropertyNameCaseInsensitive = true
            };

            var config = JsonSerializer.Deserialize<Config>(configJson, options);
            var brestCursor = JsonSerializer.Deserialize<BrestCursor>(cursorBrestJson, options);
 
            return new StoredConfigs{ Config = config, BrestCursor = brestCursor };
        }
    }
}
