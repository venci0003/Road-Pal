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
}