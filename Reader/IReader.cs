namespace scrobbler;

public interface IReader
{
    public IEnumerable<Track> Read(string file);
}

