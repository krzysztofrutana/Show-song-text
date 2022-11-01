using System.Collections.Generic;
using ShowSongText.Database.Models;

namespace ShowSongText.ViewModels.DTO
{
    public class PlaylistViewModel : ViewModelBase
    {
        public int Id { get; set; }

        public PlaylistViewModel(Playlist playlist)
        {
            Id = playlist.Id;
            Name = playlist.Name;
            Songs = playlist.Songs;
            CustomSongsOrder = playlist.CustomSongsOrder;

        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private List<Song> _songs;
        public List<Song> Songs
        {
            get { return _songs; }
            set
            {
                _songs = value;
                OnPropertyChanged(nameof(Songs));
            }
        }

        private bool _customSongsOrder;
        public bool CustomSongsOrder
        {
            get { return _customSongsOrder; }
            set
            {
                _customSongsOrder = value;
                OnPropertyChanged(nameof(CustomSongsOrder));
            }
        }

    }
}
