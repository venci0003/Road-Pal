using Newtonsoft.Json;
using RoadPal.Contracts;
using RoadPal.Infrastructure;
using RoadPal.Infrastructure.Models;
using SQLite;

namespace RoadPal.Services
{
	public class CarService : ICarService
	{
		private readonly RoadPalDatabase _roadPalDatabase;

		private readonly string BaseUrl = "https://vpic.nhtsa.dot.gov/api/vehicles/";
		private readonly string localMakesFilePath;
		private readonly TimeSpan cacheDuration = TimeSpan.FromHours(24);

		public CarService(RoadPalDatabase context)
		{
			_roadPalDatabase = context;

			string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
			localMakesFilePath = Path.Combine(baseDirectory, "Resources", "Json", "localMakes.json");

			EnsureDirectoryExists(localMakesFilePath);
		}

		public void EnsureDirectoryExists(string filePath)
		{
			string? directory = Path.GetDirectoryName(filePath);
			if (directory != null && !Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}
		}

		public bool IsCacheValid(string filePath)
		{
			if (!File.Exists(filePath))
			{
				return false;
			}

			FileInfo fileInfo = new FileInfo(filePath);
			DateTime lastUpdated = fileInfo.LastWriteTime;
			return DateTime.Now - lastUpdated < cacheDuration;
		}

		public async Task CacheCarMakesAsync()
		{
			if (!IsCacheValid(localMakesFilePath))
			{
				string url = $"{BaseUrl}getallmakes?format=json";
				using (HttpClient httpClient = new HttpClient())
				{
					try
					{
						string response = await httpClient.GetStringAsync(url);
						File.WriteAllText(localMakesFilePath, response);
					}
					catch (Exception ex)
					{
						Console.WriteLine($"Failed to download makes: {ex.Message}");
					}
				}
			}
		}

		public async Task<bool> CacheCarModelsForMakeAsync(string make)
		{
			string cacheFilePath = GetCacheFilePathForMake(make);

			string? directoryPath = Path.GetDirectoryName(cacheFilePath);
			if (directoryPath != null && !Directory.Exists(directoryPath))
			{
				Directory.CreateDirectory(directoryPath);
			}

			if (!IsCacheValid(cacheFilePath))
			{
				string url = $"{BaseUrl}GetModelsForMake/{make}?format=json";
				using (HttpClient httpClient = new HttpClient())
				{
					try
					{
						string response = await httpClient.GetStringAsync(url);
						File.WriteAllText(cacheFilePath, response);

						return true;
					}
					catch (Exception ex)
					{
						Console.WriteLine($"Failed to download models for {make}: {ex.Message}");

						return false;
					}
				}
			}

			return true;
		}

		public string GetCacheFilePathForMake(string make)
		{
			string sanitizedMake = make.Replace(" ", "_").Replace("/", "_");
			return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Json", $"models_{sanitizedMake}.json");
		}


		public async Task<List<string>> GetCarMakesAsync()
		{
			await CacheCarMakesAsync();

			string json = File.ReadAllText(localMakesFilePath);
			dynamic? jsonResponse = JsonConvert.DeserializeObject<dynamic>(json);

			if (jsonResponse?.Results == null)
			{
				return new List<string>();
			}

			List<string> makes = ((IEnumerable<dynamic>)jsonResponse.Results)
				.Select(r => (string)r.Make_Name)
				.Take(100)
				.ToList();

			return makes;
		}

		public async Task<List<string>> GetCarModelsForMakeAsync(string make)
		{
			await CacheCarModelsForMakeAsync(make);

			bool makeDirectoryExists = await CacheCarModelsForMakeAsync(make);

			if (makeDirectoryExists)
			{
				string path = GetCacheFilePathForMake(make);

				string json = File.ReadAllText(path);
				dynamic? jsonResponse = JsonConvert.DeserializeObject<dynamic>(json);

				if (jsonResponse?.Results == null)
				{
					return new List<string>();
				}

				List<string> models = ((IEnumerable<dynamic>)jsonResponse.Results)
					.Where(r => ((string)r.Make_Name).Contains(make, StringComparison.OrdinalIgnoreCase))
					.Select(r => (string)r.Model_Name)
					.ToList();

				return models;

			}
			else
			{
				return new List<string>();
			}
		}

		public async Task<List<string>> SearchCarMakesAsync(string searchTerm)
		{
			await CacheCarMakesAsync();

			string json = File.ReadAllText(localMakesFilePath);
			dynamic? jsonResponse = JsonConvert.DeserializeObject<dynamic>(json);

			if (jsonResponse?.Results == null)
			{
				return new List<string>();
			}

			List<string> makes = ((IEnumerable<dynamic>)jsonResponse.Results)
							.Where(r => ((string)r.Make_Name).Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
							.Select(r => (string)r.Make_Name)
							.Take(100)
							.ToList();

			return makes;
		}

		public async Task AddCarAsync(Car car)
		{
			SQLiteAsyncConnection connection = await _roadPalDatabase.GetConnectionAsync();
			await connection.InsertAsync(car);
		}

		public async Task<IEnumerable<Car>> GetAllCarsAsync()
		{
			SQLiteAsyncConnection connection = await _roadPalDatabase.GetConnectionAsync();
			return await connection.Table<Car>().ToListAsync();
		}

		public async Task DeleteAllCarsTestAsync()
		{
			await _roadPalDatabase.DeleteAllCarsAsync();
		}

		public async Task DeleteCarByIdAsync(int id)
		{
			SQLiteAsyncConnection connection = await _roadPalDatabase.GetConnectionAsync();
			await connection.Table<Car>().Where(c => c.CarId == id).DeleteAsync();
		}

		public async Task<Car> GetCarByIdAsync(int carId)
		{
			SQLiteAsyncConnection connection = await _roadPalDatabase.GetConnectionAsync();
			return await connection.Table<Car>().Where(c => c.CarId == carId).FirstOrDefaultAsync();
		}

		public async Task AddMoneyToTotalAsync(int carId, decimal money)
		{
			SQLiteAsyncConnection connection = await _roadPalDatabase.GetConnectionAsync();
			var car = await connection.Table<Car>().FirstOrDefaultAsync(c => c.CarId == carId);

			if (car != null)
			{
				car.TotalMoneySpent += money;

				Console.WriteLine(car.TotalMoneySpent);
				await connection.UpdateAsync(car);
			}
		}
	}
}

