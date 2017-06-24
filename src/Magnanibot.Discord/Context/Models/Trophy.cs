using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Magnanibot.Context.Models
{
    public class Trophy
    {
        public Trophy() { /* Required by EF */ }

        public Trophy(string awardedBy, string awardedTo, string reason)
        {
            (AwardedBy, AwardedTo, Reason) = (awardedBy, awardedTo, reason);
            AwardedOn = DateTime.Now;
        }

        [Key]
        public int Id { get; set; }
        [Column("awarded_on")]
        public DateTime AwardedOn { get; set; }
        [Column("awarded_by")]
        public string AwardedBy { get; set; }
        [Column("awarded_to")]
        public string AwardedTo { get; set; }
        public string Reason { get; set; }
    }
}
