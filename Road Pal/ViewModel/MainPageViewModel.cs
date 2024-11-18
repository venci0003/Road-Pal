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

			string cacheKey = $"{_searchQuery}_{isFavourite}";
			_memoryCache.Remove(cacheKey);

			await _carService.DeleteCarByIdAsync(car.CarId);

			await LoadCarsAsync();
		}

		public async Task LoadCarsAsync()
		{
			string cacheKey = $"{_searchQuery}_{isFavourite}";

			if (!_memoryCache.TryGetValue(cacheKey, out ObservableCollection<Car> cachedCars))
			{
				IEnumerable<Car> carsFromService = await _carService.GetAllCarsAsync(_searchQuery, isFavourite);

				cachedCars = new ObservableCollection<Car>(carsFromService);

				var cacheOptions = new MemoryCacheEntryOptions
				{
					AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
				};

				_memoryCache.Set(cacheKey, cachedCars, cacheOptions);
			}

			Cars = cachedCars;

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
