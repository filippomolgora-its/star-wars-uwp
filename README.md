# Star Wars UWP App

A small educational Windows UWP application written in C# and XAML. It loads Star Wars films from [SWAPI](https://swapi.dev/), caches them in a local SQLite database, and shows the names of each selected film's characters, planets, starships, vehicles, and species.

## Technologies

- C# and Windows UWP
- XAML
- SWAPI REST API with `HttpClient`
- SQLite via `Microsoft.Data.Sqlite`
- JSON via `Newtonsoft.Json`

## Features

- Loads films from a local SQLite cache, downloading and saving them on the first launch.
- Shows film details and the opening crawl.
- Opens a resource analysis page for the selected film.
- Handles network and data errors with an in-app status and message.

Open `StarWarsApp.slnx` in Visual Studio, restore NuGet packages, and run the app on a Windows target.
