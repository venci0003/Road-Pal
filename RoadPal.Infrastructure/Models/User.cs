using SQLite;

namespace RoadPal.Infrastructure.Models
{
	public class User
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }

		public string Username { get; set; }

		public string Email { get; set; }
	}
}
