using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TripBookingBE.Models;

[Table("Ticket")]
public partial class Ticket
{
    [Key]
    [Column("customerBookTripId")]
    public long CustomerBookTripId { get; set; }

    [Column("price", TypeName = "money")]
    public decimal? Price { get; set; }

    [Column("sellerCode")]
    [StringLength(20)]
    [Unicode(false)]
    public string? SellerCode { get; set; }

    [Column("dateCreated", TypeName = "datetime")]
    public DateTime? DateCreated { get; set; }

    [Column("dateModified", TypeName = "datetime")]
    public DateTime? DateModified { get; set; }

    [ForeignKey("CustomerBookTripId")]
    [InverseProperty("Ticket")]
    public virtual CustomerBookTrip CustomerBookTrip { get; set; } = null!;
}
