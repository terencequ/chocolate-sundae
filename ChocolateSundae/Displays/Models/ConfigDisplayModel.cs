using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ChocolateSundae.Displays.Models
{
    public class ConfigDisplayModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        #region Username

        private string _username = "";
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Password

        private string _password = "";
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Status

        private string _status = "";
        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged();
            }
        }

        private Brush _statusColor = Brushes.Black;
        public Brush StatusColor
        {
            get => _statusColor;
            set
            {
                _statusColor = value;
                OnPropertyChanged();
            }
        }

        public void SetStatusInfo(string message)
        {
            Status = message;
            StatusColor = Brushes.DarkGray;
        }

        public void SetStatusSuccess(string message)
        {
            Status = message;
            StatusColor = Brushes.Green;
        }

        public void SetStatusError(string message)
        {
            Status = message;
            StatusColor = Brushes.Red;
        }

        public void ResetStatus()
        {
            Status = "";
            StatusColor = Brushes.Black;
        }

        #endregion

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
