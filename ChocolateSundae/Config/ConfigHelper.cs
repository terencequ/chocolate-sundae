using System;
using System.IO;
using System.Text.Json;
using ChocolateSundae.Config.Models;

namespace ChocolateSundae.Config
{
    public static class ConfigHelper
    {
        public const string ConfigPath = "config.json";

        /// <summary>
        /// Configuration, as part of JSON file.
        /// </summary>
        public static ConfigModel Config {

            get
            {
                ConfigModel configModel;
                try 
                {
                    var configString = File.ReadAllText(ConfigPath);
                    configModel = JsonSerializer.Deserialize<ConfigModel>(configString) ?? new ConfigModel();
                } catch (Exception)
                {
                    configModel = new ConfigModel();
                }

                return configModel;
            } 
            set
            {
                var configString = JsonSerializer.Serialize(value, options: new JsonSerializerOptions() { WriteIndented=true });
                File.WriteAllText(ConfigPath, configString);
            }
        }
    }
}
