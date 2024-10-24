using System.Collections.ObjectModel;

namespace RoadPal.Common
{
	public class ApplicationConstants
	{
		public static class DatabaseConstants
		{
			public const string DatabaseFilename = "RoadPalSQLite.db3";

			public const SQLite.SQLiteOpenFlags Flags =
				SQLite.SQLiteOpenFlags.ReadWrite |

				SQLite.SQLiteOpenFlags.Create |

				SQLite.SQLiteOpenFlags.SharedCache;

			public static string DatabasePath =>
				Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
		}

		public static class MessagesConstants
		{
			public const string NoCarsMessage = "No cars yet!\n Add a new one to get started.";

			public static readonly string BarcodeInformationMessage = "Receipt Information:\n" +
					 "Serial Number: {0}\n" +
					 "VAT ID: {1}\n" +
					 "Date of Issue: {2:yyyy-MM-dd}\n" +
					 "Time of Issue: {3:HH:mm:ss}\n" +
					 "Total Amount: {4} lv";

			public const string SaveReceiptMessage = "Would you like to save this receipt?";

			public const string SuccesfullyScannedBarcodeMessage = "Barcode scanned successfully!";

			public const string UnsuccesfullyScannedBarcodeMessage = "Oops something happened...\nPlease try again later.";

			public const string ReceiptSavedSuccesfullyMessage = "Receipt saved succesfully";

			public const string ReceiptSavedWhereToFindMessage = "You can find you receipts in car details!";

			public const string ConfirmDeleteTitleMessage = "Confirm Deletion";

			public const string DeleteReceiptMessage = "Are you sure you want to delete this receipt?";

			public const string DeleteServiceNoteMessage = "Are you sure you want to delete this note?";

			public const string SaveServiceNoteErrorTitleMessage = "An error occured!";

			public const string SaveServiceNoteErrorDescriptionMessage = "Please fill out the title and description of your service note.";

			public const string NoValidVignetteMessage = $"No valid vignette found!\n\nPlease ensure the vignette is paid and the license plate number is correct.";

			public const string ValidVignetteInformationMessage = "VIGNETTE DETAILS\nThe vignette for the vehicle with license plate {0} (BG) is valid:\n" +
							"- From: {1}\n" +
							"- To: {2}\n" +
							"- Paid: {3}lv\n" +
							"- Days left: {4}";
		}

		public static class DataConstants
		{
			public static readonly ObservableCollection<string> CountryCodesConstant = new ObservableCollection<string> { "BG", "EU" };
		}
	}
}
