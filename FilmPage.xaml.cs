using System;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using StarWarsApp.Models;
using StarWarsApp.Services;

namespace StarWarsApp
{
    public sealed partial class FilmPage : Page
    {
        private readonly SwapiService _swapi = new SwapiService();

        public FilmPage() { InitializeComponent(); }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is not Film film) return;
            FilmTitleText.Text = film.Title;
            StatusText.Text = "Loading related resources from SWAPI…";
            try
            {
                // Categories load independently so a slow endpoint does not delay the others.
                var characters = _swapi.GetNamesAsync(film.Characters);
                var planets = _swapi.GetNamesAsync(film.Planets);
                var starships = _swapi.GetNamesAsync(film.Starships);
                var vehicles = _swapi.GetNamesAsync(film.Vehicles);
                var species = _swapi.GetNamesAsync(film.Species);
                await Task.WhenAll(characters, planets, starships, vehicles, species);
                AddNames(CharactersList, characters.Result);
                AddNames(PlanetsList, planets.Result);
                AddNames(StarshipsList, starships.Result);
                AddNames(VehiclesList, vehicles.Result);
                AddNames(SpeciesList, species.Result);
                StatusText.Text = "Resource names loaded.";
            }
            catch (Exception ex)
            {
                StatusText.Text = "Some resources could not be loaded.";
                await new MessageDialog("Related resources could not be loaded. Check your connection and try again.\n\n" + ex.Message, "SWAPI error").ShowAsync();
            }
        }

        private void BackButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (Frame.CanGoBack) Frame.GoBack();
        }

        private static void AddNames(ListView list, System.Collections.Generic.IEnumerable<string> names)
        {
            list.Items.Clear();
            foreach (var name in names) list.Items.Add(name);
        }
    }
}
