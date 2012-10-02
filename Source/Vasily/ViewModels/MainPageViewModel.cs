using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Windows.Networking.Sockets;
using Vasily.Models;

namespace Vasily.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public const string DefaultHostnamePlaceholder = "hostnamehere";

        public MainPageViewModel()
        {
            SetDefaults();
        }

        private void SetDefaults()
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

        public const string CanConnectPropertyName = "CanConnect";
        public bool CanConnect
        {
            get
            {
                int portNumber = 0;
                return 
                    !ConnectionInProgress && 
                    HostName.Trim().Length >= 3 && 
                    0 != String.Compare(DefaultHostnamePlaceholder, HostName, StringComparison.OrdinalIgnoreCase) && 
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
                RaisePropertyChanged(CanConnectPropertyName);
                ConnectCommand.RaiseCanExecuteChanged();
                AddItemCommand.RaiseCanExecuteChanged();
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
                RaisePropertyChanged(CanConnectPropertyName);
                ConnectCommand.RaiseCanExecuteChanged();
                AddItemCommand.RaiseCanExecuteChanged();
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

        public void LoadState(MainPageState state)
        {
            if (!String.IsNullOrWhiteSpace(state.HostName))
            {
                HostName = state.HostName;
            }

            if (!String.IsNullOrWhiteSpace(state.PortNumber))
            {
                PortNumber = state.PortNumber;
            }
        }

        public async Task LoadFavorites()
        {
            var repo = new FavoritesRepository();
            var items = await repo.LoadAsync();
            Favorites = new ObservableCollection<Favorite>(items);
        }

        public MainPageState SaveState()
        {
            var state = new MainPageState()
            {
                HostName = this.HostName,
                PortNumber = this.PortNumber
            };

            return state;
        }

        public const string SelectedFavoritePropertyName = "SelectedFavorite";
        private Favorite _selectedFavorite= null;
        public Favorite SelectedFavorite
        {
            get { return _selectedFavorite; }
            set
            {
                Set(SelectedFavoritePropertyName, ref _selectedFavorite, value);

                if (null == _selectedFavorite)
                {
                    SetDefaults();
                }
                else
                {
                    HostName = _selectedFavorite.HostName;
                    PortNumber = _selectedFavorite.PortNumber;
                }

                DeleteSelectedItemCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged(IsFavoriteSelectedPropertyName);
            }
        }

        public const string IsFavoriteSelectedPropertyName = "IsFavoriteSelected";
        public bool IsFavoriteSelected
        {
            get { return _selectedFavorite != null; }
        }

        private RelayCommand _deleteSelectedItemCommand;
        public RelayCommand DeleteSelectedItemCommand
        {
            get
            {
                return _deleteSelectedItemCommand
                    ?? (_deleteSelectedItemCommand = new RelayCommand(
                        async () => await DeleteSelectedItemAsync(),
                        () => SelectedFavorite != null));
            }
        }

        public async Task DeleteSelectedItemAsync()
        {
            Favorites.Remove(SelectedFavorite);

            var repo = new FavoritesRepository();
            bool saveOk = await repo.SaveAsync(Favorites.ToList());
        }

        public const string FavoritesPropertyName = "Favorites";
        private ObservableCollection<Favorite> _favorites = new ObservableCollection<Favorite>();
        public ObservableCollection<Favorite> Favorites
        {
            get
            {
                return _favorites;
            }
            set
            {
                Set(FavoritesPropertyName, ref _favorites, value);
            }
        }

        private RelayCommand _addItemCommand;
        public RelayCommand AddItemCommand
        {
            get
            {
                return _addItemCommand
                    ?? (_addItemCommand = new RelayCommand(
                        async () => await AddItemAsync(),
                        () => CanConnect));
            }
        }

        public async Task AddItemAsync()
        {
            var newItem = new Favorite()
                              {
                                  HostName = this.HostName,
                                  PortNumber = this.PortNumber
                              };

            Favorites.Add(newItem);

            var repo = new FavoritesRepository();
            bool saveOk = await repo.SaveAsync(Favorites.ToList());
        }
    }
}
