using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace TripBookingBE.Models;

[Table("Ticket")]
public partial class Ticket
{
    // [Timestamp]
    // public byte[]? RowVersion { get; set; } = null;

    [Key]
    [Column("customerBookTripId")]
    public long CustomerBookTripId { get; set; }

    [NotMapped]
    public long CustomerId { get; set; } = 0;

    [NotMapped]
    public long TripId { get; set; } = 0;

    [Column("generalParamId")]
    public long? GeneralParamId { get; set; }

    [Column("price", TypeName = "decimal")]
    public decimal? Price { get; set; }

    [Column("sellerCode")]
    [StringLength(20)]
    [Unicode(false)]
    public string? SellerCode { get; set; }

    [Column("dateCreated", TypeName = "datetime")]
    public DateTime? DateCreated { get; set; } = DateTime.Now;

    [Column("dateModified", TypeName = "datetime")]
    public DateTime? DateModified { get; set; } = DateTime.Now;

    [ForeignKey("CustomerBookTripId")]
    [InverseProperty("Ticket")]
    public virtual CustomerBookTrip? CustomerBookTrip { get; set; }

    [ForeignKey("GeneralParamId")]
    [InverseProperty("Tickets")]
    public virtual GeneralParam? GeneralParam { get; set; }
}
