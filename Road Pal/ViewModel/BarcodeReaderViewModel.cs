using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RoadPal.Contracts;
using RoadPal.Infrastructure.Models;
using static RoadPal.Common.ApplicationConstants.MessagesConstants;

namespace RoadPal.ViewModels
{
	public partial class BarcodeReaderViewModel : ObservableObject
	{
		public IAsyncRelayCommand GoBackToPreviousPage { get; }

		public IAsyncRelayCommand<string> BarcodeScannedCommand { get; }

		private readonly INavigationService _navigationService;

		private readonly IBarcodeService _barcodeService;

		private int _carId;

		public BarcodeReaderViewModel(INavigationService navigationService, IBarcodeService context, int carId)
		{
			_navigationService = navigationService;

			_barcodeService = context;

			GoBackToPreviousPage = new AsyncRelayCommand(GoBack);

			BarcodeScannedCommand = new AsyncRelayCommand<string>(BarcodeScannedAsync);

			_carId = carId;
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

			string receiptInfoMessage = string.Empty;

			string alertMessageTitle = string.Empty;

			string serialNumber = string.Empty;
			string vatId = string.Empty;
			DateTime dateOfIssue = DateTime.MinValue;
			DateTime timeOfIssue = DateTime.MinValue;
			decimal totalAmount = 0;

			if (splittedInformation.Length == 5)
			{
				serialNumber = splittedInformation[0];
				vatId = splittedInformation[1];

				if (DateTime.TryParse(splittedInformation[2], out DateTime parsedDate))
				{
					dateOfIssue = parsedDate;
				}

				if (DateTime.TryParse(splittedInformation[3], out DateTime parsedTime))
				{
					timeOfIssue = parsedTime;
				}

				if (decimal.TryParse(splittedInformation[4], out decimal parsedTotalAmount))
				{
					totalAmount = parsedTotalAmount;
				}

				receiptInfoMessage = string.Format(BarcodeInformationMessage, serialNumber, vatId, dateOfIssue, timeOfIssue, totalAmount);
				alertMessageTitle = SuccesfullyScannedBarcodeMessage;
			}
			else
			{
				receiptInfoMessage = barcodeInformation;

				alertMessageTitle = UnsuccesfullyScannedBarcodeMessage;
			}

			await Application.Current.MainPage.Dispatcher.DispatchAsync(async () =>
			{
				bool saveReceipt = await Application.Current.MainPage
				.DisplayAlert($"{alertMessageTitle}", $"{receiptInfoMessage}\n\n{SaveReceiptMessage}", "Yes", "No");

				if (saveReceipt)
				{
					Barcode barcode = new Barcode()
					{
						SerialNumber = serialNumber,
						VATId = vatId,
						DateOfIssue = dateOfIssue,
						TimeOfIssue = timeOfIssue,
						TotalAmount = totalAmount,
						CarId = _carId
					};

					await _barcodeService.SaveReceiptAsync(barcode);

					await Application.Current.MainPage
				.DisplayAlert($"Receipt saved succesfully", "You can find you receipts in car details!", "Ok");
				}
			});
		}
	}
}
