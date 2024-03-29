﻿using ShowSongText.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShowSongText.Models
{
    public class PresentationPageModel : ViewModelBase
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public double FontSize { get; set; } = 20;
        public string SongKey { get; set; }
        public string Chords { get; set; }
       
    }
}
