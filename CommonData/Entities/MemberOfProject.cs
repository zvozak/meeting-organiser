using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CommonData.Entities
{
    public class MemberOfProject
    {
        public int MemberId { get; set; }
        public int ProjectId { get; set; }
        [Required]
        public Member Member { get; set; }
        [Required]
        public Project Project { get; set; }
    }
}
