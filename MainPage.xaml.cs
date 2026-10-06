using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using StarWarsApp.Models;
using StarWarsApp.Services;

namespace StarWarsApp
{
    public sealed partial class MainPage : Page
    {
        private readonly SwapiService _swapi = new SwapiService();
        private List<Film> _films = new List<Film>();
        private FilmDatabase? _database;

        public MainPage() { InitializeComponent(); Loaded += MainPage_Loaded; }

        private async void MainPage_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var currentStep = "preparing the local film database";
            try
            {
                await ApplicationData.Current.LocalFolder.CreateFileAsync("starwars.db", CreationCollisionOption.OpenIfExists);
                var path = Path.Combine(ApplicationData.Current.LocalFolder.Path, "starwars.db");
                _database = new FilmDatabase(path);
                _database.Initialize();
                var films = _database.GetFilms();
                if (films.Count == 0)
                {
                    StatusText.Text = "Downloading films from SWAPI…";
                    currentStep = "downloading films from SWAPI";
                    films = await _swapi.GetFilmsAsync();
                    if (films.Count > 0)
                    {
                        currentStep = "saving films to the local database";
                        _database.SaveFilms(films);
                    }
                }
                currentStep = "displaying films";
                _films = films;
                FilmsList.Items.Clear();
                foreach (var film in _films) FilmsList.Items.Add(film.Title ?? "Untitled film");
                StatusText.Text = films.Count == 0 ? "No films were returned by SWAPI." : $"{films.Count} films loaded.";
                if (films.Count > 0) FilmsList.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                StatusText.Text = "Could not load films while " + currentStep + ".";
                await new MessageDialog("Films could not be loaded while " + currentStep + ".\n\n" + ex, "Loading error").ShowAsync();
            }
        }

        private void FilmsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FilmsList.SelectedIndex < 0 || FilmsList.SelectedIndex >= _films.Count) return;
            var film = _films[FilmsList.SelectedIndex];
            TitleText.Text = film.Title; EpisodeText.Text = "Episode ID: " + film.EpisodeId;
            DirectorText.Text = "Director: " + film.Director; ProducerText.Text = "Producer: " + film.Producer;
            ReleaseText.Text = "Release Date: " + film.ReleaseDate; CrawlText.Text = film.OpeningCrawl;
            AnalyzeButton.IsEnabled = true;
        }

        private void AnalyzeButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (FilmsList.SelectedIndex >= 0 && FilmsList.SelectedIndex < _films.Count)
                Frame.Navigate(typeof(FilmPage), _films[FilmsList.SelectedIndex]);
        }
    }
}
