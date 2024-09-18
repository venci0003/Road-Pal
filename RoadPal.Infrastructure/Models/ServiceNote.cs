using SQLite;

namespace RoadPal.Infrastructure.Models
{
	public class ServiceNote
	{
		[AutoIncrement]
		[PrimaryKey]
		public int ServiceNoteId { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public DateTime CreatedDate { get; set; }

		public int CarId { get; set; }
	}
}
