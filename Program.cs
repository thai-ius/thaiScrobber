using System;
using System.Diagnostics;
using System.Net;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace scrobbler;

static class Scrobbler
{
    public static async Task Main()
    {
        //Set constants
        var localTimeZone = TimeZoneInfo.Local;
        var offset = localTimeZone.BaseUtcOffset.Hours * 3600;
        const int batchsize = 50;
        
        var path_to = @"";
        
        var user = JsonConvert.DeserializeObject<Auth>(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "auth.json")));
        
        //GET AND STORE TOKEN
        var httpClient = new HttpClient();
        var session = new LastFM(user, httpClient);
        JsonConvert.SerializeObject(user);
        System.IO.File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "auth.json"),JsonConvert.SerializeObject(user));

        //Input tracks from .scrobbler.log
        var reader = new Reader();
        reader.setStrategy(new AutoScrobbleReader());
        var tracks = reader.Read(File.ReadAllText(path_to));
        var chunks = tracks.Chunk(batchsize);
        
        //Scrobble tracks to Last.FM
        var responses = new List<ScrobbleResponse.Root>();
        foreach(var chunk in chunks)
        {
            //Store responses into JSON class
            ScrobbleResponse.Root response = session.Scrobble(chunk, offset).Result;
            responses.Add(response);
        }

        //Display results

    }
    
}

