using System.Collections.Generic;
using ShowSongText.Database.Models;

namespace ShowSongText.ViewModels.DTO
{
    public class SongViewModel : ViewModelBase
    {
        public int Id { get; set; }

        public SongViewModel() { }

        public SongViewModel(Song song)
        {
            Id = song.Id;
            Title = song.Title;
            Artist = song.Artist;
            Text = song.Text;
            Chords = song.Chords;
            Playlist = song.Playlists;
            IsCheckBoxVisible = song.IsCheckBoxVisible;
            Positions = song.Positions;
            SongKey = song.SongKey;


        }

        private List<Playlist> _playlist;
        public List<Playlist> Playlist
        {
            get { return _playlist; }
            set
            {
                _playlist = value;
                OnPropertyChanged(nameof(Playlist));
            }
        }

        private List<Position> _positions;
        public List<Position> Positions
        {
            get { return _positions; }
            set
            {
                _positions = value;
                OnPropertyChanged(nameof(Positions));
            }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        private string _artist;
        public string Artist
        {
            get { return _artist; }
            set
            {
                _artist = value;
                OnPropertyChanged(nameof(Artist));
            }
        }

        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                OnPropertyChanged(nameof(Text));
            }
        }

        private string _chords;
        public string Chords
        {
            get { return _chords; }
            set
            {
                _chords = value;
                OnPropertyChanged(nameof(Chords));
            }
        }

        private bool _isChecked;

        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                OnPropertyChanged(nameof(IsCheckBoxVisible));
            }
        }

        private bool _isCheckBoxVisible;

        public bool IsCheckBoxVisible
        {
            get { return _isCheckBoxVisible; }
            set
            {
                _isCheckBoxVisible = value;
                OnPropertyChanged(nameof(IsCheckBoxVisible));
            }
        }

        private string _songKey;

        public string SongKey
        {
            get { return _songKey; }
            set
            {
                _songKey = value;
                OnPropertyChanged(nameof(SongKey));
            }
        }




        public string FullName
        {
            get { return $"{Artist} {Title}"; }
        }

    }
}
