using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ShowSongText.Abstraction;
using ShowSongText.Database.Abstraction;
using ShowSongText.Database.Models;
using ShowSongText.Database.Repository;
using ShowSongText.Helpers;
using ShowSongText.Models;
using ShowSongText.PresentationServerUtilis;
using ShowSongText.Resources.Languages;
using ShowSongText.Utils;
using ShowSongText.ViewModels.DTO;
using ShowSongText.Views;
using Xamarin.Forms;
using XamarinLabelFontSizer;

namespace ShowSongText.ViewModels
{
    public class SongTextPresentationViewModel : ViewModelBase
    {
        #region Property
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

        private bool showChords;

        public bool ShowChords
        {
            get { return showChords; }
            set
            {
                showChords = value;
                OnPropertyChanged(nameof(ShowChords));
            }
        }



        #endregion

        #region Variable
        private readonly IAsyncSocketListener asyncSocketListener;
        private readonly IAsyncClient asyncClient;
        private readonly SongRepository songRepository;
        #endregion

        #region Constructor
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

            ShowChords = Settings.ShowChords;
        }
        #endregion

        #region Message center method
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
            MessagingCenter.Unsubscribe<PlaylistDetailViewModel, PlaylistViewModel>
           (this, Events.SendPlaylistToPresentation);
        }

        void OnSongSended(SongListViewModel source, SongViewModel songViewModel)
        {
            Song = songViewModel;

            SongsList.Add(Song);

            PreparePresentation();

            MessagingCenter.Unsubscribe<SongListViewModel, SongViewModel>
           (this, Events.SendSongToPresentation);
        }

        void OnConnectToServer(ConnectionSettingsViewModel source, Boolean isConnected)
        {
            IsConnectedToServer = true;

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
                        FontCalc checkIfShortText = new FontCalc(testLabel, 25, App.ScreenWidth);
                        if (checkIfShortText.TextHeight < App.ScreenHeight * 0.5)
                        {
                            FontSize = checkIfShortText.FontSize;
                        }
                        else
                        {
                            // Calculate the height of the rendered text.
                            FontCalc lowerFontCalc = new FontCalc(testLabel, 10, App.ScreenWidth);
                            FontCalc upperFontCalc = new FontCalc(testLabel, 100, App.ScreenWidth);

                            while (upperFontCalc.FontSize - lowerFontCalc.FontSize > 1)
                            {
                                // Get the average font size of the upper and lower bounds.
                                double fontSize = (lowerFontCalc.FontSize + upperFontCalc.FontSize) / 2;

                                // Check the new text height against the container height.
                                FontCalc newFontCalc = new FontCalc(testLabel, fontSize, App.ScreenWidth);

                                if (newFontCalc.TextHeight > App.ScreenHeight * 0.85)
                                {
                                    upperFontCalc = newFontCalc;
                                }
                                else
                                {
                                    lowerFontCalc = newFontCalc;
                                }

                            }
                            FontSize = lowerFontCalc.FontSize;
                        }
                    });
                }
            }
        }
        #endregion

        #region Property methods
        public void PreparePresentation()
        {
            if (SongsList.Count > 0)
            {
                foreach (SongViewModel song in SongsList)
                {
                    if (ShowChords)
                    {
                        if (!String.IsNullOrWhiteSpace(song.Chords))
                        {
                            CreatePagesWithChords(song);
                        }
                        else
                        {
                            CreatePagesWithoutChords(song);
                        }
                    }
                    else
                    {
                        CreatePagesWithoutChords(song);

                    }
                }
            }

        }

        private void CreatePagesWithoutChords(SongViewModel song)
        {
            if (String.IsNullOrWhiteSpace(Song.Text))
            {
                Pages.Add(new PresentationPageModel()
                {
                    Title = song.Title,
                    Text = AppResources.SongTextPresentation_EmptyText,
                    SongKey = song.SongKey,
                    FontSize = FontSize
                });
                return;
            }

            Label testLabel = SongTextPresentationView.GetGhostLabelInstance();

            string[] allText = song.Text.Split(Environment.NewLine.ToCharArray());
            int linesLeft = allText.Length;
            List<string> leftText = allText.ToList();

            while (linesLeft > 0)
            {
                int linesFitted = PresentationPageHelper.GetFitPageModel(leftText.ToArray(), testLabel, song.Title, FontSize, song.SongKey);
                if (linesFitted != -1)
                {
                    Pages.Add(PresentationPageHelper.PresentationPageModel);
                    linesLeft -= linesFitted + 1;
                    leftText.RemoveRange(0, linesFitted + 1);
                }
            }

        }


        private void CreatePagesWithChords(SongViewModel song)
        {
            Label testLabel = SongTextPresentationView.GetGhostLabelInstance();
            string[] allText = song.Text.Split(Environment.NewLine.ToCharArray());
            string[] allChords = song.Chords.Split(Environment.NewLine.ToCharArray());
            int linesLeft = allText.Length;
            List<string> leftText = allText.ToList();
            List<string> leftChords = allChords.ToList();



            while (linesLeft > 0)
            {
                int linesFitted = PresentationPageHelper.GetFitPageModel(leftText.ToArray(), testLabel, song.Title, FontSize, song.SongKey, addChords: true, chordsToFit: leftChords.ToArray());
                if (linesFitted != -1)
                {
                    Pages.Add(PresentationPageHelper.PresentationPageModel);
                    linesLeft -= linesFitted;
                    leftText.RemoveRange(0, linesFitted);
                    leftChords.RemoveRange(0, linesFitted);
                }
            }

        }
        #endregion

        #region Share text method
        void SendText(string text)
        {
            if (text != "")
            {
                asyncSocketListener.Send(text, false);
            }

        }
        #endregion

    }
}
