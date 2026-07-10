using Microsoft.UI.Xaml.Media.Animation;

namespace ibc2025;

public sealed partial class QuestionBoardPage : Page
{
	public QuestionBoardPage()
	{
		
		InitializeComponent();
		CategoryTitle.Text = App.CategoryNames[(int)App.Category];
		PageBackground.Background = App.DailyBackground;
		App.SlideInAnimation("X", TimeSpan.FromSeconds(0.5), RootGrid, MainTransform);
		FillGridWithButtons();
		Region1Pts.Text = App.TeamPtsDsply[0].ToString();
		Region2Pts.Text = App.TeamPtsDsply[1].ToString();
		Region3Pts.Text = App.TeamPtsDsply[2].ToString();
		Region4Pts.Text = App.TeamPtsDsply[3].ToString();
		Region5Pts.Text = App.TeamPtsDsply[4].ToString();
		Region6Pts.Text = App.TeamPtsDsply[5].ToString();
		Region7Pts.Text = App.TeamPtsDsply[6].ToString();
		Region8Pts.Text = App.TeamPtsDsply[7].ToString();
		Region1Watcher();
		Region2Watcher();
		Region3Watcher();
		Region4Watcher();
		Region5Watcher();
		Region6Watcher();
		Region7Watcher();
		Region8Watcher();
		if (Window.Current is Window currentWindow)
		{
			currentWindow.SizeChanged += RebuildQuestionGridOnResize;
		}
		Unloaded += QuestionBoardPageUnloaded;
	}

	private void RebuildQuestionGridOnResize(object sender, WindowSizeChangedEventArgs e)
	{
		FillGridWithButtons();
	}

	private void QuestionBoardPageUnloaded(object sender, RoutedEventArgs e)
	{
		if (Window.Current is Window currentWindow)
		{
			currentWindow.SizeChanged -= RebuildQuestionGridOnResize;
		}
		Unloaded -= QuestionBoardPageUnloaded;
	}

	private void FillGridWithButtons()
	{
		List<Question>? questions = App.Questions[(int)App.Category];
		if (questions is null)
		{
			QuestionBoard.Children.Clear();
			QuestionBoard.RowDefinitions.Clear();
			QuestionBoard.ColumnDefinitions.Clear();
			QuestionBoard.Width = 0;
			QuestionBoard.Height = 0;
			QuestionsRemaining.Text = "0 questions remaining";
			return;
		}

		QuestionsRemaining.Text = $"{questions.Count(question => !question.Used)} questions remaining";

		QuestionBoard.Children.Clear();
		QuestionBoard.RowDefinitions.Clear();
		QuestionBoard.ColumnDefinitions.Clear();

		if (questions.Count == 0)
		{
			QuestionBoard.Width = 0;
			QuestionBoard.Height = 0;
			return;
		}

		const int cols = 10;
		int rows = (int)Math.Ceiling((double)questions.Count / cols);

		double fallbackWidth = Window.Current?.Bounds.Width ?? 1024;
		double fallbackHeight = Window.Current?.Bounds.Height ?? 768;
		double availableWidth = QuestionBoardContainer.ActualWidth > 0
			? QuestionBoardContainer.ActualWidth
			: Math.Max(1, fallbackWidth - 360);
		double availableHeight = QuestionBoardContainer.ActualHeight > 0
			? QuestionBoardContainer.ActualHeight
			: Math.Max(1, fallbackHeight - 220);

		double cellSize = Math.Max(1, Math.Floor(Math.Min(availableWidth / cols, availableHeight / rows)));
		QuestionBoard.Width = cellSize * cols;
		QuestionBoard.Height = cellSize * rows;

		for (int row = 0; row < rows; row++)
		{
			QuestionBoard.RowDefinitions.Add(new RowDefinition { Height = new GridLength(cellSize, GridUnitType.Pixel) });
		}

		for (int col = 0; col < cols; col++)
		{
			QuestionBoard.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(cellSize, GridUnitType.Pixel) });
		}

		for (int index = 0; index < questions.Count; index++)
		{
			int row = index / cols;
			int col = index % cols;
			int questionNumber = index + 1;
			Question question = questions[index];

            Button button = new()
            {
                Content = questionNumber,
                FontFamily = new FontFamily("Bahnschrift"),
                Name = "Q" + questionNumber,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Background = question.Used
                    ? new SolidColorBrush(Windows.UI.ColorHelper.FromArgb(255, 50, 50, 50))
                    : new SolidColorBrush(Windows.UI.ColorHelper.FromArgb(15, 50, 50, 50)),
                Style = (Style)Application.Current.Resources["AnimatedOutlineButton"],
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center
            };
            button.SizeChanged += (s, e) =>
			{
				button.FontSize = Math.Min(button.ActualWidth, button.ActualHeight) * 0.5;
			};
            button.Click += GoToQuestion;
			Grid.SetRow(button, row);
			Grid.SetColumn(button, col);
			QuestionBoard.Children.Add(button);
		}
	}

	public static void GoToQuestion(object sender, RoutedEventArgs e)
	{
		App.ActiveQuestion = int.Parse(sender.GetValue(NameProperty).ToString()[1..]);
		Button btn = (Button)sender;
		DependencyObject parent = btn;
		QuestionBoardPage page = null;
		while (parent != null && page == null)
		{
			parent = VisualTreeHelper.GetParent(parent);
			page = parent as QuestionBoardPage;
		}
		Storyboard storyboard = App.SlideOutAnimation("X", TimeSpan.FromSeconds(0.5), page.RootGrid, page.MainTransform);
		storyboard.Completed += (s, args) =>
		{
			_ = ((Frame)Window.Current.Content).Navigate(typeof(QuestionPage));
		};
		storyboard.Begin();
	}

	public void BackToCategory(object sender, RoutedEventArgs e)
	{
		Storyboard storyboard = App.SlideOutAnimation("X", TimeSpan.FromSeconds(0.5), RootGrid, MainTransform);
		storyboard.Completed += static (s, args) =>
		{
			_ = ((Frame)Window.Current.Content).Navigate(typeof(CategoryPage));
		};
		storyboard.Begin();
	}

	private void RegionIncrWrapper(object sender, RoutedEventArgs e)
	{
		RegionIncr(sender, e);
	}

	public static void RegionIncr(object sender, RoutedEventArgs e)
	{
		App.TeamPts[int.Parse(sender.GetValue(NameProperty).ToString()[..7].Replace("Region", "")) - 1] += 100 * ((int)App.Category + 1);
	}

	private void RegionDecrWrapper(object sender, RoutedEventArgs e)
	{
		RegionDecr(sender, e);
	}

	public static void RegionDecr(object sender, RoutedEventArgs e)
	{
		App.TeamPts[int.Parse(sender.GetValue(NameProperty).ToString()[..7].Replace("Region", "")) - 1] -= 100 * ((int)App.Category + 1);
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
}

