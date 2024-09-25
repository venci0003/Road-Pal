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

	private void OnUnfinishedTapped(object sender, EventArgs e)
	{
		UnfinishedFrame.BackgroundColor = Color.FromArgb("#171E25");
		FinishedFrame.BackgroundColor = Color.FromArgb("#3A4755");

		// Optionally change text color
		//((Label)UnfinishedFrame.Content.FindByName("UnfinishedLabel")).TextColor = Colors.White;
		//((Label)FinishedFrame.Content.FindByName("FinishedLabel")).TextColor = Colors.Black;
	}

	private void OnFinishedTapped(object sender, EventArgs e)
	{
		UnfinishedFrame.BackgroundColor = Color.FromArgb("#3A4755");
		FinishedFrame.BackgroundColor = Color.FromArgb("#171E25");

		// Optionally change text color
		//((Label)UnfinishedFrame.Content.FindByName("UnfinishedLabel")).TextColor = Colors.Black;
		//((Label)FinishedFrame.Content.FindByName("FinishedLabel")).TextColor = Colors.White;
	}
}