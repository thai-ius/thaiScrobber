namespace scrobbler;

public class Reader
{
    private IReader _reader;

    public void setStrategy(IReader reader)
    {
        _reader = reader;
    }

    public IEnumerable<Track> Read(string file)
    {
        return _reader.Read(file);
    }
    
}