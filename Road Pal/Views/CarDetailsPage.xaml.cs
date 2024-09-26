using RoadPal.ViewModels;

namespace RoadPal.Views;

public partial class CarDetailsPage : ContentPage
{
	private readonly CarDetailsViewModel _viewModel;

	public CarDetailsPage(CarDetailsViewModel vm)
	{
		InitializeComponent();
		_viewModel = vm;
		BindingContext = _viewModel;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await _viewModel.LoadDetailsAsync();
	}

	private void AnimateColorTransition(VisualElement element, Color startColor, Color endColor, uint duration = 500)
	{
		var animation = new Animation(v =>
		{
			if (element is Label label)
			{
				label.TextColor = new Color(
					(float)(Colors.Gray.Red + (Colors.White.Red - Colors.Gray.Red) * v),
					(float)(Colors.Gray.Green + (Colors.White.Green - Colors.Gray.Green) * v),
					(float)(Colors.Gray.Blue + (Colors.White.Blue - Colors.Gray.Blue) * v),
					(float)(Colors.Gray.Alpha + (Colors.White.Alpha - Colors.Gray.Alpha) * v)
				);
			}
			else if (element is Frame frame)
			{
				element.BackgroundColor = new Color(
					(float)(startColor.Red + (endColor.Red - startColor.Red) * v),
					(float)(startColor.Green + (endColor.Green - startColor.Green) * v),
					(float)(startColor.Blue + (endColor.Blue - startColor.Blue) * v),
					(float)(startColor.Alpha + (endColor.Alpha - startColor.Alpha) * v)
				);
			}
		}, 0, 1);

		animation.Commit(element, "ColorAnimation", length: duration, easing: Easing.Linear);
	}

	private bool _isInFinishedSection = false;

	private async void OnFinishedTapped(object sender, EventArgs e)
	{
		if (_isInFinishedSection) return;

		_isInFinishedSection = true;

		await ServiceNotesCollectionView.FadeTo(0, 400);

		AnimateColorTransition(UnfinishedFrame, UnfinishedFrame.BackgroundColor, Color.FromArgb("#3A4755"));
		AnimateColorTransition(FinishedFrame, FinishedFrame.BackgroundColor, Color.FromArgb("#171E25"));

		AnimateColorTransition(UnfinishedLabel, UnfinishedLabel.TextColor, Colors.Gray, 1000);
		AnimateColorTransition(FinishedLabel, FinishedLabel.TextColor, Colors.White, 1000);

		await _viewModel.ChangeToUnfinished();

		await ServiceNotesCollectionView.FadeTo(1, 400);
	}

	private async void OnUnfinishedTapped(object sender, EventArgs e)
	{
		if (!_isInFinishedSection)
		{
			return;
		}

		_isInFinishedSection = false;

		await ServiceNotesCollectionView.FadeTo(0, 400);

		AnimateColorTransition(UnfinishedFrame, UnfinishedFrame.BackgroundColor, Color.FromArgb("#171E25"));
		AnimateColorTransition(FinishedFrame, FinishedFrame.BackgroundColor, Color.FromArgb("#3A4755"));

		AnimateColorTransition(UnfinishedLabel, UnfinishedLabel.TextColor, Colors.White, 1000);
		AnimateColorTransition(FinishedLabel, FinishedLabel.TextColor, Colors.Gray, 1000);

		await _viewModel.ChangeToFinished();

		await ServiceNotesCollectionView.FadeTo(1, 400);
	}
}