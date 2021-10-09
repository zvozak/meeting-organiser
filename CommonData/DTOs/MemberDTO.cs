using CommonData.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CommonData.DTOs
{
	public class MemberDTO
	{
		[Key]
		public int Id { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		[MaxLength(60)]
		public string? Department { get; set; }
		public DateTime? DateOfJoining { get; set; }

		public ICollection<ProjectDTO> Projects { get; set; }
        public JobDTO Job { get; set; }
        public MemberDTO Boss { get; set; }

		public int IdAtOrganisation { get; set; }
		public int OrganisationId { get; set; }
		public int? BossId { get; set; }
		public int? JobId { get; set; }

		public static explicit operator MemberDTO(Member member) => new MemberDTO
		{
			Id = member.Id,
			Name = member.Name,
			Email = member.Email,
			Department = member.Department,
			DateOfJoining = member.DateOfJoining,
			IdAtOrganisation = member.IdAtOrganisation,
			OrganisationId = member.OrganisationId,
			BossId = member.BossId,
			JobId = member.JobId
		};
		public static explicit operator Member(MemberDTO dto) => new Member
		{
			Name = dto.Name,
			Email = dto.Email,
			Department = dto.Department,
			DateOfJoining = dto.DateOfJoining,
			IdAtOrganisation = dto.IdAtOrganisation,
			OrganisationId = dto.OrganisationId,
			BossId = dto.BossId,
			JobId = dto.JobId
		};
	}
}
