using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;


namespace CommonData.Entities
{
	[Flags]
	public enum SpecialNeed
	{
		None = 0,
		GlutenFree = 1,
		LactoseFree = 2,
		Vegetarian = 4,
		Vegan = 8,
		Paleo = 16
	}

	public class User : IdentityUser<int>
	{
		[Required, MaxLength(30)]
		public string Name { get; set; }
		[MaxLength(50)]
		public string? Address { get; set; }
		[Required]
		public SpecialNeed SpecialNeed { get; set; }
		public DateTime DateOfBirth { get; set; }

		public int Age()
		{
			return (int) DateTime.Today.Subtract(DateOfBirth).TotalDays / 365;
		}
    }
}
