using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RoadPal.Infrastructure.Models;
using RoadPal.Services;

namespace RoadPal.ViewModels
{
	public partial class CreateCarViewModel : ObservableObject
	{
		[ObservableProperty]
		private string make;

		[ObservableProperty]
		private string model;

		[ObservableProperty]
		private string description;

		[ObservableProperty]
		private string licensePlate;

		[ObservableProperty]
		private string carImage;

		private string _imageFilePath;

		private readonly CarService _carService;

		public IRelayCommand PickImageCommand { get; }
		public IRelayCommand SaveCarCommand { get; }

		public CreateCarViewModel(CarService carService)
		{
			_carService = carService;

			PickImageCommand = new RelayCommand(async () => await PickImageAsync());
			SaveCarCommand = new RelayCommand(async () => await SaveCarAsync());
		}

		private async Task PickImageAsync()
		{
			try
			{
				var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
				{
					Title = "Pick an image for the car"
				});

				if (result != null)
				{
					_imageFilePath = result.FullPath;
					carImage = _imageFilePath;
				}
			}
			catch (Exception ex)
			{
				await Application.Current.MainPage.DisplayAlert("Error", $"Failed to pick image: {ex.Message}", "OK");
			}
		}

		private async Task SaveCarAsync()
		{
			if (string.IsNullOrWhiteSpace(make) ||
	   string.IsNullOrWhiteSpace(model) ||
	   string.IsNullOrWhiteSpace(description) ||
	   string.IsNullOrWhiteSpace(licensePlate) ||
	   string.IsNullOrWhiteSpace(_imageFilePath))
			{
				await Application.Current.MainPage.DisplayAlert("Error", "Please fill in all fields and select an image.", "OK");
				return;
			}

			var newCar = new Car
			{
				Make = make,
				Model = model,
				Description = description,
				LicensePlate = licensePlate,
				ImagePath = _imageFilePath
			};

			await _carService.AddCarAsync(newCar);

			await Application.Current.MainPage.DisplayAlert("Success", "Car added successfully!", "OK");

			await Shell.Current.GoToAsync("///HomePage");

		}
	}
}
