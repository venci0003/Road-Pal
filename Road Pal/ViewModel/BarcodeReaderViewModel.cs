using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RoadPal.Contracts;
using static RoadPal.Common.ApplicationConstants.MessagesConstants;

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

			string receiptInfoMessage = string.Empty;

			string alertMessageTitle = string.Empty;

			if (splittedInformation.Length == 5)
			{
				var (serialNumber, vatId, dateOfIssue, timeOfIssue, totalAmount) =
					(splittedInformation[0], splittedInformation[1], splittedInformation[2], splittedInformation[3], splittedInformation[4]);

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
				await Application.Current.MainPage.DisplayAlert($"{alertMessageTitle}", $"{receiptInfoMessage}", "Ok");
			});
		}
	}
}
