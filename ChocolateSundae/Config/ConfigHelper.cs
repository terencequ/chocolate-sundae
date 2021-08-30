using ChocolateSundae.Services.Config.Models;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace ChocolateSundae.Services
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
