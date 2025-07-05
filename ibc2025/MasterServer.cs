using Firebase.Database.Query;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ibc2025;

public class MasterServer
{
    public static async Task MirrorDiscover()
    {

        var result = await App.Database.Child("mirrors").OnceAsync<object>(); // Gets root-level data
        Dictionary<string, object> mirrors = result.ToDictionary(x => x.Key, x => x.Object);
        JObject db = JObject.Parse(JsonConvert.SerializeObject(result.ToDictionary(x => x.Key, x => x.Object), Formatting.Indented));
        // Get all top-level keys (mirror IDs) in the "mirrors" directory
        var mirrorIds = db.Properties().Select(p => p.Name).ToList();
        Console.WriteLine("Mirror IDs:");
        foreach (var id in mirrorIds)
        {
            Console.WriteLine(id);
        }
    }

    public static async Task SendPingToMirror(string mirrorId, string method, string sender)
    {
        await App.Database.Child("mirrors").Child(mirrorId).Child("command").PutAsync(method + ":" + sender);
        Console.WriteLine($"[Master] Pinged {mirrorId} with method '{method}'");
    }

}
