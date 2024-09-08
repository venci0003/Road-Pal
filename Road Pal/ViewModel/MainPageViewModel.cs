using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using static RoadPal.Common.ApplicationConstants.MessagesConstants;
using System.Collections.ObjectModel;

namespace RoadPal.ViewModels
{
	using Infrastructure.Models;
	using RoadPal.Contracts;
	using Services;
	using Views;
	public partial class MainPageViewModel : ObservableObject
	{
		private readonly CarService _carService;

		private readonly INavigationService _navigationService;


		[ObservableProperty]
		private ObservableCollection<Car>? cars;

		[ObservableProperty]
		private string? carCountMessage;

		public IAsyncRelayCommand NavigateToCreateCarPageCommand { get; }
		public IAsyncRelayCommand<Car> DeleteCarCommand { get; }

		public IAsyncRelayCommand NavigateToCarDetailsCommand { get; }

		public MainPageViewModel(CarService carService, INavigationService navigationService)
		{
			_carService = carService;
			_navigationService = navigationService;
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
			var createCarViewModel = new CreateCarViewModel(_carService, _navigationService);
			var createCarPage = new CreateCarPage(createCarViewModel);
			await _navigationService.NavigateToPage(createCarPage);
		}


		private async Task NavigateToCarDetailsAsync(Car? car)
		{
			if (car != null)
			{
				var carDetailsViewModel = new CarDetailsViewModel(car, _navigationService);
				var carDetailsPage = new CarDetailsPage(carDetailsViewModel);
				await _navigationService.NavigateToPage(carDetailsPage);
			}
		}


	}
}
