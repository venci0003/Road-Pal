using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
	}
}
