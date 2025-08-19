using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TripBookingBE.Models
{
    public class GeneralParam
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("paramKey")]
        [StringLength(100)]
        public string ParamKey { get; set; }

        [StringLength(100)]
        [Column("paramCode")]
        public string? ParamCode { get; set; }

        [StringLength(2000)]
        [Column("paramDescription")]
        public string? ParamDescription { get; set; }

        [Column("dateCreated")]
        public DateTime? DateCreated { get; set; }=null;
        [Column("dateModified")]
        public DateTime? DateModified { get; set; }=null;

        [JsonIgnore]
        [InverseProperty("GeneralParam")]
        public virtual IEnumerable<Ticket>? Tickets { get; set; }
    }
}