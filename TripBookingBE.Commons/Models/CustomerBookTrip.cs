using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace TripBookingBE.Models;

[Table("CustomerBookTrip")]
[Index("CustomerId", "TripId", Name = "IX_CustomerBookTrip", IsUnique = true)]
public partial class CustomerBookTrip
{

    // [Timestamp]
    // public byte[]? RowVersion { get; set; }

    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("customerId")]
    public long? CustomerId { get; set; }

    [Column("tripId")]
    public long? TripId { get; set; }

    [Display(Name ="Place Number")]
    [Column("placeNumber")]
    public int? PlaceNumber { get; set; }

    [Column("dateCreated", TypeName = "datetime")]
    public DateTime? DateCreated { get; set; } = DateTime.Now;

    [Column("dateModified", TypeName = "datetime")]
    public DateTime? DateModified { get; set; }=  DateTime.Now;

    [ForeignKey("CustomerId")]
    [InverseProperty("CustomerBookTrips")]
    public virtual User? Customer { get; set; }

    [JsonIgnore]
    [InverseProperty("CustomerBookTrip")]
    public virtual Ticket? Ticket { get; set; }

    [ForeignKey("TripId")]
    [InverseProperty("CustomerBookTrips")]
    public virtual Trip? Trip { get; set; }
}
