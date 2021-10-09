using CommonData.Entities;
using System.ComponentModel.DataAnnotations;

namespace CommonData.DTOs
{
    public class MembershipDTO
    {
        [Required]
        public int MemberId { get; set; }
        [Required]
        public int UserId { get; set; }
        public static explicit operator MembershipDTO(Membership m) => new MembershipDTO
        {
            MemberId = m.MemberId,
            UserId = m.UserId
        };
        public static explicit operator Membership(MembershipDTO dto) => new Membership
        {
            MemberId = dto.MemberId,
            UserId = dto.UserId
        };
    }
}
