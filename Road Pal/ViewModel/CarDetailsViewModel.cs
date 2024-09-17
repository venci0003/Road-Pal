using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RoadPal.Contracts;
using RoadPal.Infrastructure.Models;
using RoadPal.Views;
using System.Collections.ObjectModel;
using static RoadPal.Common.ApplicationConstants.MessagesConstants;

namespace RoadPal.ViewModels
{
	public partial class CarDetailsViewModel : ObservableObject
	{
		private readonly INavigationService _navigationService;

		private readonly IBarcodeService _barcodeService;

		private readonly ICarService _carService;

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
		private decimal? totalMoneySpent;

		[ObservableProperty]

		private ObservableCollection<Barcode>? barcodes;

		private int _carId;

		public IRelayCommand ScanReceiptCommand { get; }
		public IRelayCommand<Barcode> DeleteBarcodeCommand { get; }


		public CarDetailsViewModel(Car car, INavigationService navigationServiceContext, IBarcodeService barcodeServiceContext, ICarService carServiceContext)
		{
			_navigationService = navigationServiceContext;
			_barcodeService = barcodeServiceContext;
			_carService = carServiceContext;
			carImage = car.ImagePath;
			make = car.Make;
			model = car.Model;
			licensePlate = car.LicensePlate;
			description = car.Description;
			countryCode = car.CountryCodeForLicensePlate;
			totalMoneySpent = car.TotalMoneySpent;

			ScanReceiptCommand = new AsyncRelayCommand(ScanReceiptNavigation);

			DeleteBarcodeCommand = new AsyncRelayCommand<Barcode>(DeleteBarcodeAsync);

			_carId = car.CarId;
		}

		public async Task LoadDetailsAsync()
		{
			var barcodesFromService = await _barcodeService.GetBarcodesAsync(_carId);

			Barcodes = new ObservableCollection<Barcode>(barcodesFromService);

			var carMoneyUpdate = await _carService.GetCarByIdAsync(_carId);

			if (carMoneyUpdate != null)
			{
				TotalMoneySpent = carMoneyUpdate.TotalMoneySpent;
			}
		}


		private async Task DeleteBarcodeAsync(Barcode? barcode)
		{
			if (barcode == null)
			{
				return;
			}

			bool deleteReceipt = await Application.Current.MainPage
	   .DisplayAlert(ConfirmDeleteTitleMessage, DeleteReceiptMessage, "Delete", "Cancel");

			if (Barcodes != null && Barcodes.Contains(barcode) && deleteReceipt)
			{
				Barcodes.Remove(barcode);

				await _barcodeService.DeleteReceiptByIdAsync(barcode.BarcodeId);
			}
		}

		private async Task ScanReceiptNavigation()
		{
			var barcodeReaderViewModel = new BarcodeReaderViewModel(_navigationService, _barcodeService, _carService, _carId);
			var barcodeReaderPage = new BarcodeReader(barcodeReaderViewModel);
			await _navigationService.NavigateToPage(barcodeReaderPage);
		}
	}
}
