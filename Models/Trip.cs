using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TripBookingBE.Models;

[Table("Trip")]
public partial class Trip
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("departureTime", TypeName = "datetime")]
    public DateTime? DepartureTime { get; set; }

    [Column("placeCount")]
    public int? PlaceCount { get; set; }

    [Column("registrationNumber")]
    [StringLength(20)]
    [Unicode(false)]
    public string? RegistrationNumber { get; set; }

    [Column("driverId")]
    public long? DriverId { get; set; }

    [Column("routeId")]
    public long? RouteId { get; set; }

    [Column("dateCreated", TypeName = "datetime")]
    public DateTime? DateCreated { get; set; }

    [Column("dateModified", TypeName = "datetime")]
    public DateTime? DateModified { get; set; }

    [InverseProperty("Trip")]
    public virtual ICollection<CustomerBookTrip> CustomerBookTrips { get; set; } = new List<CustomerBookTrip>();
    public virtual ICollection<User> Users { get; set; } = new List<User>();

    [InverseProperty("Trip")]
    public virtual ICollection<CustomerReviewTrip> CustomerReviewTrips { get; set; } = new List<CustomerReviewTrip>();

    [ForeignKey("DriverId")]
    [InverseProperty("Trips")]
    public virtual User? Driver { get; set; }

    [ForeignKey("RouteId")]
    [InverseProperty("Trips")]
    public virtual Route? Route { get; set; }

    [InverseProperty("Trip")]
    public virtual ICollection<SellerTrip> SellerTrips { get; set; } = new List<SellerTrip>();
}
