using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace TripBookingBE.Models;

[Table("User")]
[Index("UserName", Name = "UQ__User__66DCF95C55A682B1", IsUnique = true)]
public partial class User : IdentityUser<long>
{
    [Timestamp]
    public byte[] RowVersion { get; set; }

    [NotMapped]
    public string? NewPassword { get; set; }
    [NotMapped]
    public string? ConfirmPassword { get; set; }
    [NotMapped]
    public string? Password { get; set; }

    [NotMapped]
    public IFormFile? File { get; set; }

    [Key]
    [Column("id")]
    public override long Id { get; set; }

    [Column("password")]
    [StringLength(500)]
    [Unicode(false)]
    public override string? PasswordHash { get; set; } = null!;

    [Column("lastLogin", TypeName = "datetime")]
    public DateTime? LastLogin { get; set; }

    [Column("userName")]
    [StringLength(150)]
    [Unicode(false)]
    public override string UserName { get; set; } = null!;

    [Column("firstName")]
    [StringLength(150)]
    [Unicode(false)]
    public string? FirstName { get; set; }

    [Column("lastName")]
    [StringLength(150)]
    [Unicode(false)]
    public string LastName { get; set; } = null!;

    [Column("email", TypeName = "text")]
    public override string? Email { get; set; }

    [Column("active")]
    public bool Active { get; set; }

    [Column("avatar", TypeName = "text")]
    public string? Avatar { get; set; }

    [Column("name")]
    [StringLength(255)]
    public string Name { get; set; } = null!;

    [Column("phone")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Phone { get; set; }

    [Column("type")]
    [StringLength(10)]
    [Unicode(false)]
    public string Type { get; set; } = null!;

    [Column("sellerCode")]
    [StringLength(20)]
    [Unicode(false)]
    public string? SellerCode { get; set; }

    [Column("dateCreated", TypeName = "datetime")]
    public DateTime? DateCreated { get; set; } = null;

    [Column("dateModified", TypeName = "datetime")]
    public DateTime? DateModified { get; set; } = null;

    [InverseProperty("Customer")]
    public virtual ICollection<CustomerBookTrip> CustomerBookTrips { get; set; } = new List<CustomerBookTrip>();

    [InverseProperty("Customer")]
    public virtual ICollection<CustomerReviewTrip> CustomerReviewTrips { get; set; } = new List<CustomerReviewTrip>();

    [JsonIgnore]
    [InverseProperty("Driver")]
    public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();

    [InverseProperty("Sellers")]
    public virtual ICollection<Trip> SellingTrips { get; set; } = new List<Trip>();
}
