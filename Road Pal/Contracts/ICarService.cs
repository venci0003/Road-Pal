using RoadPal.Infrastructure.Models;

namespace RoadPal.Contracts
{
	public interface ICarService
	{
		void EnsureDirectoryExists(string filePath);

		bool IsCacheValid(string filePath);

		Task CacheCarMakesAsync();

		Task<bool> CacheCarModelsForMakeAsync(string make);

		string GetCacheFilePathForMake(string make);

		Task<List<string>> GetCarMakesAsync();

		Task<List<string>> GetCarModelsForMakeAsync(string make);

		Task<List<string>> SearchCarMakesAsync(string searchTerm);

		Task AddCarAsync(Car car);

		Task<IEnumerable<Car>> GetAllCarsAsync();

		Task DeleteAllCarsTestAsync();

		Task DeleteCarByIdAsync(int id);

		Task AddMoneyToTotalAsync(int carId, decimal money);
	}
}
