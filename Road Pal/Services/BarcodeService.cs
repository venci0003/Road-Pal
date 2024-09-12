using SQLite;
namespace RoadPal.Services
{
	using Infrastructure;
	using Infrastructure.Models;
	public class BarcodeService
	{
		private readonly RoadPalDatabase _roadPalDatabase;

		public BarcodeService(RoadPalDatabase context)
		{
			_roadPalDatabase = context;
		}

		public async Task SaveReceiptAsync(Barcode barcode)
		{
			SQLiteAsyncConnection connection = await _roadPalDatabase.GetConnectionAsync();
			await connection.InsertAsync(barcode);
		}

		public async Task DeleteReceiptByIdAsync(int barcodeId)
		{
			SQLiteAsyncConnection connection = await _roadPalDatabase.GetConnectionAsync();
			await connection.Table<Barcode>().Where(b => b.BarcodeId == barcodeId).DeleteAsync();
		}

		public async Task<IEnumerable<Barcode>> GetBarcodesAsync(int carId)
		{
			SQLiteAsyncConnection connection = await _roadPalDatabase.GetConnectionAsync();
			return await connection.Table<Barcode>().Where(c => c.CarId == carId).ToListAsync();
		}
	}
}
