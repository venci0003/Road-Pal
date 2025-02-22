using BruTile.Cache;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Caching.Memory;
using RoadPal.Contracts;
using RoadPal.Infrastructure.Models;
using System.Collections.ObjectModel;
using static RoadPal.Common.ApplicationConstants;

namespace RoadPal.ViewModels
{
	public partial class CreateCarViewModel : ObservableObject
	{
		private readonly INavigationService _navigationService;
		private readonly ICarService _carService;
		private readonly IMemoryCache _memoryCache;

		[ObservableProperty] private string? make;
		[ObservableProperty] private string? model;
		[ObservableProperty] private string? description;
		[ObservableProperty] private string? licensePlate;
		[ObservableProperty] private string? countryCode;
		[ObservableProperty] private string? carImage;
		[ObservableProperty] private string? _imageFilePath;
		[ObservableProperty] private string title = "Add Vehicle Maintenance";
		[ObservableProperty] private string previewText = "Preview";
		[ObservableProperty] private ObservableCollection<string>? carMakes;
		[ObservableProperty] private string? searchTerm;
		[ObservableProperty] private ObservableCollection<string>? carModels;
		[ObservableProperty] private bool isManualInputEnabled;
		[ObservableProperty] private ObservableCollection<string> countryCodes;
		[ObservableProperty] private string? selectedCountryCode;
		[ObservableProperty] private bool arePickersVisible = true;
		[ObservableProperty] private bool areManualInputsVisible = false;

		private string cachedKey = string.Empty;
		public IRelayCommand PickImageCommand { get; }
		public IRelayCommand SaveCarCommand { get; }
		public IAsyncRelayCommand SearchMakesCommand { get; }
		public IAsyncRelayCommand LoadCarModelsCommand { get; }

		public CreateCarViewModel(ICarService carService, INavigationService navigationService, IMemoryCache memoryCacheContext, string cacheKey)
		{
			_carService = carService;
			_navigationService = navigationService;
			_memoryCache = memoryCacheContext;

			PickImageCommand = new AsyncRelayCommand(PickImageAsync);
			SaveCarCommand = new AsyncRelayCommand(SaveCarAsync);
			SearchMakesCommand = new AsyncRelayCommand<string?>(SearchCarMakesAsync);
			LoadCarModelsCommand = new AsyncRelayCommand<string?>(LoadCarModelsAsync);

			CountryCodes = DataConstants.CountryCodesConstant;
			cachedKey = cacheKey;

			LoadCarMakes().ConfigureAwait(false);
		}

		private string? selectedMake;
		public string? SelectedMake
		{
			get => selectedMake;
			set
			{
				SetProperty(ref selectedMake, value);
				if (!string.IsNullOrEmpty(selectedMake) && !IsManualInputEnabled)
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
				if (!IsManualInputEnabled)
				{
					Model = selectedModel;
				}
			}
		}

		private string? manualMakeInput;
		public string? ManualMakeInput
		{
			get => manualMakeInput;
			set
			{
				SetProperty(ref manualMakeInput, value);
				if (IsManualInputEnabled)
				{
					Make = manualMakeInput;
				}
			}
		}

		private string? manualModelInput;
		public string? ManualModelInput
		{
			get => manualModelInput;
			set
			{
				SetProperty(ref manualModelInput, value);
				if (IsManualInputEnabled)
				{
					Model = manualModelInput;
				}
			}
		}

		partial void OnIsManualInputEnabledChanged(bool value)
		{
			ArePickersVisible = !value;
			AreManualInputsVisible = value;

			if (value)
			{
				Make = manualMakeInput;
				Model = manualModelInput;
			}
			else
			{
				Make = selectedMake;
				Model = selectedModel;
			}
		}

		private async Task LoadCarMakes()
		{
			var makes = await _carService.GetCarMakesAsync();
			CarMakes = new ObservableCollection<string>(makes);
		}

		private async Task LoadCarModelsAsync(string? make)
		{
			if (!string.IsNullOrEmpty(make))
			{
				var models = await _carService.GetCarModelsForMakeAsync(make);
				CarModels = models != null && models.Count > 0
					? new ObservableCollection<string>(models)
					: null;
			}
		}

		private async Task SearchCarMakesAsync(string? searchTerm)
		{
			if (string.IsNullOrWhiteSpace(searchTerm))
				return;

			var filteredMakes = await _carService.SearchCarMakesAsync(searchTerm);
			CarMakes = filteredMakes != null && filteredMakes.Count > 0
				? new ObservableCollection<string>(filteredMakes)
				: null;
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
					CarImage = _imageFilePath;
				}
			}
			catch (Exception ex)
			{
				await Application.Current.MainPage.DisplayAlert("Error", $"Failed to pick image: {ex.Message}", "OK");
			}
		}

		private async Task SaveCarAsync()
		{
			if (string.IsNullOrWhiteSpace(make) && string.IsNullOrWhiteSpace(manualMakeInput) ||
				string.IsNullOrWhiteSpace(model) && string.IsNullOrWhiteSpace(manualModelInput) ||
				string.IsNullOrWhiteSpace(licensePlate) ||
				string.IsNullOrWhiteSpace(_imageFilePath) ||
				string.IsNullOrWhiteSpace(selectedCountryCode))
			{
				await Application.Current.MainPage.DisplayAlert("Error", "Please fill in all fields and select an image.", "OK");
				return;
			}


			var newCar = new Car
			{
				LicensePlate = licensePlate!,
				CountryCodeForLicensePlate = selectedCountryCode!,
				ImagePath = _imageFilePath!,
				TotalMoneySpent = 0.0m,
				IsFavourite = false,
				Make = IsManualInputEnabled ? manualMakeInput! : make!,
				Model = IsManualInputEnabled ? manualModelInput! : model!
			};

			if (_memoryCache.TryGetValue(cachedKey, out ObservableCollection<Car>? cachedCars))
			{
				cachedCars?.Add(newCar);

				_memoryCache.Set(cachedKey, cachedCars);
			}

			await _carService.AddCarAsync(newCar);
			await Application.Current.MainPage.DisplayAlert("Success", "Car added successfully!", "OK");
			await _navigationService.GoBack();
		}
	}
}
