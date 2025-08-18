using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace TripBookingBE.Models;

[Table("Route")]
public partial class Route
{
    // [Timestamp]
    // public byte[] RowVersion { get; set; }

    [Key]
    [Column("id")]
    public long Id { get; set; }

    [DataType("nvarchar(2000)")]
    [Column("routeDescription")]
    public string? RouteDescription { get; set; }

    [Column("dateCreated", TypeName = "datetime")]
    public DateTime? DateCreated { get; set; } = DateTime.Now;

    [Column("dateModified", TypeName = "datetime")]
    public DateTime? DateModified { get; set; } = DateTime.Now;

    [JsonIgnore]
    [InverseProperty("Route")]
    public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();
}
