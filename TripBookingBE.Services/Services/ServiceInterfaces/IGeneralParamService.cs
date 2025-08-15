using TripBookingBE.DTO.GeneralParamDTO;

namespace TripBookingBE.Services.ServiceInterfaces;

public interface IGeneralParamService
{
    Task<GeneralParamGetGeneralParamsDTO> GetGeneralParams(string? paramKey = null, string? paramCode = null);
}