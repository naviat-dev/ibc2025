using Firebase.Database;
using Microsoft.UI.Xaml.Media.Animation;
using Uno.Resizetizer;

namespace ibc2025;

public partial class App : Application
{
    public static List<Question> Questions = [];
    public static int ActiveQuestion = 0;
    public static int[] TeamPts = [0, 0, 0, 0, 0, 0, 0];
    public static int[] TeamPtsDsply = [0, 0, 0, 0, 0, 0, 0];
    public static bool MasterMode;
    public static LinearGradientBrush DailyBackground;
    public static readonly DateTime Date = DateTime.Today;
    public static readonly string DatabaseURL = "https://ibc2025-7bd8a-default-rtdb.firebaseio.com/";
    public static readonly FirebaseClient Database = new(DatabaseURL);
    public static readonly Dictionary<string, Action<(object, RoutedEventArgs)>> Commands = new()
    {
        { "QuestionBoardPage.GoToQuestion", t => QuestionBoardPage.GoToQuestion(t.Item1, t.Item2) },
        { "QuestionBoardPage.RegionIncr", t => QuestionBoardPage.RegionIncr(t.Item1, t.Item2) },
        { "QuestionBoardPage.RegionDecr", t => QuestionBoardPage.RegionDecr(t.Item1, t.Item2) }
    };

    /// <summary>
    /// Initializes the singleton application object. This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        InitializeComponent();
        Suspending += (s, e) =>
        {
            Console.WriteLine("Shutting down...");
            var task = MirrorServer.MirrorShutdown();
            task.Wait(TimeSpan.FromSeconds(3));
        };
        Start();
    }

    protected Window? MainWindow { get; private set; }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        MainWindow = new Window();
#if DEBUG
        MainWindow.UseStudio();
#endif


        // Do not repeat app initialization when the Window already has content,
        // just ensure that the window is active
        if (MainWindow.Content is not Frame rootFrame)
        {
            // Create a Frame to act as the navigation context and navigate to the first page
            rootFrame = new Frame();

            // Place the frame in the current Window
            MainWindow.Content = rootFrame;

            rootFrame.NavigationFailed += OnNavigationFailed;
        }

        if (rootFrame.Content == null)
        {
            // When the navigation stack isn't restored navigate to the first page,
            // configuring the new page by passing required information as a navigation
            // parameter
            rootFrame.Navigate(typeof(MainPage), args.Arguments);
        }

        MainWindow.SetWindowIcon();
        // Ensure the current window is active
        MainWindow.Activate();
    }

    /// <summary>
    /// Invoked when Navigation to a certain page fails
    /// </summary>
    /// <param name="sender">The Frame which failed navigation</param>
    /// <param name="e">Details about the navigation failure</param>
    void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
    {
        throw new InvalidOperationException($"Failed to load {e.SourcePageType.FullName}: {e.Exception}");
    }

    /// <summary>
    /// Configures global Uno Platform logging
    /// </summary>
    public static void InitializeLogging()
    {
#if DEBUG
        // Logging is disabled by default for release builds, as it incurs a significant
        // initialization cost from Microsoft.Extensions.Logging setup. If startup performance
        // is a concern for your application, keep this disabled. If you're running on the web or
        // desktop targets, you can use URL or command line parameters to enable it.
        //
        // For more performance documentation: https://platform.uno/docs/articles/Uno-UI-Performance.html

        var factory = LoggerFactory.Create(builder =>
        {
#if __WASM__
            builder.AddProvider(new global::Uno.Extensions.Logging.WebAssembly.WebAssemblyConsoleLoggerProvider());
#elif __IOS__
            builder.AddProvider(new global::Uno.Extensions.Logging.OSLogLoggerProvider());

            // Log to the Visual Studio Debug console
            builder.AddConsole();
#else
            builder.AddConsole();
#endif

            // Exclude logs below this level
            builder.SetMinimumLevel(LogLevel.Information);

            // Default filters for Uno Platform namespaces
            builder.AddFilter("Uno", LogLevel.Warning);
            builder.AddFilter("Windows", LogLevel.Warning);
            builder.AddFilter("Microsoft", LogLevel.Warning);

            // Generic Xaml events
            // builder.AddFilter("Microsoft.UI.Xaml", LogLevel.Debug );
            // builder.AddFilter("Microsoft.UI.Xaml.VisualStateGroup", LogLevel.Debug );
            // builder.AddFilter("Microsoft.UI.Xaml.StateTriggerBase", LogLevel.Debug );
            // builder.AddFilter("Microsoft.UI.Xaml.UIElement", LogLevel.Debug );
            // builder.AddFilter("Microsoft.UI.Xaml.FrameworkElement", LogLevel.Trace );

            // Layouter specific messages
            // builder.AddFilter("Microsoft.UI.Xaml.Controls", LogLevel.Debug );
            // builder.AddFilter("Microsoft.UI.Xaml.Controls.Layouter", LogLevel.Debug );
            // builder.AddFilter("Microsoft.UI.Xaml.Controls.Panel", LogLevel.Debug );

            // builder.AddFilter("Windows.Storage", LogLevel.Debug );

            // Binding related messages
            // builder.AddFilter("Microsoft.UI.Xaml.Data", LogLevel.Debug );
            // builder.AddFilter("Microsoft.UI.Xaml.Data", LogLevel.Debug );

            // Binder memory references tracking
            // builder.AddFilter("Uno.UI.DataBinding.BinderReferenceHolder", LogLevel.Debug );

            // DevServer and HotReload related
            // builder.AddFilter("Uno.UI.RemoteControl", LogLevel.Information);

            // Debug JS interop
            // builder.AddFilter("Uno.Foundation.WebAssemblyRuntime", LogLevel.Debug );
        });

        global::Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory = factory;

#if HAS_UNO
        global::Uno.UI.Adapter.Microsoft.Extensions.Logging.LoggingAdapter.Initialize();
#endif
#endif
    }

    public void Start()
    {
        FileStream fileStream = new("Assets/questions.tsv", FileMode.Open, FileAccess.Read);
        using StreamReader reader = new(fileStream);
        DailyBackground = new LinearGradientBrush
        {
            StartPoint = new Windows.Foundation.Point(0, 0),
            EndPoint = new Windows.Foundation.Point(1, 1)
        };
        if (Date.ToString().Split(" ")[0] == "7/17/2025")
        {
            DailyBackground.GradientStops.Add(new GradientStop { Color = Windows.UI.Color.FromArgb(255, 25, 48, 115), Offset = 0 });
            DailyBackground.GradientStops.Add(new GradientStop { Color = Windows.UI.Color.FromArgb(255, 39, 20, 82), Offset = 1 });
        }
        else if (Date.ToString().Split(" ")[0] == "7/18/2025")
        {
            for (int i = 0; i < 70; i++)
            {
                reader.ReadLine(); // Skip the first 70 lines
            }
            DailyBackground.GradientStops.Add(new GradientStop { Color = Windows.UI.Color.FromArgb(255, 39, 20, 82), Offset = 0 });
            DailyBackground.GradientStops.Add(new GradientStop { Color = Windows.UI.Color.FromArgb(255, 8, 62, 71), Offset = 1 });
        }
        else if (Date.ToString().Split(" ")[0] == "6/30/2025")
        {
            for (int i = 0; i < 140; i++)
            {
                reader.ReadLine(); // Skip the first 140 lines
            }
            DailyBackground.GradientStops.Add(new GradientStop { Color = Windows.UI.Color.FromArgb(255, 8, 62, 71), Offset = 0 });
            DailyBackground.GradientStops.Add(new GradientStop { Color = Windows.UI.Color.FromArgb(255, 108, 43, 112), Offset = 1 });
        }
        else
        {
            DailyBackground.GradientStops.Add(new GradientStop { Color = Windows.UI.Color.FromArgb(255, 25, 48, 115), Offset = 0 });
            DailyBackground.GradientStops.Add(new GradientStop { Color = Windows.UI.Color.FromArgb(255, 39, 20, 82), Offset = 0.333 });
            DailyBackground.GradientStops.Add(new GradientStop { Color = Windows.UI.Color.FromArgb(255, 8, 62, 71), Offset = 0.667 });
            DailyBackground.GradientStops.Add(new GradientStop { Color = Windows.UI.Color.FromArgb(255, 108, 43, 112), Offset = 1 });
        }
        for (int i = 0; i < 70; i++)
        {
            string[] line = reader.ReadLine().Split('\t');
            Questions.Add(line[2].Length > 0 ? new Question(line[1], line[6], [line[2], line[3], line[4], line[5]], line[7]) : new Question(line[1], line[6], line[7]));
        }
    }

    public static Storyboard SlideOutAnimation(string axis, TimeSpan duration, DependencyObject elementOpacity, DependencyObject elementTrans, int offset = -200)
    {
        Storyboard storyboard = new();

        DoubleAnimation animationPos = new()
        {
            From = 0,
            To = offset,
            Duration = duration,
            EasingFunction = new ExponentialEase
            {
                EasingMode = EasingMode.EaseOut,
                Exponent = 5
            }
        };

        DoubleAnimation animationOpacity = new()
        {
            To = 0,
            Duration = duration,
            EasingFunction = new ExponentialEase
            {
                EasingMode = EasingMode.EaseOut,
                Exponent = 5
            }
        };

        Storyboard.SetTarget(animationOpacity, elementOpacity);
        Storyboard.SetTargetProperty(animationOpacity, "Opacity");

        storyboard.Children.Add(animationOpacity);

        Storyboard.SetTarget(animationPos, elementTrans);
        Storyboard.SetTargetProperty(animationPos, axis);

        storyboard.Children.Add(animationPos);

        return storyboard;
    }

    public static Storyboard SlideInAnimation(string axis, TimeSpan duration, DependencyObject elementOpacity, DependencyObject elementTrans, int offset = -200)
    {
        Storyboard storyboard = new();

        DoubleAnimation animationPos = new()
        {
            From = offset,
            To = 0,
            Duration = duration,
            EasingFunction = new ExponentialEase
            {
                EasingMode = EasingMode.EaseOut,
                Exponent = 5
            }
        };

        DoubleAnimation animationOpacity = new()
        {
            From = 0,
            To = 1,
            Duration = duration,
            EasingFunction = new ExponentialEase
            {
                EasingMode = EasingMode.EaseOut,
                Exponent = 5
            }
        };

        Storyboard.SetTarget(animationOpacity, elementOpacity);
        Storyboard.SetTargetProperty(animationOpacity, "Opacity");

        storyboard.Children.Add(animationOpacity);

        Storyboard.SetTarget(animationPos, elementTrans);
        Storyboard.SetTargetProperty(animationPos, axis);

        storyboard.Children.Add(animationPos);
        storyboard.Begin();

        return storyboard;
    }
}
