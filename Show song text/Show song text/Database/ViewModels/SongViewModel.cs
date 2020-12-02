﻿using Show_song_text.Database.Models;
using Show_song_text.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Show_song_text.Database.ViewModels
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
            Playlist = song.PlaylistID;
            IsCheckBoxVisible = song.IsCheckBoxVisible;




            
        }

        private int _playlist;
        public int Playlist
        {
            get { return _playlist; }
            set
            {
                _playlist = value;
                OnPropertyChanged(nameof(Playlist));
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

        private Boolean _isChecked;

        public Boolean IsChecked
        {
            get { return _isChecked; }
            set { _isChecked = value; }
        }

        private Boolean _isCheckBoxVisible;

        public Boolean IsCheckBoxVisible
        {
            get { return _isCheckBoxVisible; }
            set { _isCheckBoxVisible = value; }
        }



        public string FullName
        {
            get { return $"{Artist} {Title}"; }
        }

    }
}
