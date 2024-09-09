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
		public int SerialNumber { get; set; }

		public int VATId { get; set; }

		public DateTime DateOfIssue { get; set; }

		public DateTime TimeOfIssue { get; set; }

        public decimal TotalAmount { get; set; }
    }
}
