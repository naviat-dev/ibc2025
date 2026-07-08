using Microsoft.UI.Xaml.Media.Animation;
namespace ibc2025;

public sealed partial class CategoryPage : Page
{
	public CategoryPage()
	{
		InitializeComponent();
		PageBackground.Background = App.DailyBackground;
		App.SlideInAnimation("X", TimeSpan.FromSeconds(0.5), RootGrid, MainTransform);
		WisdomsTrueEndingQuestionCount.Text = $"{App.Questions[(int)App.CategoryTypes.WisdomsTrueEnding].Count(question => !question.Used)} questions remaining";
		WisdomAppliedQuestionCount.Text = $"{App.Questions[(int)App.CategoryTypes.WisdomApplied].Count(question => !question.Used)} questions remaining";
		WordsLeftUnspokenQuestionCount.Text = $"{App.Questions[(int)App.CategoryTypes.WordsLeftUnspoken].Count(question => !question.Used)} questions remaining";
		WisdomSaysQuestionCount.Text = $"{App.Questions[(int)App.CategoryTypes.WisdomSays].Count(question => !question.Used)} questions remaining";
		LocateTheWisdomQuestionCount.Text = $"{App.Questions[(int)App.CategoryTypes.LocateTheWisdom].Count(question => !question.Used)} questions remaining";
		Region1Pts.Text = App.TeamPtsDsply[0].ToString();
		Region2Pts.Text = App.TeamPtsDsply[1].ToString();
		Region3Pts.Text = App.TeamPtsDsply[2].ToString();
		Region4Pts.Text = App.TeamPtsDsply[3].ToString();
		Region5Pts.Text = App.TeamPtsDsply[4].ToString();
		Region6Pts.Text = App.TeamPtsDsply[5].ToString();
		Region7Pts.Text = App.TeamPtsDsply[6].ToString();
		Region8Pts.Text = App.TeamPtsDsply[7].ToString();
	}
	
	private void EnterCategory(object sender, RoutedEventArgs e)
	{
		if (sender is not FrameworkElement element)
		{
			Console.WriteLine("Sender is not a FrameworkElement.");
			return;
		}

		switch (element.Name)
		{
			case "WisdomsTrueEnding":
				App.Category = App.CategoryTypes.WisdomsTrueEnding;
				break;
			case "WisdomApplied":
				App.Category = App.CategoryTypes.WisdomApplied;
				break;
			case "WordsLeftUnspoken":
				App.Category = App.CategoryTypes.WordsLeftUnspoken;
				break;
			case "WisdomSays":
				App.Category = App.CategoryTypes.WisdomSays;
				break;
			case "LocateTheWisdom":
				App.Category = App.CategoryTypes.LocateTheWisdom;
				break;
		}

		Storyboard storyboard = App.SlideOutAnimation("X", TimeSpan.FromSeconds(0.5), RootGrid, MainTransform);
		storyboard.Completed += static (s, args) =>
		{
			_ = ((Frame)Window.Current.Content).Navigate(typeof(QuestionBoardPage));
		};
		storyboard.Begin();
	}
}