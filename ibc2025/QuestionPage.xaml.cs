using Microsoft.UI.Xaml.Media.Animation;
using Windows.UI;

namespace ibc2025;

public sealed partial class QuestionPage : Page
{
	private string selectedAnswer = "";
	private static readonly Color[] OptionColors = [Color.FromArgb(255, 25, 48, 115), Color.FromArgb(255, 39, 20, 82), Color.FromArgb(255, 8, 62, 71), Color.FromArgb(255, 108, 43, 112)];
	private static DispatcherTimer timer;
	private static Question question;
	public QuestionPage()
	{
		InitializeComponent();
		question = App.Questions[(int)App.Category][App.ActiveQuestion - 1];
		PageBackground.Background = App.DailyBackground;
		QuestionText.SetValue(TextBlock.TextProperty, question.QuestionText);
		CorrectAnswerGrid.SetValue(VisibilityProperty, Visibility.Collapsed);

		RoundNumber.Text = $"Round {int.Parse(DateTime.Today.ToString().Split("/")[1]) - 15}";
		RoundTimer.Text = "00:" + question.Time.ToString("D2");
		AnswerA.DataContext = this;
		AnswerB.DataContext = this;
		AnswerC.DataContext = this;
		AnswerD.DataContext = this;
		RevealAnswerBtn.DataContext = this;
		GoToQuestionBoardBtn.DataContext = this;

		if (question.IsMultiChoice)
		{
			SingleAnswerGrid.SetValue(VisibilityProperty, Visibility.Collapsed);
			AnswerAText.SetValue(TextBlock.TextProperty, question.Options[0]);
			AnswerBText.SetValue(TextBlock.TextProperty, question.Options[1]);
			AnswerCText.SetValue(TextBlock.TextProperty, question.Options[2]);
			AnswerDText.SetValue(TextBlock.TextProperty, question.Options[3]);
			ResizeMulti();
			Window.Current.SizeChanged += (s, e) =>
			{
				ResizeMulti();
			};
			DispatcherQueue.TryEnqueue(ResizeMulti);
		}
		else
		{
			MultiAnswerGrid.SetValue(VisibilityProperty, Visibility.Collapsed);
			ResizeSingle();
			Window.Current.SizeChanged += (s, e) =>
			{
				ResizeSingle();
			};
			DispatcherQueue.TryEnqueue(ResizeSingle);
		}

		if (question.Used)
		{
			AnswerSelect(RevealAnswerBtn, new RoutedEventArgs());
		}
		else
		{
			Storyboard storyboardMain = App.SlideInAnimation("X", TimeSpan.FromSeconds(0.5), RootGrid, MainTransform);
			storyboardMain.Completed += (s, args) =>
			{
				StartCountdown();
			};
			storyboardMain.Begin();
		}
	}

	private void StartCountdown()
	{
		timer = new()
		{
			Interval = TimeSpan.FromSeconds(1)
		};
		timer.Tick += (object sender, object e) =>
		{
			question.Time--;
			if (question.Time <= 5)
			{
				RoundTimer.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)); // Red color for low time
			}
			else
			{
				RoundTimer.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255)); // White color for normal time
			}
			RoundTimer.Text = "00:" + question.Time.ToString("D2");

			if (question.Time <= 0)
			{
				timer.Stop();
				if (question.IsMultiChoice)
				{
					AnswerSelect(RevealAnswerBtn, new RoutedEventArgs());
				}
				else
				{
					AnswerSelect(question.Answer == "A" ? AnswerA : AnswerB, new RoutedEventArgs());
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
		timer.Stop();

		((QuestionPage)((Button)sender).DataContext).AnswerCorrectReference.SetValue(TextBlock.TextProperty, question.Reference);
		string btnName;
		try
		{
			btnName = (string)((Button)sender).GetValue(NameProperty);
		}
		catch (InvalidCastException)
		{
			btnName = "timeout";
		}

		if (!question.IsMultiChoice)
		{
			Storyboard storyboard = App.SlideOutAnimation("X", TimeSpan.FromSeconds(0.5), ((QuestionPage)((FrameworkElement)sender).DataContext).SingleAnswerGrid, ((QuestionPage)((FrameworkElement)sender).DataContext).SingleTransform, 200);
			storyboard.Completed += (s, args) =>
			{
				QuestionPage page = (QuestionPage)((FrameworkElement)sender).DataContext;
				((QuestionPage)((FrameworkElement)sender).DataContext).MultiAnswerGrid.SetValue(VisibilityProperty, Visibility.Collapsed);
				((QuestionPage)((FrameworkElement)sender).DataContext).SingleAnswerGrid.SetValue(VisibilityProperty, Visibility.Collapsed);
				((QuestionPage)((FrameworkElement)sender).DataContext).CorrectAnswerGrid.SetValue(VisibilityProperty, Visibility.Visible);
				((QuestionPage)((FrameworkElement)sender).DataContext).AnswerCorrect.SetValue(HorizontalContentAlignmentProperty, HorizontalAlignment.Center);
				((QuestionPage)((FrameworkElement)sender).DataContext).AnswerCorrectLabel.SetValue(TextBlock.TextProperty, question.Used ? "This question has\nalready been\nanswered" : "Correct Answer");
				((QuestionPage)((FrameworkElement)sender).DataContext).AnswerCorrectLabel.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 0));
				((QuestionPage)((FrameworkElement)sender).DataContext).AnswerCorrectIcon.SetValue(VisibilityProperty, Visibility.Collapsed);
				((QuestionPage)((FrameworkElement)sender).DataContext).AnswerCorrect.SetValue(BackgroundProperty, new SolidColorBrush(OptionColors[0]));
				((QuestionPage)((FrameworkElement)sender).DataContext).AnswerCorrectText.SetValue(TextBlock.TextProperty, question.Answer);
				page.DispatcherQueue.TryEnqueue(page.ResizeSingle);
				Storyboard storyboard = App.SlideInAnimation("X", TimeSpan.FromSeconds(0.5), ((QuestionPage)((FrameworkElement)sender).DataContext).CorrectAnswerGrid, ((QuestionPage)((FrameworkElement)sender).DataContext).CorrectTransform, 200);
				storyboard.Completed += (s2, args2) =>
				{
					App.Questions[(int)App.Category][App.ActiveQuestion - 1].Used = true;
				};
				storyboard.Begin();
			};
			storyboard.Begin();
		}
		else
		{
			Storyboard storyboard = App.SlideOutAnimation("X", TimeSpan.FromSeconds(0.5), ((QuestionPage)((FrameworkElement)sender).DataContext).MultiAnswerGrid, ((QuestionPage)((FrameworkElement)sender).DataContext).MultiTransform, 200);
			storyboard.Completed += (s, args) =>
			{
				QuestionPage page = (QuestionPage)((FrameworkElement)sender).DataContext;
				((QuestionPage)((FrameworkElement)sender).DataContext).MultiAnswerGrid.SetValue(VisibilityProperty, Visibility.Collapsed);
				((QuestionPage)((FrameworkElement)sender).DataContext).SingleAnswerGrid.SetValue(VisibilityProperty, Visibility.Collapsed);
				((QuestionPage)((FrameworkElement)sender).DataContext).CorrectAnswerGrid.SetValue(VisibilityProperty, Visibility.Visible);
				if (question.Used)
				{
					((QuestionPage)((FrameworkElement)sender).DataContext).AnswerCorrectLabel.SetValue(TextBlock.TextProperty, "This question has\nalready been\nanswered");
					((QuestionPage)((FrameworkElement)sender).DataContext).AnswerCorrectLabel.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 0));
				}
				else
				{
					if (question.Time > 0)
					{
						((QuestionPage)((FrameworkElement)sender).DataContext).selectedAnswer = btnName[6].ToString();
						if (((QuestionPage)((FrameworkElement)sender).DataContext).selectedAnswer == question.Answer)
						{
							((QuestionPage)((FrameworkElement)sender).DataContext).AnswerCorrectLabel.SetValue(TextBlock.TextProperty, "Correct!");
							((QuestionPage)((FrameworkElement)sender).DataContext).AnswerCorrectLabel.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 128, 0));
						}
						else
						{
							((QuestionPage)((FrameworkElement)sender).DataContext).AnswerCorrectLabel.SetValue(TextBlock.TextProperty, "Incorrect!");
							((QuestionPage)((FrameworkElement)sender).DataContext).AnswerCorrectLabel.Foreground = new SolidColorBrush(Color.FromArgb(255, 128, 0, 0));
						}
					}
					else
					{
						((QuestionPage)((FrameworkElement)sender).DataContext).AnswerCorrectLabel.SetValue(TextBlock.TextProperty, "Time's up!");
						((QuestionPage)((FrameworkElement)sender).DataContext).AnswerCorrectLabel.Foreground = new SolidColorBrush(Color.FromArgb(255, 128, 0, 0));
					}
				}

				((QuestionPage)((FrameworkElement)sender).DataContext).AnswerCorrectText.SetValue(TextBlock.TextProperty, question.Options[question.Answer[0] - 65]);
				((QuestionPage)((FrameworkElement)sender).DataContext).AnswerCorrect.SetValue(BackgroundProperty, new SolidColorBrush(OptionColors[question.Answer[0] - 65]));
				((QuestionPage)((FrameworkElement)sender).DataContext).AnswerCorrectIconText.Text = question.Answer;
				page.DispatcherQueue.TryEnqueue(page.ResizeMulti);

				Storyboard storyboard = App.SlideInAnimation("X", TimeSpan.FromSeconds(0.5), ((QuestionPage)((FrameworkElement)sender).DataContext).CorrectAnswerGrid, ((QuestionPage)((FrameworkElement)sender).DataContext).CorrectTransform, 200);
				storyboard.Completed += (s2, args2) =>
				{
					App.Questions[(int)App.Category][App.ActiveQuestion - 1].Used = true;
				};
				storyboard.Begin();
			};
			storyboard.Begin();
		}
	}

	private void ResizeMulti()
	{
		double buttonHeight = Window.Current.Bounds.Height * 0.175;
		double buttonTextWidth = (Window.Current.Bounds.Width / 2) - buttonHeight - 26;
		double optionTextMaxHeight = Math.Max(1, buttonHeight - 20);
		double optionTextMaxFontSize = Math.Max(14, buttonTextWidth * 0.07);

		ResizeQuestionTextToFit(0.05);
		ResizeTextBlockToFit(AnswerAText, question.Options[0], buttonTextWidth, optionTextMaxHeight, optionTextMaxFontSize);
		AnswerA.SetValue(HeightProperty, buttonHeight);
		AnswerA.SetValue(CornerRadiusProperty, new CornerRadius(buttonHeight / 2));
		AnswerAIcon.SetValue(HeightProperty, buttonHeight - 40);
		AnswerAIcon.SetValue(WidthProperty, buttonHeight - 40);
		AnswerAIconText.FontSize = (buttonHeight - 40) * 28 / 48;

		ResizeTextBlockToFit(AnswerBText, question.Options[1], buttonTextWidth, optionTextMaxHeight, optionTextMaxFontSize);
		AnswerB.SetValue(HeightProperty, buttonHeight);
		AnswerB.SetValue(CornerRadiusProperty, new CornerRadius(buttonHeight / 2));
		AnswerBIcon.SetValue(HeightProperty, buttonHeight - 40);
		AnswerBIcon.SetValue(WidthProperty, buttonHeight - 40);
		AnswerBIconText.FontSize = (buttonHeight - 40) * 28 / 48;

		ResizeTextBlockToFit(AnswerCText, question.Options[2], buttonTextWidth, optionTextMaxHeight, optionTextMaxFontSize);
		AnswerC.SetValue(HeightProperty, buttonHeight);
		AnswerC.SetValue(CornerRadiusProperty, new CornerRadius(buttonHeight / 2));
		AnswerD.SetValue(HeightProperty, buttonHeight);
		AnswerCIcon.SetValue(HeightProperty, buttonHeight - 40);
		AnswerCIcon.SetValue(WidthProperty, buttonHeight - 40);
		AnswerCIconText.FontSize = (buttonHeight - 40) * 28 / 48;

		ResizeTextBlockToFit(AnswerDText, question.Options[3], buttonTextWidth, optionTextMaxHeight, optionTextMaxFontSize);
		AnswerD.SetValue(HeightProperty, buttonHeight);
		AnswerD.SetValue(CornerRadiusProperty, new CornerRadius(buttonHeight / 2));
		AnswerDIcon.SetValue(HeightProperty, buttonHeight - 40);
		AnswerDIcon.SetValue(WidthProperty, buttonHeight - 40);
		AnswerDIconText.FontSize = (buttonHeight - 40) * 28 / 48;

		ResizeTextBlockToFit(AnswerCorrectText, AnswerCorrectText.Text ?? string.Empty, buttonTextWidth, optionTextMaxHeight, optionTextMaxFontSize);
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
		double optionTextMaxHeight = Math.Max(1, buttonHeight - 20);
		double optionTextMaxFontSize = Math.Max(14, buttonTextWidth * 0.07);

		ResizeQuestionTextToFit(0.05);
		SingleAnswerText.FontSize = Window.Current.Bounds.Width * 0.05;
		SingleAnswerDesc.FontSize = Window.Current.Bounds.Width * 0.03;

		ResizeTextBlockToFit(AnswerCorrectText, AnswerCorrectText.Text ?? string.Empty, buttonTextWidth, optionTextMaxHeight, optionTextMaxFontSize);
		AnswerCorrect.SetValue(HeightProperty, buttonHeight);
		AnswerCorrect.SetValue(CornerRadiusProperty, new CornerRadius(buttonHeight / 2));
		AnswerCorrectIcon.SetValue(HeightProperty, buttonHeight - 40);
		AnswerCorrectIcon.SetValue(WidthProperty, buttonHeight - 40);
		AnswerCorrectIconText.FontSize = (buttonHeight - 40) * 28 / 48;
	}

	private void ResizeQuestionTextToFit(double maxScale)
	{
		double maxFontSize = Window.Current.Bounds.Width * maxScale;
		double availableWidth = QuestionText.ActualWidth > 0
			? QuestionText.ActualWidth
			: Math.Max(1, (Window.Current.Bounds.Width / 2) - 60);
		double availableHeight = QuestionText.ActualHeight > 0
			? QuestionText.ActualHeight
			: Math.Max(1, Window.Current.Bounds.Height - 200);

		ResizeTextBlockToFit(QuestionText, question?.QuestionText ?? string.Empty, availableWidth, availableHeight, maxFontSize);
	}

	private void ResizeTextBlockToFit(TextBlock textBlock, string text, double maxWidth, double maxHeight, double maxFontSize, double minFontSize = 14)
	{
		double safeWidth = Math.Max(1, maxWidth);
		double safeHeight = Math.Max(1, maxHeight);
		double safeMinFont = Math.Max(1, minFontSize);
		double safeMaxFont = Math.Max(safeMinFont, maxFontSize);

		textBlock.Text = text;
		textBlock.MaxWidth = safeWidth;

		if (string.IsNullOrWhiteSpace(text))
		{
			textBlock.FontSize = safeMaxFont;
			return;
		}

		double low = safeMinFont;
		double high = safeMaxFont;
		double best = safeMinFont;

		for (int i = 0; i < 12; i++)
		{
			double mid = (low + high) / 2;
			if (DoesTextBlockFit(textBlock, text, mid, safeWidth, safeHeight))
			{
				best = mid;
				low = mid;
			}
			else
			{
				high = mid;
			}
		}

		textBlock.FontSize = best;
	}

	private static bool DoesTextBlockFit(TextBlock template, string text, double fontSize, double maxWidth, double maxHeight)
	{
		TextBlock measurementBlock = new()
		{
			Text = text,
			FontFamily = template.FontFamily,
			FontWeight = template.FontWeight,
			FontStyle = template.FontStyle,
			FontStretch = template.FontStretch,
			FontSize = fontSize,
			TextWrapping = template.TextWrapping,
			TextAlignment = template.TextAlignment
		};

		measurementBlock.Measure(new Windows.Foundation.Size(maxWidth, double.PositiveInfinity));
		return measurementBlock.DesiredSize.Width <= maxWidth && measurementBlock.DesiredSize.Height <= maxHeight;
	}
}