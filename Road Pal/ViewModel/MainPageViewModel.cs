using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RoadPal.Infrastructure.Models;
using RoadPal.Services;
using System.Collections.ObjectModel;

namespace RoadPal.ViewModels
{
	public partial class MainPageViewModel : ObservableObject
	{
		private readonly CarService _carService;

		[ObservableProperty]
		private ObservableCollection<Car>? cars;

		[ObservableProperty]
		private string? noCarsMessage;

		public IAsyncRelayCommand NavigateToCreateCarPageCommand { get; }
		public IAsyncRelayCommand<Car> DeleteCarCommand { get; }
		public MainPageViewModel(CarService carService)
		{
			_carService = carService;
			NavigateToCreateCarPageCommand = new AsyncRelayCommand(NavigateToCreateCarPage);
			DeleteCarCommand = new AsyncRelayCommand<Car>(DeleteCarAsync);
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

			NoCarsMessage = (Cars == null || Cars.Count == 0)
				? "No cars yet!\n Add a new one to get started."
				: null;
		}

		private async Task NavigateToCreateCarPage()
		{
			await Shell.Current.GoToAsync("//CreateCarPage");
		}
	}
}
