using RoadPal.Contracts;
using RoadPal.Services;
using RoadPal.ViewModels;

namespace RoadPal.Views
{
	public partial class MainPage : ContentPage
	{
		private readonly MainPageViewModel _viewModel;

		public MainPage(CarService carService, INavigationService _navigationService)
		{
			InitializeComponent();
			_viewModel = new MainPageViewModel(carService, _navigationService);
			BindingContext = _viewModel;
		}
		protected override async void OnAppearing()
		{
			base.OnAppearing();
			await _viewModel.LoadItems();
		}
	}
}
