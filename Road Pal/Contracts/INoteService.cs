
using RoadPal.Infrastructure.Models;

namespace RoadPal.Contracts
{
	public interface INoteService
	{
		Task AddServiceNote(ServiceNote serviceNote);

		Task<IEnumerable<ServiceNote>> GetAllServiceNotesAsync(int carId);
	}
}
