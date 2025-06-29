namespace ibc2025;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();
    }

    private void StartMaster(object sender, RoutedEventArgs e)
    {
        App.MASTER_MODE = true;
        _ = ((Frame)Window.Current.Content).Navigate(typeof(QuestionBoardPage));
    }

    private void StartMirror(object sender, RoutedEventArgs e)
    {
        App.MASTER_MODE = false;
        _ = ((Frame)Window.Current.Content).Navigate(typeof(QuestionBoardPage));
    }
}
