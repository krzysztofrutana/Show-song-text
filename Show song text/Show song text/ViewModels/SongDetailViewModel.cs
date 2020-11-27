using Show_song_text.Database.DOA;
using Show_song_text.Database.Models;
using Show_song_text.Database.ViewModels;
using Show_song_text.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Show_song_text.Utils;

namespace Show_song_text.ViewModels
{
    public class SongDetailViewModel : ViewModelBase
    {
        private readonly ISongDAO _songDAO;
        private readonly IPageService _pageService;

        public Song Song { get; private set; }
        public ICommand SaveCommand { get; private set; }

        public ICommand DeleteSongCommand { get; private set; }


        private string _title;
        public string Title
        {
            get
            {
                return _title;

            }
            set
            {
                _title = value;
                Song.Title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        private string _artist;
        public string Artist
        {
            get
            {
                return _artist;

            }
            set
            {
                _artist = value;
                Song.Artist = value;
                OnPropertyChanged(nameof(Artist));
            }
        }

        private string _text;
        public string Text
        {
            get
            {
                return _text;

            }
            set
            {
                _text = value;
                Song.Text = value;
                OnPropertyChanged(nameof(Text));
            }
        }

        private string _chords;
        public string Chords
        {
            get
            {
                return _chords;

            }
            set
            {
                _chords = value;
                Song.Chords = value;
                OnPropertyChanged(nameof(Chords));
            }
        }

        public Boolean IsBloatingButtonVisible { get; set; }


        public SongDetailViewModel(ISongDAO songDAO, IPageService pageService, SongViewModel viewModel = null)
        {
            _pageService = pageService;
            _songDAO = songDAO;

            SaveCommand = new Command(async () => await Save());
            DeleteSongCommand = new Command(async () => await DeleteSong());

            if (viewModel != null)
            {
                IsBloatingButtonVisible = true;
                Song = new Song
                {
                    Id = viewModel.Id,
                    Artist = viewModel.Artist,
                    Title = viewModel.Title,
                    Text = viewModel.Text,
                    Chords = viewModel.Chords,
                };
                Title = Song.Title;
                Artist = Song.Artist;
                Text = Song.Text;
                Chords = Song.Chords;
            }
            else
            {
                IsBloatingButtonVisible = false;
                Song = new Song
                {
                    Artist = Artist,
                    Title = Title,
                    Text = Text,
                    Chords = Chords
                };
            }
        }

        private async Task DeleteSong()
        {
            if (await _pageService.DisplayAlert("Ostrzeżenie", $"Jesteś pewny, że chcesz usunąć {Song.Artist} {Song.Title}?", "Tak", "Nie"))
            {

                var songToDelete = await _songDAO.GetSong(Song.Id);
                await _songDAO.DeleteSong(songToDelete);
            }
        }

        async Task Save()
        {
            if (Song == null || Song.Id == 0 )
            {
                await _songDAO.AddSong(Song);
                MessagingCenter.Send(this, Events.SongAdded, Song);
            }
            else if (Song.Id != 0)
            {
                if (String.IsNullOrWhiteSpace(Song.Title) && String.IsNullOrWhiteSpace(Song.Artist))
                {
                    await _pageService.DisplayAlert("Bład", "Wprowadź nazwę i autora.", "OK");
                    return;
                }
                else
                {
                    await _songDAO.UpdateSong(Song);
                    MessagingCenter.Send(this, Events.SongUpdated, Song);
                }
            }
            await _pageService.PopAsync();
        }
    }
}

