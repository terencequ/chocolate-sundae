using ChocolateSundae.Displays.Models;
using ChocolateSundae.Services;
using ChocolateSundae.Services.Abstractions;
using ChocolateSundae.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChocolateSundae.Displays
{
    /// <summary>
    /// Interaction logic for MainDisplay.xaml
    /// </summary>
    public partial class MainDisplay : UserControl
    {
        private MainDisplayModel model;
        private IInstagramService? instagramService;

        public MainDisplay()
        {
            InitializeComponent();
            model = new MainDisplayModel();
            DataContext = model;
        }

        private async void OnDownloadUserInfo(object sender, RoutedEventArgs e)
        {
            var result = "";
            if(UserListBox.SelectedIndex == -1)
            {
                result = "No user was selected! Please select a user.";
                model.AddLog(result);
            } else
            {
                result = await GetUserProfile(UserListBox.SelectedItem?.ToString() ?? "");
                model.AddLog(result);
            }

            LogScrollViewer.ScrollToBottom();
        }

        internal async Task<string> GetUserProfile(string username)
        {
            var result = "";
            instagramService = new InstagramService();
            await instagramService.TryAuthenticateAsync();
            if (!instagramService.IsAuthenticated())
            {
                result = "Failed to authenticate.\n";
            } else
            {
                try
                {
                    result = await instagramService.GetUserProfileOrDefaultAsync(username);
                }
                catch (InstagramErrorException e)
                {
                    result = e.Message;
                }
            }
            return $"Getting user information for {username}:\n{result}";
        }

        private void OnAddUser(object sender, RoutedEventArgs e)
        {
            model.AddUser();
            UserListBox.SelectedIndex = UserListBox.Items.Count - 1;
        }

        private void OnRemoveUser(object sender, RoutedEventArgs e)
        {
            int index = UserListBox.SelectedIndex;
            if(index != -1)
            {
                model.RemoveUser(index);
                UserListBox.SelectedIndex = UserListBox.Items.Count - 1;
            }
        }

        private void OnUserListBoxSelection(object sender, SelectionChangedEventArgs e)
        {
            model.SelectedUserText = UserListBox.SelectedIndex == -1 
                ? ""
                : $"Selected user: {UserListBox.SelectedItem?.ToString()}";
        }
    }
}
