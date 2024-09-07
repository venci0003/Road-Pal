using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using static RoadPal.Common.ApplicationConstants.MessagesConstants;
using System.Collections.ObjectModel;

namespace RoadPal.ViewModels
{
	using Infrastructure.Models;
	using Services;
	using Views;
	public partial class MainPageViewModel : ObservableObject
	{
		private readonly CarService _carService;

		[ObservableProperty]
		private ObservableCollection<Car>? cars;

		[ObservableProperty]
		private string? carCountMessage;

		public IAsyncRelayCommand NavigateToCreateCarPageCommand { get; }
		public IAsyncRelayCommand<Car> DeleteCarCommand { get; }

		public IAsyncRelayCommand NavigateToCarDetailsCommand { get; }

		public MainPageViewModel(CarService carService)
		{
			_carService = carService;
			NavigateToCreateCarPageCommand = new AsyncRelayCommand(NavigateToCreateCarPage);
			DeleteCarCommand = new AsyncRelayCommand<Car>(DeleteCarAsync);
			NavigateToCarDetailsCommand = new AsyncRelayCommand<Car>(NavigateToCarDetailsAsync);

			LoadItems().ConfigureAwait(false);
		}


		private async Task DeleteCarAsync(Car? car)
		{
			if (car == null)
				return;

			Cars?.Remove(car);

			await _carService.DeleteCarByIdAsync(car.CarId);

			await LoadItems();
		}

		public async Task LoadItems()
		{
			await LoadCarsAsync();
		}

		private async Task LoadCarsAsync()
		{
			IEnumerable<Car> carsFromService = await _carService.GetAllCarsAsync();
			Cars = new ObservableCollection<Car>(carsFromService);

			CarCountMessage = (Cars == null || Cars.Count == 0)
				? NoCarsMessage
				: null;
		}

		private async Task NavigateToCreateCarPage()
		{
			await Shell.Current.GoToAsync("//CreateCarPage");
		}

		private async Task NavigateToCarDetailsAsync(Car car)
		{
			if (car != null)
			{
				var carDetailsViewModel = new CarDetailsViewModel(car);

				var carDetailsPage = new CarDetailsPage(carDetailsViewModel);
				await Shell.Current.Navigation.PushAsync(carDetailsPage);
			}
		}

	}
}
