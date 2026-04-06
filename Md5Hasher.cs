using System.Security.Cryptography;
using System.Text;
using static System.Text.Encoding;
namespace scrobbler;

public class Md5Hasher
{
    public string ComputeMd5Hash(string message) {
        using (MD5 md5 = MD5.Create()) {
            byte[] input = Encoding.UTF8.GetBytes(message);
            byte[] hash = md5.ComputeHash(input);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++) {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }
}