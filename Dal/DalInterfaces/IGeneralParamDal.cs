using TripBookingBE.DTO.GeneralParamDTO;

namespace TripBookingBE.Dal.DalInterfaces;

public interface IGeneralParamDal
{
    Task<GeneralParamGetGeneralParamsDTO> GetGeneralParams(string? paramKey = null, string? paramCode = null);
}