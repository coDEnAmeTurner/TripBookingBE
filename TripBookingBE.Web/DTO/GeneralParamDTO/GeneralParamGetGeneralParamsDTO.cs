using System.Net;
using TripBookingBE.Models;

namespace TripBookingBE.DTO.GeneralParamDTO;

public class GeneralParamGetGeneralParamsDTO
{
    public List<GeneralParam> GeneralParams = null;

    public HttpStatusCode RespCode = HttpStatusCode.NoContent;

    public string Message = string.Empty;
}