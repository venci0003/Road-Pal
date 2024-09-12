using SQLite;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadPal.Infrastructure.Models
{
	public class Barcode
	{
		[PrimaryKey]
		[AutoIncrement]
		public int BarcodeId { get; set; }

		[Required]
		public string SerialNumber { get; set; } = string.Empty;

		[Required]
		public string VATId { get; set; } = string.Empty;

		public DateTime DateOfIssue { get; set; }

		public DateTime TimeOfIssue { get; set; }

		public decimal TotalAmount { get; set; }

		public int CarId { get; set; }
	}
}
