using Microsoft.UI.Xaml.Media.Animation;

namespace ibc2025;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        InitializeComponent();
        PageBackground.Background = App.DailyBackground;
        App.SlideInAnimation("X", TimeSpan.FromSeconds(0.5), RootGrid, MainTransform);

    }

    private void StartMaster(object sender, RoutedEventArgs e)
    {
        App.MASTER_MODE = true;
        Storyboard storyboard = App.SlideOutAnimation("X", TimeSpan.FromSeconds(0.5), RootGrid, MainTransform);
        storyboard.Completed += (s, args) =>
        {
            _ = ((Frame)Window.Current.Content).Navigate(typeof(QuestionBoardPage));
        };
        storyboard.Begin();
    }

    private void StartMirror(object sender, RoutedEventArgs e)
    {
        App.MASTER_MODE = false;
        Storyboard storyboard = App.SlideOutAnimation("X", TimeSpan.FromSeconds(0.5), RootGrid, MainTransform);
        storyboard.Completed += (s, args) =>
        {
            _ = ((Frame)Window.Current.Content).Navigate(typeof(ConnectionMirrorPage));
        };
        storyboard.Begin();
    }
}
