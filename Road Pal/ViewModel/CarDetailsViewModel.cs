using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RoadPal.Contracts;
using RoadPal.Infrastructure.Models;
using RoadPal.ViewModels;
using RoadPal.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RoadPal.ViewModels
{
	public partial class CarDetailsViewModel : ObservableObject
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

		public IRelayCommand ScanReceiptCommand { get; }


		public CarDetailsViewModel(Car car, INavigationService navigationService)
		{
			_navigationService = navigationService;

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
			try
			{
				var barcodeReaderViewModel = new BarcodeReaderViewModel(_navigationService); // Initialize this view model as needed
				var barcodeReaderPage = new BarcodeReader(barcodeReaderViewModel);
				await _navigationService.NavigateToPage(barcodeReaderPage);
			}
			catch (Exception ex)
			{
				// Handle or log the exception
				Console.WriteLine($"Navigation failed: {ex.Message}");
			}
		}

	}
}
