using RoadPal.ViewModels;

namespace RoadPal.Views
{
	public partial class MainPage : ContentPage
	{
		private readonly MainPageViewModel _viewModel;

		public MainPage(MainPageViewModel viewModel)
		{
			InitializeComponent();
			_viewModel = viewModel;
			BindingContext = _viewModel;
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();
			await _viewModel.LoadCarsAsync();
		}

		private bool _isTapped = false;

		private async void OnFavouriteCarsTapped(object sender, EventArgs e)
		{
			if (_isTapped)
			{
				await _viewModel.ChangeToNonFavouriteCars();
				StarIcon.Source = "star_icon_lightgray.png";
				_isTapped = false;
			}
			else
			{
				await _viewModel.ChangeToFavouriteCars();
				StarIcon.Source = "star_icon_yellow.png";
				_isTapped = true;
			}

		}
	}
}
