using Newtonsoft.Json;

namespace scrobbler;

public class ScrobbleResponse
{
    public class Album
    {
        public string corrected { get; set; }

        [JsonProperty("#text")]
        public string text { get; set; }
    }

    public class AlbumArtist
    {
        public string corrected { get; set; }

        [JsonProperty("#text")]
        public string text { get; set; }
    }

    public class Artist
    {
        public string corrected { get; set; }

        [JsonProperty("#text")]
        public string text { get; set; }
    }

    public class Attr
    {
        public int ignored { get; set; }
        public int accepted { get; set; }
    }

    public class IgnoredMessage
    {
        public string code { get; set; }

        [JsonProperty("#text")]
        public string text { get; set; }
    }

    public class Root
    {
        public Scrobbles scrobbles { get; set; }
    }

    public class Scrobble
    {
        public Artist artist { get; set; }
        public Album album { get; set; }
        public Track track { get; set; }
        public IgnoredMessage ignoredMessage { get; set; }
        public AlbumArtist albumArtist { get; set; }
        public string timestamp { get; set; }
    }

    public class Scrobbles
    {
        public List<Scrobble> scrobble { get; set; }

        [JsonProperty("@attr")]
        public Attr attr { get; set; }
    }

    public class Track
    {
        public string corrected { get; set; }

        [JsonProperty("#text")]
        public string text { get; set; }
    }


}