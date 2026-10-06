using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StarWarsApp.Models;

namespace StarWarsApp.Services
{
    public sealed class SwapiService
    {
        private static readonly HttpClient Client = new HttpClient();
        private const string BaseUrl = "https://swapi.dev/api/";

        public async Task<List<Film>> GetFilmsAsync()
        {
            var json = await Client.GetStringAsync(BaseUrl + "films/");
            return JsonConvert.DeserializeObject<FilmListResponse>(json)?.Results ?? new List<Film>();
        }

        public async Task<List<string>> GetNamesAsync(IEnumerable<string> urls)
        {
            var names = new List<string>();
            foreach (var url in urls)
            {
                try
                {
                    var json = await Client.GetStringAsync(url);
                    var resource = JsonConvert.DeserializeObject<Resource>(json);
                    if (!string.IsNullOrWhiteSpace(resource?.Name)) names.Add(resource.Name);
                }
                catch (Exception) { /* One unavailable resource should not hide the others. */ }
            }
            return names;
        }
    }
}
