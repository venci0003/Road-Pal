using RoadPal.Infrastructure;
using RoadPal.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadPal.Services
{
	public class BarcodeService
	{
		private readonly RoadPalDatabase _roadPalDatabase;

		public BarcodeService(RoadPalDatabase context)
		{
			_roadPalDatabase = context;
		}

		public async Task SaveReceiptAsync(Barcode barcode)
		{
			await _roadPalDatabase.GetConnection().InsertAsync(barcode);
		}
	}
}
