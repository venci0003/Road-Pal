using RoadPal.Contracts;
using RoadPal.Infrastructure;
using RoadPal.Infrastructure.Models;
using SQLite;

namespace RoadPal.Services
{
	public class NoteService : INoteService
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

		public async Task DeleteServiceNoteByIdAsync(int serviceNoteId)
		{
			SQLiteAsyncConnection connection = await _roadPalDatabase.GetConnectionAsync();
			await connection.Table<ServiceNote>().Where(s => s.ServiceNoteId == serviceNoteId).DeleteAsync();
		}

		public async Task ChangeServiceNoteToFinishedAsync(ServiceNote serviceNote)
		{
			SQLiteAsyncConnection connection = await _roadPalDatabase.GetConnectionAsync();
			await connection.UpdateAsync(serviceNote);
		}

		public async Task<IEnumerable<ServiceNote>> GetAllServiceNotesAsync(int carId, bool isTaskFinished)
		{
			SQLiteAsyncConnection connection = await _roadPalDatabase.GetConnectionAsync();
			return await connection.Table<ServiceNote>().Where(n => n.CarId == carId && n.isFinished == isTaskFinished).ToListAsync();
		}

	}
}
