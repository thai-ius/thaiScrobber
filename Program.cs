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
        
        var pathTo = "";
        bool savedPathTo = false;
        
        //Checks for saved file path to .scrobbler.log
        if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "pathTo")))
        {
            pathTo = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "pathTo"));
            savedPathTo = true;
        }
        

        var user = new Auth();
        
        //Checks for saved user authentication
        if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "auth.json")))
        {
            user = JsonConvert.DeserializeObject<Auth>(
                File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "auth.json")));
        }
        else
        {
            Console.WriteLine("Please paste your API Key:");
            user.api_key = Console.ReadLine();
            Console.WriteLine("Please paste your API Secret:");
            user.api_secret = Console.ReadLine();
        }
        
        
        //GET AND STORE TOKEN
        var httpClient = new HttpClient();
        var session = new LastFM(user, httpClient);
        JsonConvert.SerializeObject(user);
        System.IO.File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "auth.json"),JsonConvert.SerializeObject(user));

        //Input tracks from .scrobbler.log
        var reader = new Reader();
        reader.setStrategy(new AutoScrobbleReader());

        //Requires valid file path to .scrobbler.log
        while (pathTo == "")
        {
            Console.WriteLine("Paste the path to your file: ");
            var temp = Console.ReadLine();
            if (File.Exists(temp))
            {
                pathTo = temp;
            }
            else
            {
                Console.WriteLine($"File at {temp} does not exist.");
            }
        }
        
        if (!savedPathTo)
        {
            Console.WriteLine($"Would you like to save {pathTo} for next time? (Y/N)");
            var input = Console.ReadLine();
            if (input == "Y")
                File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "pathTo"), pathTo);
        }
        

        var tracks = reader.Read(File.ReadAllText(pathTo));
        var chunks = tracks.Chunk(batchsize);

        //Scrobble tracks to Last.FM
        var responses = new List<ScrobbleResponse.Root>();
        foreach(var chunk in chunks)
        {
            //Store responses into JSON class
            ScrobbleResponse.Root response = session.Scrobble(chunk, offset).Result;
            responses.Add(response);
        }

        int accepted = 0;
        int ignored = 0; 
        //Display results
        foreach (var response in responses)
        {
            foreach (var scrobble in response.scrobbles.scrobble)
            {
                if (scrobble.ignoredMessage.code != "0") 
                    Console.Write("[FAILED] \t");
                Console.Write(scrobble.artist.text + "\t" + scrobble.album.text + "\t" + scrobble.track.text + "\n");
            }

            accepted += response.scrobbles.attr.accepted;
            ignored += response.scrobbles.attr.ignored;
        }
        
        Console.WriteLine($"Tracks accepted: {accepted} Tracks ignored: {ignored}");
        Console.WriteLine("Delete scrobble log? Enter Y/N:");
        
        if (Console.ReadLine() == "Y")
            File.Delete(pathTo);
        
        
    }
    
}

