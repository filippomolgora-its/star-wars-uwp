using System.Collections.Generic;
using Newtonsoft.Json;

namespace StarWarsApp.Models
{
    public sealed class Film
    {
        [JsonProperty("title")] public string Title { get; set; } = "";
        [JsonProperty("episode_id")] public int EpisodeId { get; set; }
        [JsonProperty("opening_crawl")] public string OpeningCrawl { get; set; } = "";
        [JsonProperty("director")] public string Director { get; set; } = "";
        [JsonProperty("producer")] public string Producer { get; set; } = "";
        [JsonProperty("release_date")] public string ReleaseDate { get; set; } = "";
        [JsonProperty("characters")] public List<string> Characters { get; set; } = new();
        [JsonProperty("planets")] public List<string> Planets { get; set; } = new();
        [JsonProperty("starships")] public List<string> Starships { get; set; } = new();
        [JsonProperty("vehicles")] public List<string> Vehicles { get; set; } = new();
        [JsonProperty("species")] public List<string> Species { get; set; } = new();
    }

    public sealed class FilmListResponse
    {
        [JsonProperty("results")] public List<Film> Results { get; set; } = new();
    }
}
