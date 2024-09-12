using RoadPal.Infrastructure;
using RoadPal.Infrastructure.Models;
using SQLite;

namespace RoadPal.Services
{
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

		public async Task<IEnumerable<Barcode>> GetBarcodesAsync(int carId)
		{
			SQLiteAsyncConnection connection = await _roadPalDatabase.GetConnectionAsync();
			return await connection.Table<Barcode>().Where(c => c.CarId == carId).ToListAsync();
		}
	}
}
