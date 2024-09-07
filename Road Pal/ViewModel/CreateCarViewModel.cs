using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RoadPal.Contracts;
using RoadPal.Infrastructure.Models;
using RoadPal.Services;
using System.Collections.ObjectModel;

namespace RoadPal.ViewModels
{
	public partial class CreateCarViewModel : ObservableObject
	{
		private readonly INavigationService _navigationService;

		[ObservableProperty]

		private string? make;

		[ObservableProperty]

		private string? model;

		[ObservableProperty]

		private string? description;

		[ObservableProperty]
		private string? licensePlate;

		[ObservableProperty]
		private string? countryCode;

		[ObservableProperty]
		private string? carImage;

		[ObservableProperty]
		private string? _imageFilePath;

		[ObservableProperty]
		private string title = "Add Vehicle Maintenance";

		private readonly CarService _carService;

		[ObservableProperty]
		private ObservableCollection<string>? carMakes;

		[ObservableProperty]
		private string? searchTerm;

		[ObservableProperty]
		private ObservableCollection<string>? carModels;

		[ObservableProperty]
		private string? manualModelInput;

		[ObservableProperty]
		private bool areManualModelInputVisible;

		public IRelayCommand PickImageCommand { get; }
		public IRelayCommand SaveCarCommand { get; }
		public IAsyncRelayCommand SearchMakesCommand { get; }
		public IAsyncRelayCommand LoadCarModelsCommand { get; }



		public CreateCarViewModel(CarService carService, INavigationService navigationService)
		{
			_carService = carService;
			_navigationService = navigationService;


			PickImageCommand = new RelayCommand(async () => await PickImageAsync());
			SaveCarCommand = new RelayCommand(async () => await SaveCarAsync());
			SearchMakesCommand = new AsyncRelayCommand<string?>(SearchCarMakesAsync);
			LoadCarModelsCommand = new AsyncRelayCommand<string?>(LoadCarModelsAsync);


			LoadCarMakes().ConfigureAwait(false);

		}

		private string? selectedMake;
		public string? SelectedMake
		{
			get => selectedMake;
			set
			{
				SetProperty(ref selectedMake, value);
				if (!string.IsNullOrEmpty(selectedMake))
				{
					Make = selectedMake;
					LoadCarModelsAsync(selectedMake).ConfigureAwait(false);
				}
			}
		}

		private string? selectedModel;
		public string? SelectedModel
		{
			get => selectedModel;
			set
			{
				SetProperty(ref selectedModel, value);
				Model = selectedModel;
			}
		}

		private async Task LoadCarModelsAsync(string? make)
		{
			CarModels?.Clear();

			if (!string.IsNullOrEmpty(make))
			{
				var models = await _carService.GetCarModelsForMakeAsync(make);

				if (models.Count == 0)
				{
					AreManualModelInputVisible = true;
					CarModels = null;
				}
				else
				{
					CarModels = new ObservableCollection<string>(models);
					AreManualModelInputVisible = false;
				}
			}
			else
			{
				CarModels = null;
				AreManualModelInputVisible = false;
			}
		}

		private async Task SearchCarMakesAsync(string? searchTerm)
		{
			if (string.IsNullOrWhiteSpace(searchTerm))
				return;

			var filteredMakes = await _carService.SearchCarMakesAsync(searchTerm);
			CarMakes = new ObservableCollection<string>(filteredMakes);
		}
		private async Task LoadCarMakes()
		{
			var makes = await _carService.GetCarMakesAsync();
			CarMakes = new ObservableCollection<string>(makes);
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
				string.IsNullOrWhiteSpace(_imageFilePath) ||
				string.IsNullOrWhiteSpace(countryCode))
			{
				await Application.Current.MainPage.DisplayAlert("Error", "Please fill in all fields and select an image.", "OK");
				return;
			}

			var newCar = new Car
			{
				Make = make!,
				Model = model!,
				Description = description!,
				LicensePlate = licensePlate!,
				CountryCodeForLicensePlate = countryCode!,
				ImagePath = _imageFilePath!
			};

			await _carService.AddCarAsync(newCar);

			await Application.Current.MainPage.DisplayAlert("Success", "Car added successfully!", "OK");

			await _navigationService.GoBack(); 

		}
	}
}
