using RoadPal.Infrastructure.Models;
using SQLite;
using static RoadPal.Common.ApplicationConstants;

namespace RoadPal.Infrastructure
{
	public class RoadPalDatabase
	{
		private readonly SQLiteAsyncConnection _database;
		private static bool _isInitialized = false;


		public RoadPalDatabase()
		{
			_database = new SQLiteAsyncConnection(DatabaseConstants.DatabasePath, DatabaseConstants.Flags);
		}

		public async Task InitializeAsync()
		{
			try
			{
				if (!_isInitialized)
				{
					if (!_database.TableMappings.Any(m => m.MappedType.Name == typeof(Car).Name))
					{
						await _database.CreateTableAsync<Car>();
					}
					if (!_database.TableMappings.Any(m => m.MappedType.Name == typeof(Barcode).Name))
					{
						await _database.CreateTableAsync<Barcode>();
					}
					if (!_database.TableMappings.Any(m => m.MappedType.Name == typeof(ServiceNote).Name))
					{
						await _database.CreateTableAsync<ServiceNote>();
					}
					_isInitialized = true;
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Database initialization error: {ex.Message}");
				throw;
			}
		}

		public Task<int> DeleteAllCarsAsync()
		{
			return _database.ExecuteAsync("DELETE FROM Car");
		}

		public Task<int> DeleteAllBarcodesAsync()
		{
			return _database.ExecuteAsync("DELETE FROM Barcode");
		}

		public async Task<SQLiteAsyncConnection> GetConnectionAsync()
		{
			if (!_isInitialized)
			{
				await InitializeAsync();
			}
			return _database;
		}
	}
}
