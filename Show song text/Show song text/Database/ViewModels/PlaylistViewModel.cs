using Show_song_text.Database.Models;
using Show_song_text.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Show_song_text.Database.ViewModels
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

        private Boolean _customSongsOrder;
        public Boolean CustomSongsOrder
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
