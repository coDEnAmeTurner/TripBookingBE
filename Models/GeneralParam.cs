using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TripBookingBE.Models
{
    public class GeneralParam
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("paramKey")]
        public string ParamKey { get; set; }

        [Column("paramCode")]
        public string? ParamCode { get; set; }

        [Column("paramDescription")]
        public string? ParamDescription { get; set; }

        [Column("dateCreated")]
        public DateTime? DateCreated { get; set; }=null;
        [Column("dateModified")]
        public DateTime? DateModified { get; set; }=null;

        [InverseProperty("GeneralParam")]
        public virtual IEnumerable<Ticket>? Tickets { get; set; }
    }
}