using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TripBookingBE.Models;

[Table("CustomerReviewTrip")]
public partial class CustomerReviewTrip
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("customerId")]
    public long CustomerId { get; set; }

    [Column("tripId")]
    public long? TripId { get; set; }

    [Column("content")]
    [StringLength(1000)]
    public string? Content { get; set; }

    [Column("dateCreated", TypeName = "datetime")]
    public DateTime? DateCreated { get; set; }

    [Column("dateModified", TypeName = "datetime")]
    public DateTime? DateModified { get; set; }

    [ForeignKey("CustomerId")]
    [InverseProperty("CustomerReviewTrips")]
    public virtual User Customer { get; set; } = null!;

    [ForeignKey("TripId")]
    [InverseProperty("CustomerReviewTrips")]
    public virtual Trip? Trip { get; set; }
}
