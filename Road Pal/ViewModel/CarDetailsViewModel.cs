using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RoadPal.Contracts;
using RoadPal.Infrastructure.Models;
using RoadPal.Services;
using RoadPal.Views;
using System.Collections.ObjectModel;

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


		[ObservableProperty]

		private ObservableCollection<Barcode>? barcodes;

		private int _carId;

		public IRelayCommand ScanReceiptCommand { get; }
		public IRelayCommand<Barcode> DeleteBarcodeCommand { get; }


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

			DeleteBarcodeCommand = new AsyncRelayCommand<Barcode>(DeleteBarcodeAsync);

			_carId = car.CarId;
		}

		public async Task LoadBarcodesAsync()
		{
			var barcodesFromService = await _barcodeService.GetBarcodesAsync(_carId);

			Barcodes = new ObservableCollection<Barcode>(barcodesFromService);

		}

		private async Task DeleteBarcodeAsync(Barcode? barcode)
		{
			if (barcode == null)
			{
				return;
			}

			bool deleteReceipt = await Application.Current.MainPage
	   .DisplayAlert($"Confirm Deletion", $"Are you sure you want to delete this receipt?", "Delete", "Cancel");

			if (Barcodes != null && Barcodes.Contains(barcode) && deleteReceipt)
			{
				Barcodes.Remove(barcode);

				await _barcodeService.DeleteReceiptByIdAsync(barcode.BarcodeId);
			}
		}

		private async Task ScanReceiptNavigation()
		{
			var barcodeReaderViewModel = new BarcodeReaderViewModel(_navigationService, _barcodeService, _carId);
			var barcodeReaderPage = new BarcodeReader(barcodeReaderViewModel);
			await _navigationService.NavigateToPage(barcodeReaderPage);
		}
	}
}
