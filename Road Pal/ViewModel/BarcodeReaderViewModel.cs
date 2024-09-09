using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RoadPal.Contracts;

namespace RoadPal.ViewModels
{
	public partial class BarcodeReaderViewModel : ObservableObject
	{
		public IAsyncRelayCommand GoBackToPreviousPage { get; }

		public IAsyncRelayCommand<string> BarcodeScannedCommand { get; }

		private readonly INavigationService _navigationService;

		public BarcodeReaderViewModel(INavigationService navigationService)
		{

			_navigationService = navigationService;

			GoBackToPreviousPage = new AsyncRelayCommand(GoBack);

			BarcodeScannedCommand = new AsyncRelayCommand<string>(BarcodeScannedAsync);

		}

		private async Task GoBack()
		{
			await _navigationService.GoBack();
		}


		private async Task BarcodeScannedAsync(string? barcodeInformation)
		{
			if (string.IsNullOrEmpty(barcodeInformation))
				return;

			string[] splittedInformation = barcodeInformation.Split('*');

			string serialNumber = splittedInformation[0];

			string vatId = splittedInformation[1];

			string dateOfIssue = splittedInformation[2];

			string timeOfIssue = splittedInformation[3];

			string totalAmount = splittedInformation[4];

			string receiptInfo = $"Receipt Information:\n" +
								 $"Serial Number: {serialNumber}\n" +
								 $"VAT ID: {vatId}\n" +
								 $"Date of Issue: {dateOfIssue:yyyy-MM-dd}\n" +
								 $"Time of Issue: {timeOfIssue:HH:mm:ss}\n" +
								 $"Total Amount: {totalAmount} lv";

			await Application.Current.MainPage.Dispatcher.DispatchAsync(async () =>
			{
				await Application.Current.MainPage.DisplayAlert("Barcode scanned successfully!", $"{receiptInfo}", "Ok");
			});
		}
	}
}
