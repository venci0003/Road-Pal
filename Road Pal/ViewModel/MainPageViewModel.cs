using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using static RoadPal.Common.ApplicationConstants.MessagesConstants;
using System.Collections.ObjectModel;

namespace RoadPal.ViewModels
{
	using Infrastructure.Models;
	using Microsoft.Extensions.Caching.Memory;
	using RoadPal.Contracts;
	using Views;
	public partial class MainPageViewModel : ObservableObject
	{
		private readonly IMemoryCache _memoryCache;

		private readonly ICarService _carService;

		private readonly INavigationService _navigationService;

		private readonly IBarcodeService _barcodeService;

		private readonly INoteService _noteService;

		private readonly ITrackingService _trackingService;

		[ObservableProperty]
		private ObservableCollection<Car>? cars;

		[ObservableProperty]
		private string? carCountMessage;

		[ObservableProperty]
		private bool isBusy;

		private string _searchQuery = string.Empty;

		public string SearchQuery
		{
			get => _searchQuery;
			set
			{
				SetProperty(ref _searchQuery, value);
				if (string.IsNullOrWhiteSpace(value))
				{
					_ = LoadCarsAsync();
				}
				else
				{
					_ = LoadCarsAsync();
				}
			}
		}

		[ObservableProperty]
		public bool isFavourite;

		public IAsyncRelayCommand NavigateToCreateCarPageCommand { get; }
		public IAsyncRelayCommand<Car> DeleteCarCommand { get; }
		public IAsyncRelayCommand NavigateToCarDetailsCommand { get; }

		public IAsyncRelayCommand GetFavouriteCarsCommand { get; }

		public IAsyncRelayCommand<Car> ChangeFavouritismStatusCommand { get; }

		public MainPageViewModel(ICarService carService,
			INavigationService navigationService,
			IBarcodeService barcodeService,
			INoteService noteServiceContext,
			ITrackingService trackingServiceContext,
			IMemoryCache memoryCacheContext)
		{
			_carService = carService;
			_navigationService = navigationService;
			_barcodeService = barcodeService;
			NavigateToCreateCarPageCommand = new AsyncRelayCommand(NavigateToCreateCarPage);
			DeleteCarCommand = new AsyncRelayCommand<Car>(DeleteCarAsync);
			NavigateToCarDetailsCommand = new AsyncRelayCommand<Car>(NavigateToCarDetailsAsync);
			GetFavouriteCarsCommand = new AsyncRelayCommand(ChangeToFavouriteCars);
			ChangeFavouritismStatusCommand = new AsyncRelayCommand<Car>(ChangeFavouritismStatus);
			_noteService = noteServiceContext;
			_trackingService = trackingServiceContext;
			_memoryCache = memoryCacheContext;
		}

		public async Task ChangeFavouritismStatus(Car? car)
		{
			if (car == null)
			{
				return;
			}

			if (car.IsFavourite == false)
			{
				await _carService.ChangeFavouritismAsync(car.CarId, true);
			}
			else if (car.IsFavourite == true)
			{
				await _carService.ChangeFavouritismAsync(car.CarId, false);
			}
			await LoadCarsAsync();
		}

		public async Task ChangeToFavouriteCars()
		{
			isFavourite = true;

			await LoadCarsAsync();
		}

		public async Task ChangeToNonFavouriteCars()
		{
			isFavourite = false;

			await LoadCarsAsync();
		}

		private async Task DeleteCarAsync(Car? car)
		{
			if (car == null)
				return;

			Cars?.Remove(car);

			await _carService.DeleteCarByIdAsync(car.CarId);

			await LoadCarsAsync();
		}

		public async Task LoadCarsAsync()
		{
			// Construct a cache key based on search query and favourite status
			string cacheKey = $"{_searchQuery}_{isFavourite}";

			// Try to get the cars from the cache first
			if (!_memoryCache.TryGetValue(cacheKey, out ObservableCollection<Car> cachedCars))
			{
				// If the cars are not in the cache, fetch from the service
				IEnumerable<Car> carsFromService = await _carService.GetAllCarsAsync(_searchQuery, isFavourite);

				// If cars are fetched, cache them for later use
				cachedCars = new ObservableCollection<Car>(carsFromService);

				// Set cache options (you can define an expiration time here)
				var cacheOptions = new MemoryCacheEntryOptions
				{
					AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) // Cache for 10 minutes, for example
				};

				// Store the fetched cars in the cache
				_memoryCache.Set(cacheKey, cachedCars, cacheOptions);
			}

			// Set the cached cars (or the newly fetched ones) to the Cars property
			Cars = cachedCars;

			// Set the car count message
			CarCountMessage = (Cars == null || Cars.Count == 0)
				? NoCarsMessage
				: null;
		}


		private async Task NavigateToCreateCarPage()
		{
			if (IsBusy)
				return;

			IsBusy = true;

			try
			{

				var createCarViewModel = new CreateCarViewModel(_carService, _navigationService);
				var createCarPage = new CreateCarPage(createCarViewModel);
				await _navigationService.NavigateToPage(createCarPage);
			}
			finally
			{
				IsBusy = false;
			}
		}


		private async Task NavigateToCarDetailsAsync(Car? car)
		{
			if (IsBusy)
				return;

			IsBusy = true;

			try
			{
				if (car != null)
				{
					var carDetailsViewModel = new CarDetailsViewModel(car, _navigationService, _barcodeService, _carService, _noteService, _trackingService);
					var carDetailsPage = new CarDetailsPage(carDetailsViewModel);
					await _navigationService.NavigateToPage(carDetailsPage);
				}
			}
			finally
			{

				IsBusy = false;
			}
		}
	}
}
