using Microsoft.UI.Xaml.Media.Animation;
using Windows.UI;

namespace ibc2025;

public sealed partial class QuestionPage : Page
{
	private string selectedAnswer = "";
	private static readonly Color[] OptionColors = [Color.FromArgb(255, 25, 48, 115), Color.FromArgb(255, 39, 20, 82), Color.FromArgb(255, 8, 62, 71), Color.FromArgb(255, 108, 43, 112)];
	private static DispatcherTimer timer;
	private static int secondsRemaining = 20;
	public QuestionPage()
	{
		InitializeComponent();
		PageBackground.Background = App.DailyBackground;
		QuestionText.SetValue(TextBlock.TextProperty, App.Questions[App.ActiveQuestion - 1].QuestionText);
		CorrectAnswerGrid.SetValue(VisibilityProperty, Visibility.Collapsed);

		MirrorServer.QuestionCommandChanged += () =>
		{
			string[] command = MirrorServer.LastCommand.Split(":");
			_ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => MirrorServer.Commands[command[0]]((PageBackground.FindName(command[1]), new RoutedEventArgs())));
		};

		RoundNumber.Text = "Round " + (int.Parse(DateTime.Today.ToString().Split("/")[1]) - 16);
		AnswerA.DataContext = this;
		AnswerB.DataContext = this;
		AnswerC.DataContext = this;
		AnswerD.DataContext = this;
		RevealAnswerBtn.DataContext = this;
		GoToQuestionBoardBtn.DataContext = this;

		if (App.Questions[App.ActiveQuestion - 1].IsMultiChoice)
		{
			SingleAnswerGrid.SetValue(VisibilityProperty, Visibility.Collapsed);
			AnswerAText.SetValue(TextBlock.TextProperty, App.Questions[App.ActiveQuestion - 1].Options[0]);
			AnswerBText.SetValue(TextBlock.TextProperty, App.Questions[App.ActiveQuestion - 1].Options[1]);
			AnswerCText.SetValue(TextBlock.TextProperty, App.Questions[App.ActiveQuestion - 1].Options[2]);
			AnswerDText.SetValue(TextBlock.TextProperty, App.Questions[App.ActiveQuestion - 1].Options[3]);
			ResizeMulti();
			Window.Current.SizeChanged += (s, e) =>
			{
				ResizeMulti();
			};
		}
		else
		{
			MultiAnswerGrid.SetValue(VisibilityProperty, Visibility.Collapsed);
			ResizeSingle();
			Window.Current.SizeChanged += (s, e) =>
			{
				ResizeSingle();
			};
		}

		Storyboard storyboardMain = App.SlideInAnimation("X", TimeSpan.FromSeconds(0.5), RootGrid, MainTransform);
		storyboardMain.Completed += (s, args) =>
		{
			StartCountdown();
		};
		storyboardMain.Begin();
	}

	private void StartCountdown()
	{
		secondsRemaining = 20; // or any time you want
		RoundTimer.Text = "00:" + secondsRemaining.ToString("D2");

		timer = new()
		{
			Interval = TimeSpan.FromSeconds(1)
		};
		timer.Tick += (object sender, object e) =>
		{
			secondsRemaining--;
			if (secondsRemaining <= 5)
			{
				RoundTimer.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)); // Red color for low time
			}
			else
			{
				RoundTimer.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255)); // White color for normal time
			}
			RoundTimer.Text = "00:" + secondsRemaining.ToString("D2");

			if (secondsRemaining <= 0)
			{
				timer.Stop();
				if (App.Questions[App.ActiveQuestion - 1].IsMultiChoice)
				{
					AnswerSelect(RevealAnswerBtn, new RoutedEventArgs());
				}
				else
				{
					AnswerSelect(App.Questions[App.ActiveQuestion - 1].Answer == "A" ? AnswerA : AnswerB, new RoutedEventArgs());
				}
			}
		};
		timer.Start();
	}

	public void GoToQuestionBoardWrapper(object sender, RoutedEventArgs e)
	{
		GoToQuestionBoard(sender, e);
	}

	public static void GoToQuestionBoard(object sender, RoutedEventArgs e)
	{
		if (App.MasterMode)
		{
			MasterServer.SendPingToMirror(MasterServer.MirrorId, "QuestionPage.GoToQuestionBoard", sender.GetValue(NameProperty).ToString());
		}
		Storyboard storyboard = App.SlideOutAnimation("X", TimeSpan.FromSeconds(0.5), ((QuestionPage)((FrameworkElement)sender).DataContext).RootGrid, ((QuestionPage)((FrameworkElement)sender).DataContext).MainTransform);
		storyboard.Completed += static (s, args) =>
		{
			_ = ((Frame)Window.Current.Content).Navigate(typeof(QuestionBoardPage));
		};
		storyboard.Begin();
	}

	public void AnswerSelectWrapper(object sender, RoutedEventArgs e)
	{
		AnswerSelect(sender, e);
	}

	public static void AnswerSelect(object sender, RoutedEventArgs e)
	{
		if (App.MasterMode)
		{
			if (secondsRemaining > 0)
			{
				MasterServer.SendPingToMirror(MasterServer.MirrorId, "QuestionPage.AnswerSelect", "timeout");
			}
			else
			{
				MasterServer.SendPingToMirror(MasterServer.MirrorId, "QuestionPage.AnswerSelect", sender.GetValue(NameProperty).ToString());
			}
		}
		timer.Stop();

		((QuestionPage)((Button)sender).DataContext).AnswerCorrectReference.SetValue(TextBlock.TextProperty, App.Questions[App.ActiveQuestion - 1].Reference);
		string btnName;
		try
		{
			btnName = (string)((Button)sender).GetValue(NameProperty);
		}
		catch (InvalidCastException)
		{
			btnName = "timeout";
		}

		if (!App.Questions[App.ActiveQuestion - 1].IsMultiChoice)
		{
			Storyboard storyboard = App.SlideOutAnimation("X", TimeSpan.FromSeconds(0.5), ((QuestionPage)((FrameworkElement)sender).DataContext).SingleAnswerGrid, ((QuestionPage)((FrameworkElement)sender).DataContext).SingleTransform, 200);
			storyboard.Completed += (s, args) =>
			{
				((QuestionPage)((FrameworkElement)sender).DataContext).MultiAnswerGrid.SetValue(VisibilityProperty, Visibility.Collapsed);
				((QuestionPage)((FrameworkElement)sender).DataContext).SingleAnswerGrid.SetValue(VisibilityProperty, Visibility.Collapsed);
				((QuestionPage)((FrameworkElement)sender).DataContext).CorrectAnswerGrid.SetValue(VisibilityProperty, Visibility.Visible);
				((QuestionPage)((FrameworkElement)sender).DataContext).AnswerCorrect.SetValue(HorizontalContentAlignmentProperty, HorizontalAlignment.Center);
				((QuestionPage)((FrameworkElement)sender).DataContext).AnswerCorrectLabel.SetValue(TextBlock.TextProperty, "Correct Answer");
				((QuestionPage)((FrameworkElement)sender).DataContext).AnswerCorrectIcon.SetValue(VisibilityProperty, Visibility.Collapsed);
				((QuestionPage)((FrameworkElement)sender).DataContext).AnswerCorrect.SetValue(BackgroundProperty, new SolidColorBrush(OptionColors[0]));
				((QuestionPage)((FrameworkElement)sender).DataContext).AnswerCorrectText.SetValue(TextBlock.TextProperty, App.Questions[App.ActiveQuestion - 1].Answer);
				Storyboard storyboard = App.SlideInAnimation("X", TimeSpan.FromSeconds(0.5), ((QuestionPage)((FrameworkElement)sender).DataContext).CorrectAnswerGrid, ((QuestionPage)((FrameworkElement)sender).DataContext).CorrectTransform, 200);
				storyboard.Begin();
			};
			storyboard.Begin();
		}
		else
		{
			Storyboard storyboard = App.SlideOutAnimation("X", TimeSpan.FromSeconds(0.5), ((QuestionPage)((FrameworkElement)sender).DataContext).MultiAnswerGrid, ((QuestionPage)((FrameworkElement)sender).DataContext).MultiTransform, 200);
			storyboard.Completed += (s, args) =>
			{
				((QuestionPage)((FrameworkElement)sender).DataContext).MultiAnswerGrid.SetValue(VisibilityProperty, Visibility.Collapsed);
				((QuestionPage)((FrameworkElement)sender).DataContext).SingleAnswerGrid.SetValue(VisibilityProperty, Visibility.Collapsed);
				((QuestionPage)((FrameworkElement)sender).DataContext).CorrectAnswerGrid.SetValue(VisibilityProperty, Visibility.Visible);
				if (secondsRemaining > 0)
				{
					((QuestionPage)((FrameworkElement)sender).DataContext).selectedAnswer = btnName[6].ToString();
					((QuestionPage)((FrameworkElement)sender).DataContext).AnswerCorrectLabel.SetValue(TextBlock.TextProperty, ((QuestionPage)((FrameworkElement)sender).DataContext).selectedAnswer == App.Questions[App.ActiveQuestion - 1].Answer ? "Correct!" : "Incorrect!");
					((QuestionPage)((FrameworkElement)sender).DataContext).AnswerCorrectLabel.Foreground = ((QuestionPage)((FrameworkElement)sender).DataContext).selectedAnswer == App.Questions[App.ActiveQuestion - 1].Answer ? new SolidColorBrush(Color.FromArgb(255, 0, 128, 0)) : new SolidColorBrush(Color.FromArgb(255, 128, 0, 0));
				}
				else
				{
					((QuestionPage)((FrameworkElement)sender).DataContext).AnswerCorrectLabel.SetValue(TextBlock.TextProperty, "Time's up!");
					((QuestionPage)((FrameworkElement)sender).DataContext).AnswerCorrectLabel.Foreground = new SolidColorBrush(Color.FromArgb(255, 128, 0, 0));
				}

				((QuestionPage)((FrameworkElement)sender).DataContext).AnswerCorrectText.SetValue(TextBlock.TextProperty, App.Questions[App.ActiveQuestion - 1].Options[App.Questions[App.ActiveQuestion - 1].Answer[0] - 65]);
				((QuestionPage)((FrameworkElement)sender).DataContext).AnswerCorrect.SetValue(BackgroundProperty, new SolidColorBrush(OptionColors[App.Questions[App.ActiveQuestion - 1].Answer[0] - 65]));
				((QuestionPage)((FrameworkElement)sender).DataContext).AnswerCorrectIconText.Text = App.Questions[App.ActiveQuestion - 1].Answer;

				Storyboard storyboard = App.SlideInAnimation("X", TimeSpan.FromSeconds(0.5), ((QuestionPage)((FrameworkElement)sender).DataContext).CorrectAnswerGrid, ((QuestionPage)((FrameworkElement)sender).DataContext).CorrectTransform, 200);
				storyboard.Begin();
			};
			storyboard.Begin();
		}
	}

	private void ResizeMulti()
	{
		double buttonHeight = Window.Current.Bounds.Height * 0.175;
		double buttonTextWidth = (Window.Current.Bounds.Width / 2) - buttonHeight - 26;

		QuestionText.FontSize = Window.Current.Bounds.Width * 0.05;
		AnswerAText.MaxWidth = buttonTextWidth;
		AnswerAText.FontSize = buttonTextWidth * 0.07;
		AnswerA.SetValue(HeightProperty, buttonHeight);
		AnswerA.SetValue(CornerRadiusProperty, new CornerRadius(buttonHeight / 2));
		AnswerAIcon.SetValue(HeightProperty, buttonHeight - 40);
		AnswerAIcon.SetValue(WidthProperty, buttonHeight - 40);
		AnswerAIconText.FontSize = (buttonHeight - 40) * 28 / 48;

		AnswerBText.MaxWidth = buttonTextWidth;
		AnswerBText.FontSize = buttonTextWidth * 0.07;
		AnswerB.SetValue(HeightProperty, buttonHeight);
		AnswerB.SetValue(CornerRadiusProperty, new CornerRadius(buttonHeight / 2));
		AnswerBIcon.SetValue(HeightProperty, buttonHeight - 40);
		AnswerBIcon.SetValue(WidthProperty, buttonHeight - 40);
		AnswerBIconText.FontSize = (buttonHeight - 40) * 28 / 48;

		AnswerCText.MaxWidth = buttonTextWidth;
		AnswerCText.FontSize = buttonTextWidth * 0.07;
		AnswerC.SetValue(HeightProperty, buttonHeight);
		AnswerC.SetValue(CornerRadiusProperty, new CornerRadius(buttonHeight / 2));
		AnswerD.SetValue(HeightProperty, buttonHeight);
		AnswerCIcon.SetValue(HeightProperty, buttonHeight - 40);
		AnswerCIcon.SetValue(WidthProperty, buttonHeight - 40);
		AnswerCIconText.FontSize = (buttonHeight - 40) * 28 / 48;

		AnswerDText.MaxWidth = buttonTextWidth;
		AnswerDText.FontSize = buttonTextWidth * 0.07;
		AnswerD.SetValue(HeightProperty, buttonHeight);
		AnswerD.SetValue(CornerRadiusProperty, new CornerRadius(buttonHeight / 2));
		AnswerDIcon.SetValue(HeightProperty, buttonHeight - 40);
		AnswerDIcon.SetValue(WidthProperty, buttonHeight - 40);
		AnswerDIconText.FontSize = (buttonHeight - 40) * 28 / 48;

		AnswerCorrectText.MaxWidth = buttonTextWidth;
		AnswerCorrectText.FontSize = buttonTextWidth * 0.07;
		AnswerCorrect.SetValue(HeightProperty, buttonHeight);
		AnswerCorrect.SetValue(CornerRadiusProperty, new CornerRadius(buttonHeight / 2));
		AnswerCorrectIcon.SetValue(HeightProperty, buttonHeight - 40);
		AnswerCorrectIcon.SetValue(WidthProperty, buttonHeight - 40);
		AnswerCorrectIconText.FontSize = (buttonHeight - 40) * 28 / 48;
	}

	private void ResizeSingle()
	{
		double buttonHeight = Window.Current.Bounds.Height * 0.175;
		double buttonTextWidth = (Window.Current.Bounds.Width / 2) - buttonHeight - 26;

		QuestionText.FontSize = Window.Current.Bounds.Width * 0.05;
		SingleAnswerText.FontSize = Window.Current.Bounds.Width * 0.05;
		SingleAnswerDesc.FontSize = Window.Current.Bounds.Width * 0.03;

		AnswerCorrectText.MaxWidth = buttonTextWidth;
		AnswerCorrectText.FontSize = buttonTextWidth * 0.07;
		AnswerCorrect.SetValue(HeightProperty, buttonHeight);
		AnswerCorrect.SetValue(CornerRadiusProperty, new CornerRadius(buttonHeight / 2));
		AnswerCorrectIcon.SetValue(HeightProperty, buttonHeight - 40);
		AnswerCorrectIcon.SetValue(WidthProperty, buttonHeight - 40);
		AnswerCorrectIconText.FontSize = (buttonHeight - 40) * 28 / 48;
	}
}