using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CommonData.Entities;
using Microsoft.AspNetCore.Identity;


namespace CommonData.DTOs
{
	public class UserDTO : IdentityUser<int>
	{
		[Required, MaxLength(30)]
		public string Name { get; set; }
		[MaxLength(50)]
		public string? Address { get; set; }
		[Required]
		public SpecialNeed SpecialNeed { get; set; }
		public DateTime DateOfBirth { get; set; }

		public static explicit operator UserDTO(User user) => new UserDTO
		{
			Name = user.Name,
			Address = user.Address,
			SpecialNeed = user.SpecialNeed,
			DateOfBirth = user.DateOfBirth
        };
		public static explicit operator User(UserDTO dto) => new User
		{
			Name = dto.Name,
			Address = dto.Address,
			SpecialNeed = dto.SpecialNeed,
			DateOfBirth = dto.DateOfBirth
		};
	}
}
