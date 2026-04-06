using System.Diagnostics;
using Newtonsoft.Json;
using Microsoft.AspNetCore.WebUtilities;
using static System.Environment;

namespace scrobbler;

public class LastFM
{
    private Auth user;
    private static HttpClient http_client;
    private static AuthSig sig = new AuthSig();
    private static string base_url = "http://ws.audioscrobbler.com/2.0/";


    public LastFM(Auth auth, HttpClient http)
    {
        user = auth;
        http_client = http;

        //first time use prompts user to login and gets session key
        if (auth.session_key == null)
        {
            Authenticate(user);
        }
    }

    private void Authenticate(Auth user)
    {
        getToken(user, new AuthSig()).Wait();
        UserLogin();
        Console.WriteLine("Please press ENTER once you have allowed access with Last.FM");
        Console.ReadKey();
        getSession(user).Wait();
    }

    private static async Task getToken(Auth user, AuthSig sig)
    {

        string method = "auth.getToken";
        var parameters = new Dictionary<string, string>();
        parameters.Add("api_key", user.api_key);
        parameters.Add("method", method);
        parameters.Add("api_sig", sig.CreateSignature(parameters, user.api_secret));
        parameters.Add("format", "json");

        var uri = QueryHelpers.AddQueryString(base_url, parameters);

        try
        {
            var response =
                await http_client.GetAsync(uri).ConfigureAwait(false);

            string responseBody = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            dynamic json = JsonConvert.DeserializeObject(responseBody);
            user.token = json.token;
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Request error: {e.Message}");
        }

    }

    private void UserLogin()
    {
        var parameters = new Dictionary<string, string>();
        parameters.Add("api_key", user.api_key);
        parameters.Add("token", user.token);
        var uri = QueryHelpers.AddQueryString("http://last.fm/api/auth/", parameters);
        Process.Start(new ProcessStartInfo(uri) { UseShellExecute = true });
    }

    private static async Task getSession(Auth user)
    {
        try
        {
            var parameters = new Dictionary<string, string>();
            parameters.Add("api_key", user.api_key);
            parameters.Add("method", "auth.getsession");
            parameters.Add("token", user.token);
            parameters.Add("api_sig", sig.CreateSignature(parameters, user.api_secret));
            parameters.Add("format", "json");


            //Different base url for authentication!
            var uri = QueryHelpers.AddQueryString(base_url, parameters);

            var response =
                await http_client.GetAsync(uri);

            string responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseBody);
            response.EnsureSuccessStatusCode();

            dynamic json = JsonConvert.DeserializeObject(responseBody);
            user.session_key = json.session.key;
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Request error: {e.Message}");
        }
    }

    public async Task<ScrobbleResponse.Root> Scrobble(Track[] chunk, int OFFSET)
    {
        string method = "track.scrobble";
        var parameters = new Dictionary<string, string>();
        parameters.Add("api_key", user.api_key);
        parameters.Add("method", method);
        parameters.Add("sk", user.session_key);
        int i = 0;
        foreach (var track in chunk)
        {
            parameters.Add($"artist[{i}]", track.Artist);
            parameters.Add($"album[{i}]", track.Album);
            parameters.Add($"track[{i}]", track.Title);
            parameters.Add($"timestamp[{i}]", (track.TimeStamp - OFFSET).ToString());
            i++;
        }

        parameters.Add("api_sig", sig.CreateSignature(parameters, user.api_secret));
        parameters.Add("format", "json");


        var content = new FormUrlEncodedContent(parameters);

        var uri = QueryHelpers.AddQueryString(base_url, parameters);

        var res = new ScrobbleResponse.Root();
        try
        {
            var response =
                await http_client.PostAsync(base_url, content);

            string responseBody = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            res = JsonConvert.DeserializeObject<ScrobbleResponse.Root>(responseBody);
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Request error: {e.Message}");
        }

        return res;
    }
}
