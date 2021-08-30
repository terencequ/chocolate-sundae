using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ChocolateSundae.Displays.Models
{
    public class MainDisplayModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        #region Logs

        private string _logContents = "";
        public string LogContents
        {
            get => _logContents;
            set
            {
                _logContents = value;
                OnPropertyChanged();
            }
        }

        public void AddLog(string log)
        {
            LogContents += $"{DateTime.Now}: {log}\n";
        }

        #endregion

        #region Users

        private string _userInput = "";
        public string UserInput
        {
            get => _userInput;
            set
            {
                _userInput = value;
                OnPropertyChanged();
            }
        }

        private string _selectedUserText = "";
        public string SelectedUserText
        {
            get => _selectedUserText;
            set
            {
                _selectedUserText = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<string> _users = new ObservableCollection<string>();
        public ObservableCollection<string> Users
        {
            get => _users;
            set
            {
                _users = value;
                OnPropertyChanged();
            }
        }

        public void AddUser()
        {
            if(UserInput != "")
            {
                Users.Add(UserInput);
            }
        }

        public void RemoveUser(int index)
        {
            _users.RemoveAt(index);
        }

        #endregion

        protected virtual void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}