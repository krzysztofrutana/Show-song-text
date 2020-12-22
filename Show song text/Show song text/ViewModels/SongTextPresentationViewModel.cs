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

        // COMANDS START

        public ICommand SetFontSizeCommand { get; private set; }

        // COMMANDS END

        // CONSTRUCTOR START
        public SongTextPresentationViewModel()
        {
            songRepository = new SongRepository(DependencyService.Get<ISQLiteDb>());

            SetFontSizeCommand = new Command<double>(size => SetFontSize(size));

            MessagingCenter.Subscribe<PlaylistDetailViewModel, PlaylistViewModel>
           (this, Events.SendPlaylistToPresentation, OnPlaylistSended);

            MessagingCenter.Subscribe<ConnectionSettingsViewModel, Boolean>
            (this, Events.ConnectToServer, OnConnectToServer);

            MessagingCenter.Subscribe<AsyncClient, string>
            (this, Events.SendedText, OnTextRecive);

            asyncSocketListener = AsyncSocketListener.Instance;
            asyncClient = AsyncClient.Instance;

            FontSize = 20;
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
                    TitleOfSongWhenConnectedToServer = songTitleAndText[0];
                    TextOfSongWhenConnectedToServer = songTitleAndText[1];
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
                        int linesFitted = GetFitPageModel(leftText.ToArray(), testLabel, song.Title);
                        if (linesFitted != -1)
                        {
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

        // COMMAND METHOD START

        private void SetFontSize(double size)
        {
            FontSize = size;
        }
        // COMMAND METHOD END

        // OTHER METHOD START

        private Boolean CheckIfFit(Label label)
        {
            FontCalc lowerFontCalc = new FontCalc(label, FontSize, App.ScreenWidth * 0.9, App.ScreenHeight - 120);
            if (lowerFontCalc.TextHeight > App.ScreenHeight - 120)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private int GetFitPageModel(string[] textToFit, Label testLabel, string songTitle)
        {

            int songTextLines = textToFit.Length;

            int linesCount = songTextLines;
            PresentationPageModel presentationPageModel = new PresentationPageModel();
            for (int i = songTextLines - 1; i >= 0; i--)
            {
                string[] temp = new string[i + 1];
                for (int j = 0; j <= i; j++)
                {
                    temp[j] = textToFit[j];
                    temp[j] = temp[j].Trim();

                }
                testLabel.Text = string.Join(Environment.NewLine.ToString(), temp);
                Boolean isFit = CheckIfFit(testLabel);
                if (isFit)
                {
                    linesCount = i;
                    presentationPageModel.Title = songTitle;
                    presentationPageModel.Text = testLabel.Text;
                    Pages.Add(presentationPageModel);
                    break;

                }
            }
            if (!String.IsNullOrEmpty(presentationPageModel.Text) && !String.IsNullOrEmpty(presentationPageModel.Title))
            {
                return linesCount;
            }
            else
            {
                return -1;
            }


        }
        // OTHER METHOD END
    }
}
