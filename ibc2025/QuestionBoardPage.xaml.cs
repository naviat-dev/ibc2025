using Microsoft.UI.Xaml.Media.Animation;

namespace ibc2025;

public sealed partial class QuestionBoardPage : Page
{
	public QuestionBoardPage()
	{
		InitializeComponent();
		PageBackground.Background = App.DailyBackground;
		App.SlideInAnimation("X", TimeSpan.FromSeconds(0.5), RootGrid, MainTransform);
		FillGridWithButtons(QuestionBoard);
		Region1Pts.Text = App.TeamPtsDsply[0].ToString();
		Region2Pts.Text = App.TeamPtsDsply[1].ToString();
		Region3Pts.Text = App.TeamPtsDsply[2].ToString();
		Region4Pts.Text = App.TeamPtsDsply[3].ToString();
		Region5Pts.Text = App.TeamPtsDsply[4].ToString();
		Region6Pts.Text = App.TeamPtsDsply[5].ToString();
		Region7Pts.Text = App.TeamPtsDsply[6].ToString();
		Region1Watcher();
		Region2Watcher();
		Region3Watcher();
		Region4Watcher();
		RegionCEWatcher();
		RegionCWWatcher();
		RegionCSWatcher();
		MirrorServer.QuestionBoardCommandChanged += () =>
		{
			string[] command = MirrorServer.LastCommand.Split(":");
			_ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => MirrorServer.Commands[command[0]]((PageBackground.FindName(command[1]), new RoutedEventArgs())));
		};
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
				Button button = new()
				{
					Content = row * cols + col + 1,
					FontFamily = new FontFamily("Bahnschrift"),
					Name = "Q" + (row * cols + col + 1),
					HorizontalAlignment = HorizontalAlignment.Stretch,
					VerticalAlignment = VerticalAlignment.Stretch,
					Background = App.Questions[row * cols + col].Used ? new SolidColorBrush(Windows.UI.ColorHelper.FromArgb(255, 50, 50, 50)) : new SolidColorBrush(Windows.UI.ColorHelper.FromArgb(15, 50, 50, 50))
				};
				button.SetValue(Grid.RowProperty, row);
				button.SetValue(Grid.ColumnProperty, col);
				button.Style = (Style)Application.Current.Resources["AnimatedOutlineButton"];
				button.Click += GoToQuestion;
				button.IsEnabled = !App.Questions[row * cols + col].Used;
				button.HorizontalContentAlignment = HorizontalAlignment.Center;
				button.VerticalContentAlignment = VerticalAlignment.Center;
				button.SizeChanged += (s, e) =>
				{
					button.FontSize = Math.Min(button.ActualWidth, button.ActualHeight) * 0.5;
				};
				Grid.SetRow(button, row);
				Grid.SetColumn(button, col);
				grid.Children.Add(button);
			}
		}
	}

	public static void GoToQuestion(object sender, RoutedEventArgs e)
	{
		App.ActiveQuestion = int.Parse(sender.GetValue(NameProperty).ToString()[1..]);
		App.Questions[int.Parse(sender.GetValue(NameProperty).ToString()[1..]) - 1].Used = true;
		Button btn = (Button)sender;
		if (App.MasterMode)
		{
			MasterServer.SendPingToMirror(MasterServer.MirrorId, "QuestionBoardPage.GoToQuestion", sender.GetValue(NameProperty).ToString());
			Console.WriteLine($"Master mode: Button {btn.Name} clicked.");
		}
		else
		{
			Console.WriteLine($"Mirror mode: Button {btn.Name} clicked.");
		}
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

	private void RegionIncrWrapper(object sender, RoutedEventArgs e)
	{
		RegionIncr(sender, e);
	}

	public static void RegionIncr(object sender, RoutedEventArgs e)
	{
		if (App.MasterMode)
		{
			MasterServer.SendPingToMirror(MasterServer.MirrorId, "QuestionBoardPage.RegionIncr", sender.GetValue(NameProperty).ToString());
		}
		App.TeamPts[int.Parse(sender.GetValue(NameProperty).ToString()[..7].Replace("Region", "")) - 1] += 100;
	}

	private void RegionDecrWrapper(object sender, RoutedEventArgs e)
	{
		RegionDecr(sender, e);
	}

	public static void RegionDecr(object sender, RoutedEventArgs e)
	{
		if (App.MasterMode)
		{
			MasterServer.SendPingToMirror(MasterServer.MirrorId, "QuestionBoardPage.RegionDecr", sender.GetValue(NameProperty).ToString());
		}
		App.TeamPts[int.Parse(sender.GetValue(NameProperty).ToString()[..7].Replace("Region", "")) - 1] -= 100;
	}

	private static Storyboard QuestionBtnDeactivate(object sender)
	{
		ColorAnimation colorAnim = new()
		{
			To = Windows.UI.Color.FromArgb(255, 50, 50, 50), // or any other color
			Duration = TimeSpan.FromMilliseconds(800)
		};

		Storyboard.SetTarget(colorAnim, ((Button)sender).Background);
		Storyboard.SetTargetProperty(colorAnim, "Color");

		Storyboard storyboard = new();
		storyboard.Children.Add(colorAnim);
		return storyboard;
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

	private async void RegionCEWatcher()
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

	private async void RegionCWWatcher()
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

	private async void RegionCSWatcher()
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
}

