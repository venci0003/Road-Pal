using RoadPal.Infrastructure;
using RoadPal.Infrastructure.Models;

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
			await _roadPalDatabase.GetConnection().InsertAsync(barcode);
		}

		public async Task<IEnumerable<Barcode>> GetBarcodesAsync()
		{
			return await _roadPalDatabase.GetConnection().Table<Barcode>().ToListAsync();
		}
	}
}
