using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CommonData.Entities
{
	public class Member
    {
		[Key]
        public int Id { get; set; }
        public string Name { get; set; }
		public string Email { get; set; }
		[MaxLength(60)]
		public string? Department { get; set; }
		public DateTime? DateOfJoining { get; set; }

        public int IdAtOrganisation { get; set; }

		public virtual Organisation Organisation { get; set; }
		[Required]
		public int OrganisationId { get; set; }
		public virtual Member Boss { get; set; }
		public int? BossId { get; set; }        
		public int? JobId { get; set; }
		public virtual Job Job { get; set; }

		public int? TimeSpentAtOrganisation()
		{
			if(DateOfJoining.HasValue)
            {
				return (int)DateTime.Today.Subtract((DateTime)DateOfJoining).TotalDays / 365;
			}
			return null;
			//throw new Exception(string.Format("Cannot calculate time spent at organisation for member {0} of organisation {1}. No joiningdate has been given.", MemberId, OrganisationId));
		}

	}
}
