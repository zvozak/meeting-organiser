using CommonData.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CommonData.DTOs
{
    public class MemberOfProjectDTO
    {
        [Required]
        public int MemberId { get; set; }
        [Required]
        public int ProjectId { get; set; }
        public static explicit operator MemberOfProjectDTO (MemberOfProject m) => new MemberOfProjectDTO
        {
            MemberId = m.MemberId,
            ProjectId = m.ProjectId
        };
        public static explicit operator MemberOfProject (MemberOfProjectDTO dto) => new MemberOfProject
        {
            MemberId = dto.MemberId,
            ProjectId = dto.ProjectId
        };
    }
}
