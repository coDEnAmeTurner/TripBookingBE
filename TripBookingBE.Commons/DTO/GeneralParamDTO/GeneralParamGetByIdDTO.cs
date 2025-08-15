using System.Net;
using TripBookingBE.Models;

namespace TripBookingBE.DTO.GeneralParamDTO;

public class GeneralParamGetByIdDTO
{
    public Models.GeneralParam GeneralParam { get; set; } = null;
    public HttpStatusCode RespCode = HttpStatusCode.OK;
    public string Message = string.Empty;
}