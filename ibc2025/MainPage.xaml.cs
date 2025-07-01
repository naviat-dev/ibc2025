using Microsoft.UI.Xaml.Media.Animation;

namespace ibc2025;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        InitializeComponent();
        PageBackground.Background = App.DailyBackground;

    }

    private void StartMaster(object sender, RoutedEventArgs e)
    {
        App.MASTER_MODE = true;
        Storyboard storyboard = SlideAnimation("X", TimeSpan.FromSeconds(0.5));
        storyboard.Completed += (s, args) =>
        {
            _ = ((Frame)Window.Current.Content).Navigate(typeof(QuestionBoardPage));
        };
        storyboard.Begin();
    }

    private void StartMirror(object sender, RoutedEventArgs e)
    {
        App.MASTER_MODE = false;
        Storyboard storyboard = SlideAnimation("X", TimeSpan.FromSeconds(0.5));
        storyboard.Completed += (s, args) =>
        {
            _ = ((Frame)Window.Current.Content).Navigate(typeof(QuestionBoardPage));
        };
        storyboard.Begin();
    }

    private Storyboard SlideAnimation(string axis, TimeSpan duration, int offset = -200)
    {
        Storyboard storyboard = new();

        DoubleAnimation animationPos = new()
        {
            To = -200,
            Duration = TimeSpan.FromSeconds(0.5),
            EasingFunction = new ExponentialEase
            {
                EasingMode = EasingMode.EaseOut,
                Exponent = 5
            }
        };

        DoubleAnimation animationOpacity = new()
        {
            To = 0.0, // Fade out to invisible
            Duration = TimeSpan.FromSeconds(0.5),
            EasingFunction = new ExponentialEase
            {
                EasingMode = EasingMode.EaseOut,
                Exponent = 5
            }
        };

        Storyboard.SetTarget(animationOpacity, RootGrid);
        Storyboard.SetTargetProperty(animationOpacity, "Opacity");

        storyboard.Children.Add(animationOpacity);

        Storyboard.SetTarget(animationPos, MainTransform);
        Storyboard.SetTargetProperty(animationPos, axis);

        storyboard.Children.Add(animationPos);
        storyboard.Begin();

        return storyboard;
    }
}
