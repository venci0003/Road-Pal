using Newtonsoft.Json;
using RoadPal.Infrastructure;
using RoadPal.Infrastructure.Models;

namespace RoadPal.Services
{
	public class CarService
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

		private void EnsureDirectoryExists(string filePath)
		{
			string? directory = Path.GetDirectoryName(filePath);
			if (directory != null && !Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}
		}

		private bool IsCacheValid(string filePath)
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

		public async Task CacheCarModelsForMakeAsync(string make)
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
					}
					catch (Exception ex)
					{
						Console.WriteLine($"Failed to download models for {make}: {ex.Message}");
					}
				}
			}
		}

		private string GetCacheFilePathForMake(string make)
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
				//.Take(100)
				.ToList();

			return makes;
		}

		public async Task<List<string>> GetCarModelsForMakeAsync(string make)
		{
			await CacheCarModelsForMakeAsync(make);
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

