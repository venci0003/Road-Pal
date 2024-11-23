using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Caching.Memory;
using RoadPal.Contracts;
using RoadPal.Infrastructure.Models;
using System.Collections.ObjectModel;
using static RoadPal.Common.ApplicationConstants.MessagesConstants;

namespace RoadPal.ViewModels
{
	public partial class BarcodeReaderViewModel : ObservableObject
	{
		public IAsyncRelayCommand<string> BarcodeScannedCommand { get; }

		private readonly INavigationService _navigationService;

		private readonly IBarcodeService _barcodeService;

		private readonly ICarService _carService;

		private readonly IMemoryCache _memoryCache;

		private int _carId;

		public BarcodeReaderViewModel(INavigationService navigationServiceContext,
			IBarcodeService barcodeServiceContext,
			ICarService carServiceContext,
			IMemoryCache memoryCacheContext,
			int carId)
		{
			_navigationService = navigationServiceContext;

			_barcodeService = barcodeServiceContext;

			_carService = carServiceContext;

			_memoryCache = memoryCacheContext;

			BarcodeScannedCommand = new AsyncRelayCommand<string>(BarcodeScannedAsync);

			_carId = carId;
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

					var cacheKey = $"Barcode_{_carId}";

					if (_memoryCache.TryGetValue(cacheKey, out ObservableCollection<Barcode> cachedBarcodes))
					{
						cachedBarcodes!.Add(barcode);

						_memoryCache.Set(cacheKey, cachedBarcodes, new MemoryCacheEntryOptions
						{
							AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20)
						});
					}

					await _carService.AddMoneyToTotalAsync(_carId, totalAmount);

					await Application.Current.MainPage
				.DisplayAlert($"{ReceiptSavedSuccesfullyMessage}", ReceiptSavedWhereToFindMessage, "Ok");
				}
			});
		}
	}
}
