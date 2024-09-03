using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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


		public CarDetailsViewModel(Car car)
		{
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
			await Shell.Current.GoToAsync("///BarcodeReader");
		}
	}
}
