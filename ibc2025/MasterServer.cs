using Firebase.Database.Query;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ibc2025;

public class MasterServer
{
    public static string MirrorId;

    public static async Task<(List<string> mirrorIds, List<string> mirrorNames)> MirrorDiscover()
    {
        var result = await App.Database.Child("mirrors").OnceAsync<object>(); // Gets root-level data
        Dictionary<string, object> mirrors = result.ToDictionary(static x => x.Key, static x => x.Object);
        JObject db = JObject.Parse(JsonConvert.SerializeObject(result.ToDictionary(x => x.Key, x => x.Object), Formatting.Indented));
        List<string> mirrorIds = [.. db.Properties().Select(p => p.Name)];
        List<string> mirrorNames = [.. mirrors.Values.Select(static m => (m as JObject)?["name"]?.ToString() ?? (m is IDictionary<string, object> dict && dict.TryGetValue("name", out object? value) ? value?.ToString() : null)).Where(static name => !string.IsNullOrEmpty(name))];
        return (mirrorIds, mirrorNames);
    }

    public static async Task SendPingToMirror(string mirrorId, string method, string sender)
    {
        await App.Database.Child("mirrors").Child(mirrorId).Child("command").PutAsync(method + ":" + sender);
    }

    public static async Task MasterShutdown()
    {
        if (MirrorId != null)
        {
            if (await App.Database.Child("mirrors").Child(MirrorId).OnceSingleAsync<object>() != null)
            {
                await App.Database.Child("mirrors").Child(MirrorId).Child("available").PutAsync(true);
            }
        }
    }
}
