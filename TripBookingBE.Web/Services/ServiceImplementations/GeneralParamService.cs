using TripBookingBE.Dal.DalInterfaces;
using TripBookingBE.DTO.GeneralParamDTO;
using TripBookingBE.Services.ServiceInterfaces;

public class GeneralParamService : IGeneralParamService
{
    private readonly IGeneralParamDal generalParamDal;

    public GeneralParamService(IGeneralParamDal generalParamDal)
    {
        this.generalParamDal = generalParamDal;
    }

    public async Task<GeneralParamGetGeneralParamsDTO> GetGeneralParams(string? paramKey = null, string? paramCode = null)
    {
        return await generalParamDal.GetGeneralParams(paramKey, paramCode);
    }
}