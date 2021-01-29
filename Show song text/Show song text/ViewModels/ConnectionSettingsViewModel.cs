using Show_song_text.Helpers;
using Show_song_text.Interfaces;
using Show_song_text.PresentationServerUtilis;
using Show_song_text.Utils;
using Show_song_text.Views;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Show_song_text.ViewModels
{
    public class ConnectionSettingsViewModel : ViewModelBase
    {
        // VARIABLE START
        private readonly IPageService _pageService;
        private readonly IAsyncSocketListener asyncSocketListener;
        private readonly IAsyncClient asyncClient;
        // VARIABLE END

        // PROPERTY START
        private string _IPServerAdress;

        public string IPServerAdress
        {
            get { return _IPServerAdress; }
            set
            {
                _IPServerAdress = value;
                OnPropertyChanged(nameof(IPServerAdress));
            }
        }
        private int _ServerPort;

        public int ServerPort
        {
            get { return _ServerPort; }
            set
            {
                _ServerPort = value;
                OnPropertyChanged(nameof(ServerPort));
            }
        }


        private Boolean _ServerSectionVisibility;

        public Boolean ServerSectionVisibility
        {
            get { return _ServerSectionVisibility; }
            set
            {
                _ServerSectionVisibility = value;
                OnPropertyChanged(nameof(ServerSectionVisibility));
            }
        }

        private Boolean _ServerIsRunning;

        public Boolean ServerIsRunning
        {
            get { return _ServerIsRunning; }
            set
            {
                _ServerIsRunning = value;
                OnPropertyChanged(nameof(ServerIsRunning));
            }
        }

        private int _ClientConnectedCount;

        public int ClientConnectedCount
        {
            get { return _ClientConnectedCount; }
            set
            {
                _ClientConnectedCount = value;
                OnPropertyChanged(nameof(ClientConnectedCount));
            }
        }


        private Boolean _ClientSectionVisibility;

        public Boolean ClientSectionVisibility
        {
            get { return _ClientSectionVisibility; }
            set
            {
                _ClientSectionVisibility = value;
                OnPropertyChanged(nameof(ClientSectionVisibility));
            }
        }

        private string _IpAdressToConnect;

        public string IpAdressToConnect
        {
            get { return _IpAdressToConnect; }
            set
            {
                _IpAdressToConnect = value;
                OnPropertyChanged(nameof(IpAdressToConnect));
            }
        }

        private int? _PortToConnect = null;

        public int? PortToConnect
        {
            get { return _PortToConnect; }
            set
            {
                _PortToConnect = value;
                OnPropertyChanged(nameof(PortToConnect));
            }
        }
        private Boolean _IsConnectToServer;

        public Boolean IsConnectToServer
        {
            get { return _IsConnectToServer; }
            set
            {
                _IsConnectToServer = value;
                OnPropertyChanged(nameof(IsConnectToServer));
            }
        }

        // PROPERTY END

        // COMMANDS START
        public ICommand StartServerCommand { get; private set; }
        public ICommand StopServerCommand { get; private set; }
        public ICommand ConnectToServerCommand { get; private set; }
        public ICommand StartPresentationForCLientCommand { get; private set; }
        public ICommand DisconnectWithServerCommand { get; private set; }
        // COMMANDS END

        // CONSTRUCTOR START
        public ConnectionSettingsViewModel()
        {
            _pageService = new PageService();
            asyncSocketListener = AsyncSocketListener.Instance;
            asyncClient = AsyncClient.Instance;


            if (asyncClient.IsConnected() == true)
            {
                Settings.ClientIsConnected = true;
            }
            else
            {
                Settings.ClientIsConnected = false;
            }


            ServerIsRunning = Settings.ServerIsRunning;
            IsConnectToServer = Settings.ClientIsConnected;
            ServerPort = Settings.ServerPort ;
            ClientConnectedCount = Settings.ServerClientConnectedQty;

            StartServerCommand = new Command(() => StartServer());
            StopServerCommand = new Command(() => StopServer());
            ConnectToServerCommand = new Command(() => ConnectToServer());
            StartPresentationForCLientCommand = new Command(async () => await StartPresentationForCLient());
            DisconnectWithServerCommand = new Command(() => DisconnectWithServer());

            
            IPServerAdress = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].ToString();

            

            MessagingCenter.Subscribe<AsyncSocketListener, Boolean>
            (this, Events.SendPlaylist, OnServerIsRunning);

            MessagingCenter.Subscribe<AsyncSocketListener, int>
            (this, Events.ClientConnected, OnClientConnected);

            MessagingCenter.Subscribe<AsyncSocketListener, int>
            (this, Events.ClientDisconnected, OnClientDisconnected);

            MessagingCenter.Subscribe<AsyncClient, Boolean>
            (this, Events.ConnectToServer, OnConnectToServer);

        }
        // CONSTRUCTOR END

        // COMMAND METHOD START
        private void StartServer()
        {
            new Thread(new ThreadStart(delegate
            {
                Settings.ServerIsRunning = true;
                Settings.ServerPort = ServerPort;
                asyncSocketListener.StartListening(ServerPort);
            })).Start();
        }

        private void StopServer()
        {
            Settings.ServerIsRunning = false;
            Settings.ServerPort = 11000;
            asyncSocketListener.StopListening();

        }

        private async void ConnectToServer()
        {
            if (PortToConnect != null)
            {
                Settings.ConnectedServerIP = IpAdressToConnect;
                Settings.ConnectedServerPort = (int)PortToConnect;
                asyncClient.StartClient((int)PortToConnect, IPAddress.Parse(IpAdressToConnect));
                asyncClient.Receive();
            }
            else
            {
                await _pageService.DisplayAlert("Puste pola", "Proszę wpisać numer portu", "OK");
            }       
        }

        async Task StartPresentationForCLient()
        {
            await _pageService.ChangePageAsync(new SongTextPresentationView());
            MessagingCenter.Send(this, Events.ConnectToServer, true);
        }
        private async void DisconnectWithServer()
        {
            asyncClient.Send("<EOC>", true);
            asyncClient.DisconnectWithServer();
        }
        // COMMAND METHOD END

        // MESSAGING CENTER METHOD START
        private void OnServerIsRunning(AsyncSocketListener source, Boolean isRunnning)
        {
            ServerIsRunning = isRunnning;
        }

        private void OnClientConnected(AsyncSocketListener source, int clientConnected)
        {
            ClientConnectedCount = Settings.ServerClientConnectedQty;
        }

        private void OnClientDisconnected(AsyncSocketListener source, int clientDisconnected)
        {
            ClientConnectedCount = Settings.ServerClientConnectedQty;
        }

        private void OnConnectToServer(AsyncClient source, Boolean isConnected)
        {
            IsConnectToServer = isConnected;
        }
        // MESSAGING CENTER METHOD END
    }
}
