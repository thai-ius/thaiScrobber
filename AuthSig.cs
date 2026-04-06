using System.Text;


namespace scrobbler;

public class AuthSig
{
    public string CreateSignature(Dictionary<string, string> parameters, string secret)
    {
        var builder = new StringBuilder();
        var hasher = new Md5Hasher();

        foreach (var kv in parameters.OrderBy(kv => kv.Key, StringComparer.Ordinal))
        {
            builder.Append(kv.Key);
            builder.Append(kv.Value);
        }

        builder.Append(secret);
        
        return(hasher.ComputeMd5Hash(builder.ToString()));
    }
}