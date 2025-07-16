using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TripBookingBE.Models;

[PrimaryKey("SellerId", "TripId")]
[Table("SellerTrip")]
public partial class SellerTrip
{
    [Key]
    [Column("sellerId")]
    public long SellerId { get; set; }

    [Key]
    [Column("tripId")]
    public long TripId { get; set; }

    [Column("dateCreated", TypeName = "datetime")]
    public DateTime? DateCreated { get; set; }

    [Column("dateModified", TypeName = "datetime")]
    public DateTime? DateModified { get; set; }

    [ForeignKey("TripId")]
    [InverseProperty("SellerTrips")]
    public virtual Trip Trip { get; set; } = null!;
}
