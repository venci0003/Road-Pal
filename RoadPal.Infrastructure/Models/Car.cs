﻿using SQLite;
using System.ComponentModel.DataAnnotations;

namespace RoadPal.Infrastructure.Models
{
	public class Car
	{
		[PrimaryKey]
		[AutoIncrement]
		public int CarId { get; set; }

		[Required]
		public string Description { get; set; } = string.Empty;
		[Required]
		public string Make { get; set; } = string.Empty;
		[Required]
		public string Model { get; set; } = string.Empty;
		[Required]
		public string LicensePlate { get; set; } = string.Empty;

		public string ImagePath { get; set; } = string.Empty;

	}
}