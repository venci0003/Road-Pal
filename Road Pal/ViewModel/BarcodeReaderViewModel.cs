﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZXing.PDF417.Internal;

namespace RoadPal.ViewModels
{
	public partial class BarcodeReaderViewModel : ObservableObject
	{
		public IAsyncRelayCommand GoBackToPreviousPage { get; }

		public IAsyncRelayCommand<string> BarcodeScannedCommand { get; }

		public BarcodeReaderViewModel()
		{
			GoBackToPreviousPage = new AsyncRelayCommand(GoBack);

			BarcodeScannedCommand = new AsyncRelayCommand<string>(BarcodeScannedAsync);

		}

		private async Task GoBack()
		{
			await Shell.Current.GoToAsync("///HomePage");
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
