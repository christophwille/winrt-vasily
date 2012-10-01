using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Windows.Networking.Sockets;

namespace Vasily.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public const string DefaultHostnamePlaceholder = "hostnamehere";

        public MainPageViewModel()
        {
            HostName = DefaultHostnamePlaceholder;
            PortNumber = "80";
        }

        private RelayCommand _connectCommand;
        public RelayCommand ConnectCommand
        {
            get
            {
                return _connectCommand
                    ?? (_connectCommand = new RelayCommand(
                        async () => await ConnectAsync(),
                        () => CanConnect));
            }
        }

        public async Task ConnectAsync()
        {
            if (ConnectionInProgress) return;

            ConnectionAttemptInformation = "";
            ConnectionInProgress = true;

            try
            {
                using (var tcpClient = new StreamSocket())
                {
                    await tcpClient.ConnectAsync(
                        new Windows.Networking.HostName(HostName),
                        PortNumber,
                        SocketProtectionLevel.PlainSocket);

                    var localIp = tcpClient.Information.LocalAddress.DisplayName;
                    var remoteIp = tcpClient.Information.RemoteAddress.DisplayName;

                    ConnectionAttemptInformation = String.Format("Success, remote server contacted at IP address {0}",
                                                                 remoteIp);
                    tcpClient.Dispose();
                }
            }
            catch (Exception ex)
            {
                if (ex.HResult == -2147013895)
                {
                    ConnectionAttemptInformation = "Error: No such host is known";
                }
                else if (ex.HResult == -2147014836)
                {
                    ConnectionAttemptInformation = "Error: Timeout when connecting (check hostname and port)";
                }
                else
                {
                    ConnectionAttemptInformation = "Error: Exception returned from network stack: " + ex.Message;
                }
            }
            finally
            {
                ConnectionInProgress = false;
            }
        }

        public bool CanConnect
        {
            get
            {
                int portNumber = 0;
                return 
                    !ConnectionInProgress && 
                    HostName.Trim().Length >= 3 && 
                    Int32.TryParse(PortNumber, out portNumber);
            }
        }

        public const string HostNamePropertyName = "HostName";
        private string _hostName = "";
        public string HostName
        {
            get
            {
                return _hostName;
            }
            set
            {
                Set(HostNamePropertyName, ref _hostName, value);
                ConnectCommand.RaiseCanExecuteChanged();
            }
        }

        public Action<string> UpdateBoundPortNumberProperty
        {
            get { return new Action<string>((value) => PortNumber = value); }
        }

        public const string PortNumberPropertyName = "PortNumber";
        private string _portNumber = "";
        public string PortNumber
        {
            get
            {
                return _portNumber;
            }
            set
            {
                Set(PortNumberPropertyName, ref _portNumber, value);
                ConnectCommand.RaiseCanExecuteChanged();
            }
        }

        public Action<string> UpdateBoundHostNameProperty
        {
            get { return new Action<string>((value) => HostName = value); }
        }

        public const string ConnectionInProgressPropertyName = "ConnectionInProgress";
        private bool _connectionInProgress = false;
        public bool ConnectionInProgress
        {
            get
            {
                return _connectionInProgress;
            }
            set
            {
                Set(ConnectionInProgressPropertyName, ref _connectionInProgress, value);
            }
        }

        public const string ConnectionAttemptInformationPropertyName = "ConnectionAttemptInformation";
        private string _resultInformation = "";
        public string ConnectionAttemptInformation
        {
            get
            {
                return _resultInformation;
            }
            set
            {
                Set(ConnectionAttemptInformationPropertyName, ref _resultInformation, value);
            }
        }
        
    }
}
