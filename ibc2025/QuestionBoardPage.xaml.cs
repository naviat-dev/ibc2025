namespace ibc2025;

public sealed partial class QuestionBoardPage : Page
{
    public QuestionBoardPage()
    {
        this.InitializeComponent();
        FillGridWithButtons(QuestionBoard);
    }

    // Example: Fill a Grid with Buttons (assume you have a Grid named "MyGrid" in XAML)
    public static void FillGridWithButtons(Grid grid)
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
                button.HorizontalContentAlignment = HorizontalAlignment.Center;
                button.VerticalContentAlignment = VerticalAlignment.Center;
                button.Loaded += (s, e) =>
                {
                    var btn = (Button)s;
                    // Set font size to fill button (simple approach)
                    double min = Math.Min(btn.ActualWidth, btn.ActualHeight);
                    btn.FontSize = min * 0.5; // Adjust multiplier as needed
                };
                Grid.SetRow(button, row);
                Grid.SetColumn(button, col);
                grid.Children.Add(button);
            }
        }
    }
}
