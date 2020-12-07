using Show_song_text.Database.Models;
using Show_song_text.Database.ViewModels;
using Show_song_text.Models;
using Show_song_text.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Net;
using System.Threading;
using System.IO;
using Show_song_text.PresentationServerUtilis;
using Show_song_text.Interfaces;

namespace Show_song_text.ViewModels
{
    public class SongTextPresentationViewModel : ViewModelBase
    {
        // PROPERTY START
        public PlaylistViewModel Playlist { get; private set; }
        private string _Name;
        public string Name
        {
            get
            {
                return _Name;

            }
            set
            {
                _Name = value;
                Playlist.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private List<Song> _SongsList;
        public List<Song> SongsList
        {
            get
            {
                return _SongsList;

            }
            set
            {
                _SongsList = value;
                Playlist.Songs = value;
                OnPropertyChanged(nameof(SongsList));
            }
        }

        

        private PresentationPageModel _CurrentPage;
        public PresentationPageModel CurrentPage
        {
            get
            {
                return _CurrentPage;

            }
            set
            {
                _CurrentPage = value;
                SendText(value.Text);
                OnPropertyChanged(nameof(CurrentPage));
            }
        }
        public ObservableCollection<PresentationPageModel> Pages { get; private set; }
           = new ObservableCollection<PresentationPageModel>();

        private int _position;
        public int Position { get { return _position; } set { _position = value; OnPropertyChanged(nameof(Position)); } }

        private Boolean _IsConnectedToServer;

        public Boolean IsConnectedToServer
        {
            get { return _IsConnectedToServer; }
            set { _IsConnectedToServer = value;
                OnPropertyChanged(nameof(IsConnectedToServer));
            }
        }

        private string _TextOfSongWhennConnectToServer;
        public string TextOfSongWhennConnectToServer
        {
            get
            {
                return _TextOfSongWhennConnectToServer;

            }
            set
            {
                _TextOfSongWhennConnectToServer = value;
                OnPropertyChanged(nameof(TextOfSongWhennConnectToServer));
            }
        }
        //PROPERTIES SECTION END

        // VARIABLE SECTION START
        private readonly IAsyncSocketListener asyncSocketListener;
        private readonly IAsyncClient asyncClient;
        // VARIABLE SECTION END

        // CONSTRUCTOR START
        public SongTextPresentationViewModel()
        {
            MessagingCenter.Subscribe<PlaylistDetailViewModel, PlaylistViewModel>
           (this, Events.SendPlaylistToPresentation, OnPlaylistSended);

            MessagingCenter.Subscribe<ConnectionSettingsViewModel, Boolean>
            (this, Events.ConnectToServer, OnConnectToServer);

            MessagingCenter.Subscribe<AsyncClient, string>
            (this, Events.SendedText, OnTextRecive);

            asyncSocketListener = AsyncSocketListener.Instance;
            asyncClient = AsyncClient.Instance;
        }
        // CONSTRUCTOR END

        // MESSAGING CENTER METHOD START
        void OnPlaylistSended(PlaylistDetailViewModel source, PlaylistViewModel playlistViewModel)
        {
            Playlist = playlistViewModel;
            Name = Playlist.Name;
            SongsList = Playlist.Songs;
            PreparePresentation();
        }

        void OnConnectToServer(ConnectionSettingsViewModel source, Boolean isConnected)
        {
            IsConnectedToServer = true;
            asyncClient.Receive();
        }

        void OnTextRecive(AsyncClient source, string text)
        {
            TextOfSongWhennConnectToServer = text;
        }
        // MESSAGING CENTER METHOD END

        // PROPERTY METHOD START
        void PreparePresentation()
        {
            foreach(Song song in SongsList)
            {
                string[] result = song.Text.Split(System.Environment.NewLine.ToCharArray());
                int songTextLines = result.Length;
                if(songTextLines <= 25)
                {
                    string[] temp = new string[songTextLines];
                        for (int j = 0; j < songTextLines; j++)
                        {
                                temp[j] = result[j];

                        }
                        string pageText = string.Join(Environment.NewLine.ToString(), temp);
                        var t = new PresentationPageModel() { Title = song.Artist + " - " + song.Title, Text = pageText };
                        try
                        {
                            Pages.Add(t);
                        }
                        catch (Exception e)
                        {

                            Console.WriteLine(e.Message);
                        }
                }
                else
                {
                    for (int i = 0; i < songTextLines; i++)
                    {
                        if (i % 20 == 0)
                        {
                            string[] temp = new string[20];
                            for (int j = 0; j < 20; j++)
                            {
                                if (i + j < songTextLines)
                                {
                                    temp[j] = result[i + j];
                                }

                            }
                            string pageText = string.Join(Environment.NewLine.ToString(), temp);
                            var t = new PresentationPageModel() { Title = song.Artist + " - " + song.Title, Text = pageText };
                            try
                            {
                                Pages.Add(t);
                            }
                            catch (Exception e)
                            {

                                Console.WriteLine(e.Message);
                            }

                        }
                    }
                }
            }
        }
        // PROPERTY METHOD END

        // TEXT SHARE METHOD START

        void SendText(string text)
        {
            if(text != "")
            asyncSocketListener.Send(1, text, false);
        }

        // TEXT SHARE METHOD END

    }
}
