using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RoadPal.Contracts;
using RoadPal.Infrastructure.Models;
using RoadPal.ViewModel;
using RoadPal.Views;
using System.Collections.ObjectModel;
using System.Text;
using static RoadPal.Common.ApplicationConstants.MessagesConstants;
using static RoadPal.Common.ApplicationConstants;
using System.Net;

namespace RoadPal.ViewModels
{
	public partial class CarDetailsViewModel : ObservableObject
	{
		private readonly INavigationService _navigationService;

		private readonly IBarcodeService _barcodeService;

		private readonly ICarService _carService;

		private readonly INoteService _noteService;

		private readonly ITrackingService _trackingService;

		[ObservableProperty]

		private string make;

		[ObservableProperty]

		private string model;

		[ObservableProperty]

		private string description;

		[ObservableProperty]
		private string title = string.Empty;

		[ObservableProperty]
		private string createdDate = DateTime.UtcNow.ToString("yyyy-MM-dd");

		[ObservableProperty]
		private string licensePlate;

		[ObservableProperty]
		private string countryCode;

		[ObservableProperty]
		private string carImage;

		[ObservableProperty]
		private decimal totalMoneySpent;

		[ObservableProperty]

		private ObservableCollection<Barcode>? barcodes;

		[ObservableProperty]

		private ObservableCollection<ServiceNote>? serviceNotes;

		private int _carId;

		[ObservableProperty]
		private bool isFinished;

		[ObservableProperty]
		private bool hideCheckOffButton;

		[ObservableProperty]
		private bool isEditing;

		[ObservableProperty]
		private bool isInfoOpen;

		[ObservableProperty]
		private string? _imageFilePath;

		[ObservableProperty]
		private string? selectedCountryCode;

		[ObservableProperty]
		private ObservableCollection<string> countryCodes;

		public IRelayCommand ScanReceiptCommand { get; }
		public IRelayCommand<Barcode> DeleteBarcodeCommand { get; }
		public IRelayCommand AddServiceNoteCommand { get; }
		public IRelayCommand<ServiceNote> DeleteServiceNoteCommand { get; }
		public IRelayCommand<ServiceNote> MarkAsFinishedCommand { get; }
		public IRelayCommand NavigateToTrackingCommand { get; }
		public IRelayCommand CheckCarVignetteCommand { get; }
		public IRelayCommand OpenCarInformationCommand { get; }
		public IRelayCommand EditCarCommand { get; }
		public IRelayCommand AcceptEditCommand { get; }
		public IRelayCommand CancelEditCommand { get; }
		public IRelayCommand PickImageCommand { get; }


		public CarDetailsViewModel(Car car,
			INavigationService navigationServiceContext,
			IBarcodeService barcodeServiceContext,
			ICarService carServiceContext,
			INoteService noteServiceContext,
			ITrackingService trackingServiceContext)
		{
			_navigationService = navigationServiceContext;
			_barcodeService = barcodeServiceContext;
			_carService = carServiceContext;
			_noteService = noteServiceContext;
			carImage = car.ImagePath!;
			make = car.Make!;
			model = car.Model!;
			licensePlate = car.LicensePlate!;
			description = car.Description;
			countryCode = car.CountryCodeForLicensePlate;
			totalMoneySpent = car.TotalMoneySpent;

			ScanReceiptCommand = new AsyncRelayCommand(ScanReceiptNavigation);

			DeleteBarcodeCommand = new AsyncRelayCommand<Barcode>(DeleteBarcodeAsync);

			AddServiceNoteCommand = new AsyncRelayCommand(SaveServiceNoteAsync);

			DeleteServiceNoteCommand = new AsyncRelayCommand<ServiceNote>(DeleteServiceNoteAsync);

			MarkAsFinishedCommand = new AsyncRelayCommand<ServiceNote>(ChangeServiceNoteToFinished);

			NavigateToTrackingCommand = new AsyncRelayCommand(NavigateToTrackingPage);

			CheckCarVignetteCommand = new AsyncRelayCommand(CheckCarVignetteAsync);

			EditCarCommand = new RelayCommand(EditCar);
			AcceptEditCommand = new AsyncRelayCommand(AcceptEdit);
			CancelEditCommand = new RelayCommand(CancelEdit);
			PickImageCommand = new AsyncRelayCommand(PickImageAsync);

			CountryCodes = DataConstants.CountryCodesConstant;

			SelectedCountryCode = car.CountryCodeForLicensePlate;

			IsEditing = false;

			IsInfoOpen = false;

			OpenCarInformationCommand = new RelayCommand(OpenCarInformation);

			_carId = car.CarId;

			HideCheckOffButton = true;
			_trackingService = trackingServiceContext;
		}

		private async Task PickImageAsync()
		{
			try
			{
				var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
				{
					Title = "Pick an image for the car"
				});

				if (result != null)
				{
					_imageFilePath = result.FullPath;
					carImage = _imageFilePath;
				}
			}
			catch (Exception ex)
			{
				await Application.Current.MainPage.DisplayAlert("Error", $"Failed to pick image: {ex.Message}", "OK");
			}
		}

		private void EditCar()
		{
			IsEditing = true;
		}

		private async Task AcceptEdit()
		{
			IsEditing = false;

			Car? carToEdit = await _carService.GetCarByIdAsync(_carId);
			if (carToEdit == null)
			{
				await Application.Current.MainPage.DisplayAlert("Error", "Car not found.", "OK");
				return;
			}

			if (string.IsNullOrWhiteSpace(make) ||
				string.IsNullOrWhiteSpace(model) ||
				string.IsNullOrWhiteSpace(licensePlate))
			{
				Make = carToEdit.Make;
				Model = carToEdit.Model;
				LicensePlate = carToEdit.LicensePlate;
				await Application.Current.MainPage.DisplayAlert("Error", "Please fill in all fields and select an image.", "OK");
				return;
			}

			string trimmedMake = make.Trim();
			string trimmedModel = model.Trim();
			string trimmedLicensePlate = licensePlate.Trim();

			bool hasChanges = false;
			StringBuilder changes = new StringBuilder("Old -> New\n");

			if (trimmedMake != carToEdit.Make)
			{
				changes.AppendLine($"- {carToEdit.Make} -> {trimmedMake}");
				carToEdit.Make = trimmedMake;
				hasChanges = true;
			}

			if (trimmedModel != carToEdit.Model)
			{
				changes.AppendLine($"- {carToEdit.Model} -> {trimmedModel}");
				carToEdit.Model = trimmedModel;
				hasChanges = true;
			}

			if (trimmedLicensePlate != carToEdit.LicensePlate)
			{
				changes.AppendLine($"- {carToEdit.LicensePlate} -> {trimmedLicensePlate}");
				carToEdit.LicensePlate = trimmedLicensePlate;
				hasChanges = true;
			}

			if (!string.IsNullOrWhiteSpace(selectedCountryCode) && selectedCountryCode != carToEdit.CountryCodeForLicensePlate)
			{
				changes.AppendLine($"- {carToEdit.CountryCodeForLicensePlate} -> {selectedCountryCode}");
				carToEdit.CountryCodeForLicensePlate = selectedCountryCode;
				CountryCode = selectedCountryCode;
				hasChanges = true;
			}

			bool updatePage = false;
			if (!string.IsNullOrWhiteSpace(_imageFilePath) && _imageFilePath != carToEdit.ImagePath)
			{
				changes.AppendLine($"- Image updated!");
				carToEdit.ImagePath = _imageFilePath;
				hasChanges = true;

				updatePage = true;
			}

			if (!hasChanges)
			{
				await Application.Current.MainPage.DisplayAlert("No Changes", "No changes were made to the car.", "OK");
				return;
			}

			bool confirmation = await Application.Current.MainPage.DisplayAlert(
				"Confirm Changes",
				$"You are about to update the car's details. Would you like to proceed with these changes?\n{changes}",
				"Yes",
				"No");

			if (!confirmation)
			{
				return;
			}

			if (updatePage)
			{
				var newPage = new CarDetailsPage(this);
				await _navigationService.RefreshCurrentPage(newPage);
			}

			await _carService.UpdateCarAsync(carToEdit);

			await Application.Current.MainPage.DisplayAlert("Car edited successfully", $"Changes:\n{changes}", "OK");
		}

		private void CancelEdit()
		{
			IsEditing = false;
		}

		private void OpenCarInformation()
		{
			IsInfoOpen = true;
		}

		public async Task CheckCarInsuranceAsync(string extractedText)
		{
			IsInfoOpen = false;

			await Application.Current.MainPage
					 .DisplayAlert("Vehicle Insurance Summary", extractedText, "OK");
		}

		public async Task CheckCarVignetteAsync()
		{
			IsInfoOpen = false;

			var message = await _carService.CheckVignette(licensePlate);

			await Application.Current.MainPage
					 .DisplayAlert("Vehicle Vignette Summary", message, "OK");
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

		private async Task NavigateToTrackingPage()
		{
			var trackingViewModel = new TrackingViewModel(_trackingService);
			var trackingPage = new TrackingPage(trackingViewModel);
			await _navigationService.NavigateToPage(trackingPage);
		}
	}
}
