namespace ibc2025;

public sealed partial class QuestionPage : Page
{
    public QuestionPage()
    {
        InitializeComponent();
        PageBackground.Background = App.DailyBackground;
        QuestionHeader.SetValue(TextBlock.TextProperty, "Question " + App.ActiveQuestion);
        QuestionText.SetValue(TextBlock.TextProperty, App.Questions[App.ActiveQuestion - 1].QuestionText);
        TempBackBtn.Click += (s, e) =>
        {
            _ = ((Frame)Window.Current.Content).Navigate(typeof(QuestionBoardPage));
        };
    }
}