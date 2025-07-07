using Firebase.Database.Query;
using Microsoft.UI.Xaml.Media.Animation;

namespace ibc2025;

public partial class ConnectionMasterPage : Page
{
    public ConnectionMasterPage()
    {
        InitializeComponent();
        PageBackground.Background = App.DailyBackground;
        ContinueBtn.IsEnabled = false;
        Storyboard storyboard = App.SlideInAnimation("X", TimeSpan.FromSeconds(0.5), RootGrid, MainTransform);
        storyboard.Begin();
        DisplayMirrors();
    }

    private void GoToMain(object sender, RoutedEventArgs e)
    {
        Storyboard storyboard = App.SlideOutAnimation("X", TimeSpan.FromSeconds(0.5), RootGrid, MainTransform);
        storyboard.Completed += static (s, args) =>
        {
            _ = ((Frame)Window.Current.Content).Navigate(typeof(MainPage));
        };
        storyboard.Begin();
    }

    private async void GoToQuestionBoard(object sender, RoutedEventArgs e)
    {
        if (MasterServer.MirrorId != null)
        {
            await App.Database.Child("mirrors").Child(MasterServer.MirrorId).Child("available").PutAsync(false);
        }
        Storyboard storyboard = App.SlideOutAnimation("X", TimeSpan.FromSeconds(0.5), RootGrid, MainTransform);
        storyboard.Completed += static (s, args) =>
        {
            _ = ((Frame)Window.Current.Content).Navigate(typeof(QuestionBoardPage));
        };
        storyboard.Begin();
    }

    private async void DisplayMirrors()
    {
        (List<string> mirrorIds, List<string> mirrorNames) = await MasterServer.MirrorDiscover();
        MirrorList.Children.Clear();
        for (int i = 0; i < mirrorIds.Count; i++)
        {
            Button mirrorButton = new()
            {
                Content = mirrorNames[i],
                Tag = mirrorIds[i],
                FontFamily = "Bahnschrift",
                FontSize = 20,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Left
            };
            mirrorButton.SetValue(Grid.RowProperty, i);
            mirrorButton.Click += (sender, e) =>
            {
                MasterServer.MirrorId = (string)((Button)sender).Tag;
                ContinueBtn.IsEnabled = true;
            };
            MirrorList.Children.Add(mirrorButton);
        }
    }
}