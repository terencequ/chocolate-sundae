using ChocolateSundae.Displays.Models;
using ChocolateSundae.Services;
using ChocolateSundae.Services.Abstractions;
using ChocolateSundae.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using ChocolateSundae.Services.Models;
using Path = System.IO.Path;

namespace ChocolateSundae.Displays
{
    /// <summary>
    /// Interaction logic for MainDisplay.xaml
    /// </summary>
    public partial class MainDisplay : UserControl
    {
        private MainDisplayModel model;
        private IInstagramService? _instagramService;
        private ISpreadsheetService? _spreadsheetService;

        public MainDisplay()
        {
            InitializeComponent();
            model = new MainDisplayModel();
            DataContext = model;
        }

        /// <summary>
        /// Download selected user's information into a spreadsheet.
        /// </summary>
        private async void OnDownloadSelectedUserInfo(object sender, RoutedEventArgs e)
        {
            var result = "";
            if(UserListBox.SelectedIndex == -1)
            {
                result = "No user was selected! Please select a user.";
                model.AddLog(result);
            } else
            {
                var username = UserListBox.SelectedItem?.ToString() ?? "";
                model.AddLog($"Getting user information for {username}...");
                DownloadSelectedUserButton.IsEnabled = false;
                DownloadAllUsersButton.IsEnabled = false;
                result = await GetAndDownloadUserProfiles(new Progress<UserDataProgress>(UpdateProgress), UserListBox.SelectedItem?.ToString() ?? "");
                model.AddLog(result);
            }

            LogScrollViewer.ScrollToBottom();
            DownloadSelectedUserButton.IsEnabled = true;
            DownloadAllUsersButton.IsEnabled = true;
        }

        /// <summary>
        /// Download all user information into a spreadsheet.
        /// </summary>
        private async void OnDownloadAllUsersInfo(object sender, RoutedEventArgs e)
        {
            var result = "";
            if(!UserListBox.HasItems)
            {
                result = "No user was selected! Please select a user.";
                model.AddLog(result);
            } else
            {
                var usernames = UserListBox.Items.OfType<string>().ToArray();
                model.AddLog($"Getting user information for all {usernames.Length} users...");
                DownloadSelectedUserButton.IsEnabled = false;
                DownloadAllUsersButton.IsEnabled = false;
                result = await GetAndDownloadUserProfiles(new Progress<UserDataProgress>(UpdateProgress), usernames);
                model.AddLog(result);
            }

            LogScrollViewer.ScrollToBottom();
            DownloadSelectedUserButton.IsEnabled = true;
            DownloadAllUsersButton.IsEnabled = true;
        }

        private async void UpdateProgress(UserDataProgress progress)
        {
            model.AddLog($"Progress:\n" +
                         $"Get basic user information: {progress.LoadBasicUserInfo}\n" +
                         $"Get full user information: {progress.LoadFullUserInfo}\n" +
                         $"Get user media: {progress.LoadUserMediaCount}, obtained {progress.LoadUserMediaCount} media\n");
        }
        
        private async Task<string> GetAndDownloadUserProfiles(IProgress<UserDataProgress> progress, params string[] usernames)
        {
            _instagramService = new InstagramService();
            _spreadsheetService = new SpreadsheetService();
            await _instagramService.TryAuthenticateAsync();
            if (!_instagramService.IsAuthenticated())
            {
                return "Failed to authenticate.\n";
            }
            
            try
            {
                // Get and write user data
                var userDataList = new List<UserData>();
                foreach (var username in usernames)
                {
                    var userData = await _instagramService.GetUserData(username, progress);
                    if (userData != null)
                    {
                        userDataList.Add(userData);
                    }
                }
                await new SpreadsheetService().WriteUserDataToCsvAsync(userDataList.ToArray());
                
                // Open newly created file
                var path = Path.GetFullPath($"{SpreadsheetService.OutputPath}.csv");
                Process.Start("explorer.exe", path);
                
                return $"Wrote information for {userDataList.Count}/{usernames.Length} users to {SpreadsheetService.OutputPath}.csv.";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        private void OnAddUser(object sender, RoutedEventArgs e)
        {
            model.AddUser();
            UserListBox.SelectedIndex = UserListBox.Items.Count - 1;
            model.UserInput = "";
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
