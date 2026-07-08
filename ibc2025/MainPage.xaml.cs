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

    private void Start(object sender, RoutedEventArgs e)
    {
        Storyboard storyboard = App.SlideOutAnimation("X", TimeSpan.FromSeconds(0.5), RootGrid, MainTransform);
        storyboard.Completed += static (s, args) =>
        {
            _ = ((Frame)Window.Current.Content).Navigate(typeof(CategoryPage));
        };
        storyboard.Begin();
    }
}
