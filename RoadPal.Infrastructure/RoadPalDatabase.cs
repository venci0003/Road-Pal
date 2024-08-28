﻿using RoadPal.Infrastructure.Models;
using SQLite;
using static RoadPal.Common.ApplicationConstants;

namespace RoadPal.Infrastructure
{
	public class RoadPalDatabase
	{
		private readonly SQLiteAsyncConnection _database;

		public RoadPalDatabase()
		{
			_database = new SQLiteAsyncConnection(DatabaseConstants.DatabasePath, DatabaseConstants.Flags);
		}

		public async Task InitializeAsync()
		{
			try
			{
				if (!_database.TableMappings.Any(m => m.MappedType.Name == typeof(Car).Name))
				{
					await _database.CreateTableAsync<Car>();
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

		public SQLiteAsyncConnection GetConnection()
		{
			InitializeAsync();
			return _database;
		}
	}
}