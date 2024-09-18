using RoadPal.Infrastructure;
using RoadPal.Infrastructure.Models;
using SQLite;

namespace RoadPal.Services
{
	public class NoteService
	{
		private readonly RoadPalDatabase _roadPalDatabase;

		public NoteService(RoadPalDatabase context)
		{
			_roadPalDatabase = context;
		}

		public async Task AddServiceNote(ServiceNote serviceNote)
		{
			SQLiteAsyncConnection connection = await _roadPalDatabase.GetConnectionAsync();
			await connection.InsertAsync(serviceNote);
		}

	}
}
