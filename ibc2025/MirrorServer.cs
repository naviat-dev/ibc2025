using Firebase.Database.Query;

namespace ibc2025;

public class MirrorServer
{
	public static string MirrorId;
	private static bool runCommand = true;
	public static event Action MirrorAvailabilityChanged;
	public static event Action QuestionBoardCommandChanged;
	public static event Action QuestionCommandChanged;
	public static string LastCommand;
    public static readonly Dictionary<string, Action<(object, RoutedEventArgs)>> Commands = new()
    {
        { "QuestionBoardPage.GoToQuestion", t => QuestionBoardPage.GoToQuestion(t.Item1, t.Item2) },
        { "QuestionBoardPage.RegionIncr", t => QuestionBoardPage.RegionIncr(t.Item1, t.Item2) },
        { "QuestionBoardPage.RegionDecr", t => QuestionBoardPage.RegionDecr(t.Item1, t.Item2) }
    };

	public static void Execute(string name)
	{
		if (Commands.ContainsKey(name.Split(":")[0]))
		{
			LastCommand = name;
			if (name.StartsWith("QuestionBoard"))
			{
				QuestionBoardCommandChanged.Invoke();
			}
			else if (name.StartsWith("Question"))
			{
				QuestionCommandChanged.Invoke();
			}
		}
		else
			Console.WriteLine($"[Mirror] Unknown command: {name}");
	}

	public static void ListenForPings()
	{
		App.Database.Child("mirrors").Child(MirrorId).AsObservable<string>().Subscribe(async ping =>
		{
			if (ping.Key == "command" && !string.IsNullOrWhiteSpace(ping.Object))
			{
				if (runCommand)
				{
					runCommand = !runCommand;
					Execute(ping.Object);
					Console.WriteLine($"[Mirror] Ping received! Method: {ping.Object}");
					await App.Database.Child("mirrors").Child(MirrorId).Child("command").PutAsync("");
				}
				else
				{
					runCommand = !runCommand;
				}
			}
		});
	}

	public static async Task MirrorInit()
	{
		MirrorId = Environment.MachineName;
		await App.Database.Child("mirrors").Child(MirrorId).Child("available").PutAsync(true);
		await App.Database.Child("mirrors").Child(MirrorId).Child("command").PutAsync("");
		await App.Database.Child("mirrors").Child(MirrorId).Child("lastSeen").PutAsync(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
		App.Database.Child("mirrors").Child(MirrorId).AsObservable<string>().Subscribe(async ping =>
		{
			if (ping.Key == "available" && !string.IsNullOrWhiteSpace(ping.Object))
			{
				Console.WriteLine(bool.Parse(ping.Object));
				if (!bool.Parse(ping.Object))
				{
					MirrorAvailabilityChanged.Invoke();
					ListenForPings();
				}
			}
		});
	}

	public static async Task MirrorShutdown()
	{
		if (App.MasterMode)
		{
			Console.WriteLine("You shouldn't attempt to terminate a mirror server from a master application");
			return;
		}
		try
		{
			await App.Database.Child("mirrors").Child(MirrorId).DeleteAsync();
			Console.WriteLine($"Mirror {MirrorId} deleted successfully.");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Failed to delete mirror {MirrorId}: {ex.Message}");
		}
	}
}