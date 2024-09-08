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

			await Application.Current.MainPage.Dispatcher.DispatchAsync(async () =>
			{
				await Application.Current.MainPage.DisplayAlert("Barcode scanned successfully!", $"{barcodeInformation}", "Ok");
			});
		}
	}
}
