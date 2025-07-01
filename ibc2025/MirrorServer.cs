using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace ibc2025;

public class MirrorServer
{
    private static readonly Dictionary<string, Action<(object, RoutedEventArgs)>> _commands = new()
    {
        { "QuestionBoardPage.GoToQuestion", t => QuestionBoardPage.GoToQuestion(t.Item1, t.Item2) }
    };

    public static void Start()
    {
        Task.Run(() =>
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder();
            WebApplication app = builder.Build();

            app.MapPost("/mirror/ping", async (HttpContext context) =>
            {
                using StreamReader reader = new(context.Request.Body);
                string methodName = await reader.ReadToEndAsync();

                Console.WriteLine($"[Mirror] Received: {methodName}");
                Execute(methodName);
                return Results.Ok();
            });

            app.Urls.Add("http://0.0.0.0:5000");
            app.Run();
        });
    }

    public static void Execute(string name)
    {
        if (_commands.TryGetValue(name, out var action))
        {
            
        }
        // action();
        else
            Console.WriteLine($"[Mirror] Unknown command: {name}");
    }

    public static void DiscoveryInit()
    {
        Task.Run(() =>
        {
            using var udp = new UdpClient(8888);
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

            Console.WriteLine("[Mirror] Listening for UDP discovery pings...");

            while (true)
            {
                var data = udp.Receive(ref remoteEP);
                var message = Encoding.UTF8.GetString(data);

                if (message == "discover-mirror")
                {
                    Console.WriteLine($"[Mirror] Received discovery ping from {remoteEP.Address}");

                    // Respond with identity
                    var response = Encoding.UTF8.GetBytes("mirror-online");
                    udp.Send(response, response.Length, remoteEP);
                }
            }
        });
    }
}
