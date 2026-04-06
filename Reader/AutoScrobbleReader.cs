namespace scrobbler;

public class AutoScrobbleReader : IReader
{
    public IEnumerable<Track> Read(string file)
    {
        //do something   
        var tracks = new List<Track>();

        string[] split = file.Split('\n', 1000, StringSplitOptions.RemoveEmptyEntries);
        
        for (var i = 4; i < split.Length; i++)
        {
            string[] track = split[i].Split('\t', 8, StringSplitOptions.None);
            Track song = new(track[0], //artist
                track[1], //album
            track[2], //track title
                int.Parse(track[3]), //track num
                int.Parse(track[4]),  // track length
                char.Parse(track[5]), //rating
                int.Parse(track[6]), 
                track[7], 
                false);
            
            tracks.Add(song);
        }
        
        return tracks;
    }
}

