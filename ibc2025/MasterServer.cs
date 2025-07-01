using System.Text;

namespace ibc2025;

public class MasterPinger
{
    private readonly HttpClient _http = new();

    public async Task SendPingAsync(string mirrorIp, string methodName, string sender)
    {
        StringContent content = new (methodName + "," + sender, Encoding.UTF8, "text/plain");

        try
        {
            HttpResponseMessage response = await _http.PostAsync($"http://{mirrorIp}:5000/mirror/ping", content);
            Console.WriteLine($"[Master] Sent {methodName}, status: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Master] Ping failed: {ex.Message}");
        }
    }
}
