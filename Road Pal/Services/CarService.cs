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
	}
}
