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
		// TODO: Combine these watchers into one method
		Region1Watcher();
		Region2Watcher();
		Region3Watcher();
		Region4Watcher();
		Region5Watcher();
		Region6Watcher();
		Region7Watcher();
		Region8Watcher();
	}

	private void RegionIncrWrapper(object sender, RoutedEventArgs e)
	{
		RegionIncr(sender, e);
	}

	public static void RegionIncr(object sender, RoutedEventArgs e)
	{
		App.TeamPts[int.Parse(sender.GetValue(NameProperty).ToString()[..7].Replace("Region", "")) - 1] += 100;
	}

	private void RegionDecrWrapper(object sender, RoutedEventArgs e)
	{
		RegionDecr(sender, e);
	}

	public static void RegionDecr(object sender, RoutedEventArgs e)
	{
		App.TeamPts[int.Parse(sender.GetValue(NameProperty).ToString()[..7].Replace("Region", "")) - 1] -= 100;
	}

	private async void Region1Watcher()
	{
		while (true)
		{
			if (App.TeamPtsDsply[0] != App.TeamPts[0])
			{
				int current = App.TeamPtsDsply[0];
				int target = App.TeamPts[0];
				int steps = 30;
				int delay = 15;

				for (int i = 0; i < steps; i++)
				{
					double t = (i + 1) / (double)steps;
					double eased = 1 - Math.Pow(1 - t, 3); // ease out cubic

					int interpolated = (int)(current + (target - current) * eased);
					if (interpolated != App.TeamPtsDsply[0])
					{
						App.TeamPtsDsply[0] = interpolated;
						Region1Pts.Text = interpolated.ToString();
					}

					await Task.Delay(delay);
				}

				App.TeamPtsDsply[0] = target;
				Region1Pts.Text = target.ToString(); // Ensure it's exact
			}
			else
			{
				await Task.Delay(100);
			}
		}
	}

	private async void Region2Watcher()
	{
		while (true)
		{
			if (App.TeamPtsDsply[1] != App.TeamPts[1])
			{
				int current = App.TeamPtsDsply[1];
				int target = App.TeamPts[1];
				int steps = 30;
				int delay = 15;

				for (int i = 0; i < steps; i++)
				{
					double t = (i + 1) / (double)steps;
					double eased = 1 - Math.Pow(1 - t, 3); // ease out cubic

					int interpolated = (int)(current + (target - current) * eased);
					if (interpolated != App.TeamPtsDsply[1])
					{
						App.TeamPtsDsply[1] = interpolated;
						Region2Pts.Text = interpolated.ToString();
					}

					await Task.Delay(delay);
				}

				App.TeamPtsDsply[1] = target;
				Region2Pts.Text = target.ToString(); // Ensure it's exact
			}
			else
			{
				await Task.Delay(100);
			}
		}
	}

	private async void Region3Watcher()
	{
		while (true)
		{
			if (App.TeamPtsDsply[2] != App.TeamPts[2])
			{
				int current = App.TeamPtsDsply[2];
				int target = App.TeamPts[2];
				int steps = 30;
				int delay = 15;

				for (int i = 0; i < steps; i++)
				{
					double t = (i + 1) / (double)steps;
					double eased = 1 - Math.Pow(1 - t, 3); // ease out cubic

					int interpolated = (int)(current + (target - current) * eased);
					if (interpolated != App.TeamPtsDsply[2])
					{
						App.TeamPtsDsply[2] = interpolated;
						Region3Pts.Text = interpolated.ToString();
					}

					await Task.Delay(delay);
				}

				App.TeamPtsDsply[2] = target;
				Region3Pts.Text = target.ToString(); // Ensure it's exact
			}
			else
			{
				await Task.Delay(100);
			}
		}
	}

	private async void Region4Watcher()
	{
		while (true)
		{
			if (App.TeamPtsDsply[3] != App.TeamPts[3])
			{
				int current = App.TeamPtsDsply[3];
				int target = App.TeamPts[3];
				int steps = 30;
				int delay = 15;

				for (int i = 0; i < steps; i++)
				{
					double t = (i + 1) / (double)steps;
					double eased = 1 - Math.Pow(1 - t, 3); // ease out cubic

					int interpolated = (int)(current + (target - current) * eased);
					if (interpolated != App.TeamPtsDsply[3])
					{
						App.TeamPtsDsply[3] = interpolated;
						Region4Pts.Text = interpolated.ToString();
					}

					await Task.Delay(delay);
				}

				App.TeamPtsDsply[3] = target;
				Region4Pts.Text = target.ToString(); // Ensure it's exact
			}
			else
			{
				await Task.Delay(100);
			}
		}
	}

	private async void Region5Watcher()
	{
		while (true)
		{
			if (App.TeamPtsDsply[4] != App.TeamPts[4])
			{
				int current = App.TeamPtsDsply[4];
				int target = App.TeamPts[4];
				int steps = 30;
				int delay = 15;

				for (int i = 0; i < steps; i++)
				{
					double t = (i + 1) / (double)steps;
					double eased = 1 - Math.Pow(1 - t, 3); // ease out cubic

					int interpolated = (int)(current + (target - current) * eased);
					if (interpolated != App.TeamPtsDsply[4])
					{
						App.TeamPtsDsply[4] = interpolated;
						Region5Pts.Text = interpolated.ToString();
					}

					await Task.Delay(delay);
				}

				App.TeamPtsDsply[4] = target;
				Region5Pts.Text = target.ToString(); // Ensure it's exact
			}
			else
			{
				await Task.Delay(100);
			}
		}
	}

	private async void Region6Watcher()
	{
		while (true)
		{
			if (App.TeamPtsDsply[5] != App.TeamPts[5])
			{
				int current = App.TeamPtsDsply[5];
				int target = App.TeamPts[5];
				int steps = 30;
				int delay = 15;

				for (int i = 0; i < steps; i++)
				{
					double t = (i + 1) / (double)steps;
					double eased = 1 - Math.Pow(1 - t, 3); // ease out cubic

					int interpolated = (int)(current + (target - current) * eased);
					if (interpolated != App.TeamPtsDsply[5])
					{
						App.TeamPtsDsply[5] = interpolated;
						Region6Pts.Text = interpolated.ToString();
					}

					await Task.Delay(delay);
				}

				App.TeamPtsDsply[5] = target;
				Region6Pts.Text = target.ToString(); // Ensure it's exact
			}
			else
			{
				await Task.Delay(100);
			}
		}
	}

	private async void Region7Watcher()
	{
		while (true)
		{
			if (App.TeamPtsDsply[6] != App.TeamPts[6])
			{
				int current = App.TeamPtsDsply[6];
				int target = App.TeamPts[6];
				int steps = 30;
				int delay = 15;

				for (int i = 0; i < steps; i++)
				{
					double t = (i + 1) / (double)steps;
					double eased = 1 - Math.Pow(1 - t, 3); // ease out cubic

					int interpolated = (int)(current + (target - current) * eased);
					if (interpolated != App.TeamPtsDsply[6])
					{
						App.TeamPtsDsply[6] = interpolated;
						Region7Pts.Text = interpolated.ToString();
					}

					await Task.Delay(delay);
				}

				App.TeamPtsDsply[6] = target;
				Region7Pts.Text = target.ToString(); // Ensure it's exact
			}
			else
			{
				await Task.Delay(100);
			}
		}
	}

	private async void Region8Watcher()
	{
		while (true)
		{
			if (App.TeamPtsDsply[7] != App.TeamPts[7])
			{
				int current = App.TeamPtsDsply[7];
				int target = App.TeamPts[7];
				int steps = 30;
				int delay = 15;

				for (int i = 0; i < steps; i++)
				{
					double t = (i + 1) / (double)steps;
					double eased = 1 - Math.Pow(1 - t, 3); // ease out cubic

					int interpolated = (int)(current + (target - current) * eased);
					if (interpolated != App.TeamPtsDsply[7])
					{
						App.TeamPtsDsply[7] = interpolated;
						Region8Pts.Text = interpolated.ToString();
					}

					await Task.Delay(delay);
				}

				App.TeamPtsDsply[7] = target;
				Region8Pts.Text = target.ToString(); // Ensure it's exact
			}
			else
			{
				await Task.Delay(100);
			}
		}
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