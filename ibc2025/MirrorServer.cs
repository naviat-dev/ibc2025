using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
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
        { "QuestionBoardPage.GoToQuestion", static t => QuestionBoardPage.GoToQuestion(t.Item1, t.Item2) },
        { "QuestionBoardPage.RegionIncr", static t => QuestionBoardPage.RegionIncr(t.Item1, t.Item2) },
        { "QuestionBoardPage.RegionDecr", static t => QuestionBoardPage.RegionDecr(t.Item1, t.Item2) },
        { "QuestionPage.GoToQuestionBoard", static t => QuestionPage.GoToQuestionBoard(t.Item1, t.Item2) },
        { "QuestionPage.AnswerSelect", static t => QuestionPage.AnswerSelect(t.Item1, t.Item2) }
    };

	private static string GetHashedMachineIdentifier()
    {
        // Get machine name and user name
        string machineName = Environment.MachineName;
        string userName = Environment.UserName;

        // Get primary MAC address
        string macAddress = NetworkInterface.GetAllNetworkInterfaces().FirstOrDefault(static n => n.NetworkInterfaceType != NetworkInterfaceType.Loopback && n.OperationalStatus == OperationalStatus.Up && !n.Description.Contains("virtual", StringComparison.CurrentCultureIgnoreCase) && !n.Name.Contains("virtual", StringComparison.CurrentCultureIgnoreCase))?.GetPhysicalAddress().ToString() ?? "00:00:00:00:00:00";

        // Combine identifiers
        string rawId = $"{machineName}-{userName}-{macAddress}";

        // Hash with SHA-256
        byte[] hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawId));
        return Convert.ToHexString(hashBytes); // returns uppercase hex string
    }

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
	}

	public static void ListenForPings()
	{
		App.Database.Child("mirrors").Child(MirrorId).AsObservable<string>().Subscribe(static async ping =>
		{
			if (ping.Key == "command" && !string.IsNullOrWhiteSpace(ping.Object))
			{
				if (runCommand)
				{
					runCommand = !runCommand;
					Execute(ping.Object);
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
		MirrorId = GetHashedMachineIdentifier();
		await App.Database.Child("mirrors").Child(MirrorId).Child("name").PutAsync(Environment.MachineName);
		await App.Database.Child("mirrors").Child(MirrorId).Child("available").PutAsync(true);
		await App.Database.Child("mirrors").Child(MirrorId).Child("command").PutAsync("");
		App.Database.Child("mirrors").Child(MirrorId).AsObservable<string>().Subscribe(static async ping =>
		{
			if (ping.Key == "available" && !string.IsNullOrWhiteSpace(ping.Object))
			{
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
			return;
		}
		try
		{
			await App.Database.Child("mirrors").Child(MirrorId).DeleteAsync();
		}
		catch (Exception)
		{

		}
	}
}