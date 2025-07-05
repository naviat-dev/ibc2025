using Microsoft.UI.Xaml.Media.Animation;

namespace ibc2025;

public partial class ConnectionMirrorPage : Page
{
    public ConnectionMirrorPage()
    {
        InitializeComponent();
        PageBackground.Background = App.DailyBackground;
        Storyboard storyboard = App.SlideInAnimation("X", TimeSpan.FromSeconds(0.5), RootGrid, MainTransform);
        storyboard.Begin();
        MirrorServer.MirrorInit();
        MirrorServer.MirrorAvailabilityChanged += () =>
        {
            _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => GoToQuestionBoard());
        };
    }

    private void GoToMain(object sender, RoutedEventArgs e)
    {
        Storyboard storyboard = App.SlideOutAnimation("X", TimeSpan.FromSeconds(0.5), RootGrid, MainTransform);
        storyboard.Completed += (s, args) =>
        {
            _ = ((Frame)Window.Current.Content).Navigate(typeof(MainPage));
        };
        storyboard.Begin();
    }

    public void GoToQuestionBoard()
    {
        Storyboard storyboard = App.SlideOutAnimation("X", TimeSpan.FromSeconds(0.5), RootGrid, MainTransform);
        storyboard.Completed += (s, args) =>
        {
            _ = ((Frame)Window.Current.Content).Navigate(typeof(QuestionBoardPage));
        };
        storyboard.Begin();
    }
}