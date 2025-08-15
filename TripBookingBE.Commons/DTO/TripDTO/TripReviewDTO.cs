using System.Net;
using TripBookingBE.Models;

namespace TripBookingBE.DTO.TripDTO;

public class TripReviewDTO
{
    public CustomerReviewTrip? Review { get; set; }
    public HttpStatusCode RespCode { get; set; } = HttpStatusCode.Created;

    public string? Message {get;set;}
}