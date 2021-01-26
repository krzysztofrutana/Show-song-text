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
using Show_song_text.Database.Repository;
using System.Linq;
using Show_song_text.Database.Persistence;
using System.Windows.Input;
using XamarinLabelFontSizer;
using Show_song_text.Views;
using Show_song_text.Helpers;

namespace Show_song_text.ViewModels
{
    public class SongTextPresentationViewModel : ViewModelBase
    {
        // PROPERTY START
        public PlaylistViewModel Playlist { get; private set; }
        public SongViewModel Song { get; private set; }

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
                OnPropertyChanged(nameof(Name));
            }
        }

        public ObservableCollection<SongViewModel> SongsList { get; private set; } = new ObservableCollection<SongViewModel>();

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
                String textToSend = value.Title + '|' + value.Text;
                SendText(textToSend);
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
            set
            {
                _IsConnectedToServer = value;
                OnPropertyChanged(nameof(IsConnectedToServer));
            }
        }

        private string _TextOfSongWhennConnectToServer;
        public string TextOfSongWhenConnectedToServer
        {
            get
            {
                return _TextOfSongWhennConnectToServer;

            }
            set
            {
                _TextOfSongWhennConnectToServer = value;
                OnPropertyChanged(nameof(TextOfSongWhenConnectedToServer));
            }
        }

        private string _TitleOfSongWhennConnectToServer;
        public string TitleOfSongWhenConnectedToServer
        {
            get
            {
                return _TitleOfSongWhennConnectToServer;

            }
            set
            {
                _TitleOfSongWhennConnectToServer = value;
                OnPropertyChanged(nameof(TitleOfSongWhenConnectedToServer));
            }
        }

        private double _FontSize;

        public double FontSize
        {
            get { return _FontSize; }
            set
            {
                _FontSize = value;
                OnPropertyChanged(nameof(FontSize));
            }
        }

        //PROPERTIES SECTION END

        // VARIABLE SECTION START
        private readonly IAsyncSocketListener asyncSocketListener;
        private readonly IAsyncClient asyncClient;
        private readonly SongRepository songRepository;
        // VARIABLE SECTION END


        // CONSTRUCTOR START
        public SongTextPresentationViewModel()
        {
            songRepository = new SongRepository(DependencyService.Get<ISQLiteDb>());

            MessagingCenter.Subscribe<PlaylistDetailViewModel, PlaylistViewModel>
           (this, Events.SendPlaylistToPresentation, OnPlaylistSended);

            MessagingCenter.Subscribe<ConnectionSettingsViewModel, Boolean>
            (this, Events.ConnectToServer, OnConnectToServer);

            MessagingCenter.Subscribe<AsyncClient, string>
            (this, Events.SendedText, OnTextRecive);

            MessagingCenter.Subscribe<SongListViewModel, SongViewModel>
           (this, Events.SendSongToPresentation, OnSongSended);
            asyncSocketListener = AsyncSocketListener.Instance;
            asyncClient = AsyncClient.Instance;

            FontSize = Double.Parse(Settings.FontSize);
        }
        // CONSTRUCTOR END

        // MESSAGING CENTER METHOD START
        async void OnPlaylistSended(PlaylistDetailViewModel source, PlaylistViewModel playlistViewModel)
        {
            Playlist = playlistViewModel;
            Name = Playlist.Name;
            ObservableCollection<SongViewModel> tempList = new ObservableCollection<SongViewModel>();
            foreach (Song songTemp in Playlist.Songs)
            {
                if (Playlist.CustomSongsOrder)
                {
                    Song tempSong = await songRepository.GetSongWithChildren(songTemp.Id);
                    tempSong.IsCheckBoxVisible = false;
                    tempList.Add(new SongViewModel(tempSong));
                }
                else
                {
                    songTemp.IsCheckBoxVisible = false;
                    tempList.Add(new SongViewModel(songTemp));
                }
            }
            if (Playlist.CustomSongsOrder)
            {
                tempList = new ObservableCollection<SongViewModel>(tempList.OrderBy(s => s.Positions.Where(p => p.PlaylistId == Playlist.Id).FirstOrDefault().PositionOnPlaylist));
            }

            foreach (SongViewModel song in tempList)
            {
                SongsList.Add(song);
            }
            PreparePresentation();
        }

        async void OnSongSended(SongListViewModel source, SongViewModel songViewModel)
        {
            Song = songViewModel;
            
            SongsList.Add(Song);

            PreparePresentation();
        }

        void OnConnectToServer(ConnectionSettingsViewModel source, Boolean isConnected)
        {
            IsConnectedToServer = true;
            asyncClient.Receive();
        }

        void OnTextRecive(AsyncClient source, string text)
        {
            if (text != "")
            {
                string[] songTitleAndText = text.Split('|');
                if (songTitleAndText != null)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Label testLabel = SongTextPresentationView.GetGhostLabelInstance();
                        TitleOfSongWhenConnectedToServer = songTitleAndText[0];
                        TextOfSongWhenConnectedToServer = songTitleAndText[1];
                        testLabel.Text = TextOfSongWhenConnectedToServer;
                        testLabel.FontSize = FontSize;
                        // Calculate the height of the rendered text.
                        FontCalc lowerFontCalc = new FontCalc(testLabel, 10, App.ScreenWidth);
                        FontCalc upperFontCalc = new FontCalc(testLabel, 100, App.ScreenWidth);

                        while (upperFontCalc.FontSize - lowerFontCalc.FontSize > 1)
                        {
                            // Get the average font size of the upper and lower bounds.
                            double fontSize = (lowerFontCalc.FontSize + upperFontCalc.FontSize) / 2;

                            // Check the new text height against the container height.
                            FontCalc newFontCalc = new FontCalc(testLabel, fontSize, App.ScreenWidth);

                            if (newFontCalc.TextHeight > App.ScreenHeight - 120)
                            {
                                upperFontCalc = newFontCalc;
                            }
                            else
                            {
                                lowerFontCalc = newFontCalc;
                            }

                        }
                        FontSize = lowerFontCalc.FontSize;
                    });
                }
            }
        }
        // MESSAGING CENTER METHOD END

        // PROPERTY METHOD START
        public void PreparePresentation()
        {
            if (SongsList.Count > 0)
            {
                Label testLabel = SongTextPresentationView.GetGhostLabelInstance();
                foreach (SongViewModel song in SongsList)
                {
                    string[] allText = song.Text.Split(Environment.NewLine.ToCharArray());
                    int linesLeft = allText.Length;
                    List<string> leftText = allText.ToList();



                    while (linesLeft > 0)
                    {
                        int linesFitted = PresentationPageHelper.GetFitPageModel(leftText.ToArray(), testLabel, song.Title, FontSize);
                        if (linesFitted != -1)
                        {
                            Pages.Add(PresentationPageHelper.presentationPageModel);
                            linesLeft -= linesFitted +1;
                            leftText.RemoveRange(0, linesFitted + 1);
                        }
                    }

            
                }
            }



        }
        // PROPERTY METHOD END

        // TEXT SHARE METHOD START

        void SendText(string text)
        {
            if (text != "")
            {
                asyncSocketListener.Send(text, false);
            }

        }

        // TEXT SHARE METHOD END

    }
}
