using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RoadPal.Contracts;
using RoadPal.Infrastructure.Models;
using RoadPal.Services;
using RoadPal.Views;

namespace RoadPal.ViewModels
{
	public partial class CarDetailsViewModel : ObservableObject
	{
		private readonly INavigationService _navigationService;

		private readonly BarcodeService _barcodeService;

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

		public IRelayCommand ScanReceiptCommand { get; }


		public CarDetailsViewModel(Car car, INavigationService navigationService, BarcodeService context)
		{
			_navigationService = navigationService;
			_barcodeService = context;
			carImage = car.ImagePath;
			make = car.Make;
			model = car.Model;
			licensePlate = car.LicensePlate;
			description = car.Description;
			countryCode = car.CountryCodeForLicensePlate;

			ScanReceiptCommand = new AsyncRelayCommand(ScanReceiptNavigation);
		}

		private async Task ScanReceiptNavigation()
		{
			var barcodeReaderViewModel = new BarcodeReaderViewModel(_navigationService, _barcodeService);
			var barcodeReaderPage = new BarcodeReader(barcodeReaderViewModel);
			await _navigationService.NavigateToPage(barcodeReaderPage);
		}
	}
}
