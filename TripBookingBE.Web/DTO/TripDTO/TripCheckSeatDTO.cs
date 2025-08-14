using System.Net;

namespace TripBookingBE.DTO.TripDTO;

public class TripCheckSeatDTO
{
    public bool IsBooked { get; set; } = false;

    public int RespCode { get; set; } = (int)HttpStatusCode.OK;

    public string? Message {get; set; }

}   