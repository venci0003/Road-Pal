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

		Task UpdateCarAsync(Car car);

		Task<IEnumerable<Car>> GetAllCarsAsync(string querySearch, bool isFavourite);

		Task DeleteAllCarsTestAsync();

		Task DeleteCarByIdAsync(int carId);

		Task<Car> GetCarByIdAsync(int carId);

		Task AddMoneyToTotalAsync(int carId, decimal money);

		Task<string> CheckVignette(string licensePlate);

		Task ChangeFavouritismAsync(int carId, bool status);
	}
}
