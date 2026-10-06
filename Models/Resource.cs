using Newtonsoft.Json;

namespace StarWarsApp.Models
{
    // SWAPI resources use the same name field for all five analysis categories.
    public class Resource
    {
        [JsonProperty("name")] public string Name { get; set; } = "";
    }

    public sealed class Person : Resource { }
    public sealed class Planet : Resource { }
    public sealed class Starship : Resource { }
    public sealed class Vehicle : Resource { }
    public sealed class Species : Resource { }
}
