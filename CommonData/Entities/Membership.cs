using System.ComponentModel.DataAnnotations;

namespace CommonData.Entities
{
    public class Membership
    {
        public int MemberId { get; set; }
        public int UserId { get; set; }

        [Required]
        public virtual Member Member { get; set; }
        [Required]
        public virtual User User { get; set; }
    }
}
