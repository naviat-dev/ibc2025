using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace ibc2025;

public class MirrorServer
{
    public static void Start()
    {
        Task.Run(() =>
        {
            var builder = WebApplication.CreateBuilder();
            var app = builder.Build();

            var executor = new MirrorCommandExecutor();

            app.MapPost("/mirror/ping", async (HttpContext context) =>
            {
                using var reader = new StreamReader(context.Request.Body);
                var methodName = await reader.ReadToEndAsync();

                Console.WriteLine($"[Mirror] Received: {methodName}");
                executor.Execute(methodName);
                return Results.Ok();
            });

            app.Urls.Add("http://0.0.0.0:5000");
            app.Run();
        });
    }
}

public class MirrorCommandExecutor
{
    private readonly Dictionary<string, Action> _commands = new()
    {
        { "OpenMenu", () => Console.WriteLine("[Mirror] Opening Menu...") },
        { "CloseMenu", () => Console.WriteLine("[Mirror] Closing Menu...") }
    };

    public void Execute(string name)
    {
        if (_commands.TryGetValue(name, out var action))
            action();
        else
            Console.WriteLine($"[Mirror] Unknown command: {name}");
    }
}
