using RoadPal.Infrastructure;
using RoadPal.Infrastructure.Models;

namespace RoadPal.Services
{
	public class CarService
	{
		private readonly RoadPalDatabase _roadPalDatabase;

		public CarService(RoadPalDatabase context)
		{
			_roadPalDatabase = context;
		}

		public async Task AddCarAsync(Car car)
		{
			await _roadPalDatabase.GetConnection().InsertAsync(car);
		}

		public async Task<IEnumerable<Car>> GetAllCarsAsync()
		{
			return await _roadPalDatabase.GetConnection().Table<Car>().ToListAsync();
		}

		public async Task DeleteAllCarsTestAsync()
		{
			await _roadPalDatabase.DeleteAllCarsAsync();
		}

		public async Task DeleteCarByIdAsync(int id)
		{
			await _roadPalDatabase.GetConnection().Table<Car>().Where(c => c.CarId == id).DeleteAsync();
		}


		//public Task<List<Car>> GetAllCarsAsync()
		//{
		//	return _database.Table<Car>().ToListAsync();
		//}

		//public Task<int> AddCarAsync(Car car)
		//{
		//	return _database.InsertAsync(car);
		//}

		//public Task<Car> GetCarByIdAsync(int id)
		//{
		//	return _database.Table<Car>().Where(c => c.CarId == id).FirstOrDefaultAsync();
		//}

		//public Task<Car> GetCarByMakeAsync(string make)
		//{
		//	return _database.Table<Car>().Where(c => c.Make == make).FirstOrDefaultAsync();
		//}

		//public Task<int> UpdateCarAsync(Car car)
		//{
		//	return _database.UpdateAsync(car);
		//}

		//public Task<int> DeleteCarAsync(Car car)
		//{
		//	return _database.DeleteAsync(car);
		//}

		//public Task DeleteAllCarsAsync()
		//{
		//	return _database.ExecuteAsync("DELETE FROM Car");
		//}
	}
}
