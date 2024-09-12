using RoadPal.Infrastructure.Models;

namespace RoadPal.Contracts
{
	public interface IBarcodeService
	{
		Task<IEnumerable<Barcode>> GetBarcodesAsync(int carId);
		Task SaveReceiptAsync(Barcode barcode);
		Task DeleteReceiptByIdAsync(int barcodeId);
	}
}
