using ChocolateSundae.Displays.Models;
using ChocolateSundae.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ChocolateSundae.Config;
using ChocolateSundae.Config.Models;

namespace ChocolateSundae.Displays
{
    /// <summary>
    /// Interaction logic for ConfigDisplay.xaml
    /// </summary>
    public partial class ConfigDisplay : UserControl
    {
        private ConfigDisplayModel model;

        public ConfigDisplay()
        {
            InitializeComponent();
            var config = ConfigHelper.Config;
            model = new ConfigDisplayModel();
            model.Username = config.Username;
            model.Password = config.Password;
            DataContext = model;
        }

        private void OnReset(object sender, RoutedEventArgs e)
        {
            var config = ConfigHelper.Config;
            model.Username = config.Username;
            model.Password = config.Password;
            model.SetStatusInfo($"Reloaded username and password from {ConfigHelper.ConfigPath}.");
        }

        private void OnSave(object sender, RoutedEventArgs e)
        {
            if(model.Username is null or "" || model.Password is null or "")
            {
                model.SetStatusError("Fields cannot be empty! Nothing was saved.");
            } else
            {
                var config = new ConfigModel();
                config.Username = model.Username;
                config.Password = model.Password;
                ConfigHelper.Config = config;
                model.SetStatusSuccess($"Successfully saved to {ConfigHelper.ConfigPath}.");
            }
        }
    }
}
