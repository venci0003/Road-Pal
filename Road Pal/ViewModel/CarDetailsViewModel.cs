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

		private readonly INoteService _noteService;

		[ObservableProperty]

		private string? make;

		[ObservableProperty]

		private string? model;

		[ObservableProperty]

		private string? description;

		[ObservableProperty]

		private string? title;

		[ObservableProperty]

		private string? createdDate;

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

		[ObservableProperty]

		private ObservableCollection<ServiceNote>? serviceNotes;

		private int _carId;

		[ObservableProperty]
		private bool isFinished;

		[ObservableProperty]
		private bool hideCheckOffButton;

		public IRelayCommand ScanReceiptCommand { get; }
		public IRelayCommand<Barcode> DeleteBarcodeCommand { get; }
		public IRelayCommand AddServiceNoteCommand { get; }
		public IRelayCommand<ServiceNote> DeleteServiceNoteCommand { get; }
		public IRelayCommand<ServiceNote> MarkAsFinishedCommand { get; }

		public CarDetailsViewModel(Car car,
			INavigationService navigationServiceContext,
			IBarcodeService barcodeServiceContext,
			ICarService carServiceContext,
			INoteService noteServiceContext)
		{
			_navigationService = navigationServiceContext;
			_barcodeService = barcodeServiceContext;
			_carService = carServiceContext;
			_noteService = noteServiceContext;
			carImage = car.ImagePath;
			make = car.Make;
			model = car.Model;
			licensePlate = car.LicensePlate;
			description = car.Description;
			countryCode = car.CountryCodeForLicensePlate;
			totalMoneySpent = car.TotalMoneySpent;

			ScanReceiptCommand = new AsyncRelayCommand(ScanReceiptNavigation);

			DeleteBarcodeCommand = new AsyncRelayCommand<Barcode>(DeleteBarcodeAsync);

			AddServiceNoteCommand = new AsyncRelayCommand(SaveServiceNoteAsync);

			DeleteServiceNoteCommand = new AsyncRelayCommand<ServiceNote>(DeleteServiceNoteAsync);

			MarkAsFinishedCommand = new AsyncRelayCommand<ServiceNote>(ChangeServiceNoteToFinished);

			_carId = car.CarId;

			HideCheckOffButton = true;

		}

		public async Task ChangeServiceNoteToFinished(ServiceNote? serviceNote)
		{
			if (serviceNote == null)
			{
				return;
			}

			serviceNote.isFinished = true;

			await _noteService.ChangeServiceNoteToFinishedAsync(serviceNote);

			await LoadNewServiceNotes();
		}

		public async Task ChangeToUnfinished()
		{
			IsFinished = false;
			HideCheckOffButton = true;
			await LoadNewServiceNotes();
		}

		public async Task ChangeToFinished()
		{
			IsFinished = true;
			HideCheckOffButton = false;
			await LoadNewServiceNotes();
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

			await LoadNewServiceNotes();
		}

		private async Task SaveServiceNoteAsync()
		{
			if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(description))
			{
				await Application.Current.MainPage
					 .DisplayAlert(SaveServiceNoteErrorTitleMessage, SaveServiceNoteErrorDescriptionMessage, "OK");

				return;
			}
			var serviceNote = new ServiceNote()
			{
				Title = title,
				Description = description,
				CreatedDate = DateTime.UtcNow,
				isFinished = false,
				CarId = _carId,
			};

			await _noteService.AddServiceNote(serviceNote);

			await LoadNewServiceNotes();

			Title = string.Empty;
			Description = string.Empty;
		}

		private async Task LoadNewServiceNotes()
		{
			var serviceNotesFromService = await _noteService.GetAllServiceNotesAsync(_carId, IsFinished);

			ServiceNotes = new ObservableCollection<ServiceNote>(serviceNotesFromService);
		}

		private async Task DeleteServiceNoteAsync(ServiceNote? serviceNote)
		{
			if (serviceNote == null)
			{
				return;
			}

			bool deleteReceipt = await Application.Current.MainPage
	   .DisplayAlert(ConfirmDeleteTitleMessage, DeleteServiceNoteMessage, "Delete", "Cancel");

			if (ServiceNotes != null && ServiceNotes.Contains(serviceNote) && deleteReceipt)
			{
				ServiceNotes.Remove(serviceNote);

				await _noteService.DeleteServiceNoteByIdAsync(serviceNote.ServiceNoteId);
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
