using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using StarWarsApp.Models;

namespace StarWarsApp.Services
{
    public sealed class FilmDatabase
    {
        private readonly string _connectionString;

        public FilmDatabase(string databasePath)
        {
            _connectionString = new SqliteConnectionStringBuilder { DataSource = databasePath }.ToString();
        }

        public void Initialize()
        {
            // Register the SQLite provider before opening a connection in UWP.
            SQLitePCL.Batteries.Init();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"CREATE TABLE IF NOT EXISTS Films (
                Title TEXT NOT NULL, EpisodeId INTEGER NOT NULL, OpeningCrawl TEXT, Director TEXT,
                Producer TEXT, ReleaseDate TEXT, Characters TEXT, Planets TEXT, Starships TEXT,
                Vehicles TEXT, Species TEXT)";
            command.ExecuteNonQuery();
        }

        public List<Film> GetFilms()
        {
            var films = new List<Film>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Title, EpisodeId, OpeningCrawl, Director, Producer, ReleaseDate, Characters, Planets, Starships, Vehicles, Species FROM Films ORDER BY EpisodeId";
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                films.Add(new Film
                {
                    Title = reader.GetString(0), EpisodeId = reader.GetInt32(1), OpeningCrawl = reader.IsDBNull(2) ? "" : reader.GetString(2),
                    Director = reader.IsDBNull(3) ? "" : reader.GetString(3), Producer = reader.IsDBNull(4) ? "" : reader.GetString(4),
                    ReleaseDate = reader.IsDBNull(5) ? "" : reader.GetString(5),
                    Characters = ReadList(reader, 6), Planets = ReadList(reader, 7), Starships = ReadList(reader, 8),
                    Vehicles = ReadList(reader, 9), Species = ReadList(reader, 10)
                });
            }
            return films;
        }

        public void SaveFilms(IEnumerable<Film> films)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();
            using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = @"INSERT INTO Films (Title, EpisodeId, OpeningCrawl, Director, Producer, ReleaseDate,
                Characters, Planets, Starships, Vehicles, Species) VALUES ($title,$episode,$crawl,$director,$producer,$date,$characters,$planets,$ships,$vehicles,$species)";
            foreach (var film in films)
            {
                command.Parameters.Clear();
                command.Parameters.AddWithValue("$title", film.Title); command.Parameters.AddWithValue("$episode", film.EpisodeId);
                command.Parameters.AddWithValue("$crawl", film.OpeningCrawl); command.Parameters.AddWithValue("$director", film.Director);
                command.Parameters.AddWithValue("$producer", film.Producer); command.Parameters.AddWithValue("$date", film.ReleaseDate);
                command.Parameters.AddWithValue("$characters", JsonConvert.SerializeObject(film.Characters));
                command.Parameters.AddWithValue("$planets", JsonConvert.SerializeObject(film.Planets));
                command.Parameters.AddWithValue("$ships", JsonConvert.SerializeObject(film.Starships));
                command.Parameters.AddWithValue("$vehicles", JsonConvert.SerializeObject(film.Vehicles));
                command.Parameters.AddWithValue("$species", JsonConvert.SerializeObject(film.Species));
                command.ExecuteNonQuery();
            }
            transaction.Commit();
        }

        private static List<string> ReadList(SqliteDataReader reader, int index) =>
            reader.IsDBNull(index) ? new List<string>() : JsonConvert.DeserializeObject<List<string>>(reader.GetString(index)) ?? new List<string>();
    }
}
