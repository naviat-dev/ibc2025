namespace ibc2025;

public sealed partial class QuestionBoardPage : Page
{
    public QuestionBoardPage()
    {
        InitializeComponent();
        PageBackground.Background = App.DailyBackground;
        FillGridWithButtons(QuestionBoard);
    }

    // Example: Fill a Grid with Buttons (assume you have a Grid named "MyGrid" in XAML)
    public void FillGridWithButtons(Grid grid)
    {
        int rows = grid.RowDefinitions.Count;
        int cols = grid.ColumnDefinitions.Count;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                var button = new Button
                {
                    Content = row * cols + col + 1,
                    FontFamily = new FontFamily("Bahnschrift"),
                    Name = "Q" + (row * cols + col + 1),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch
                };
                button.SetValue(Grid.RowProperty, row);
                button.SetValue(Grid.ColumnProperty, col);
                button.Click += (s, e) => GoToQuestion(s, e);
                button.HorizontalContentAlignment = HorizontalAlignment.Center;
                button.VerticalContentAlignment = VerticalAlignment.Center;
                button.Loaded += (s, e) =>
                {
                    var btn = (Button)s;
                    double min = Math.Min(btn.ActualWidth, btn.ActualHeight);
                };
                Grid.SetRow(button, row);
                Grid.SetColumn(button, col);
                grid.Children.Add(button);
            }
        }
    }

    private void GoToQuestion(object sender, RoutedEventArgs e)
    {
        var btn = (Button)sender;
        if (App.MASTER_MODE)
        {
            // Handle master mode logic here
            Console.WriteLine($"Master mode: Button {btn.Name} clicked.");
            _ = ((Frame)Window.Current.Content).Navigate(typeof(QuestionPage));
        }
        else
        {
            // Handle mirror mode logic here
            Console.WriteLine("You can't click buttons in mirror mode.");
        }
    }
}
