using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using TripBookingBE.Dal.DalInterfaces;
using TripBookingBE.Data;
using TripBookingBE.DTO.GeneralParamDTO;

public class GeneralParamDal : IGeneralParamDal
{
    private readonly TripBookingContext context;
    public GeneralParamDal(TripBookingContext context)
    {
        this.context = context;
    }

    public async Task<GeneralParamGetByIdDTO> GetGeneralParamById(long? id)
    {
        GeneralParamGetByIdDTO dto = new();
        try
        {
            var gp = await context.GeneralParams.FirstOrDefaultAsync(x => x.Id == id);
            if (gp == null)
            {
                dto.RespCode = System.Net.HttpStatusCode.InternalServerError;
                dto.Message = $"User with Id {id} not found!";
            }
            dto.GeneralParam = gp;
        }
        catch (Exception ex)
        {
            dto.RespCode = System.Net.HttpStatusCode.InternalServerError;
            dto.Message = ex.Message;
        }
        return dto;
    }

    public async Task<GeneralParamGetGeneralParamsDTO> GetGeneralParams(string? paramKey = null, string? paramCode = null)
    {
        GeneralParamGetGeneralParamsDTO dto = new();
        try
        {

            var prams = from g in context.GeneralParams select g;
            var list_prams = await (from p in prams
                                    where (paramKey == null || p.ParamKey.Contains(paramKey))
                                            && (paramCode == null || (p.ParamCode != null && p.ParamCode.Contains(paramCode)))
                                    select p).ToListAsync();

            dto.GeneralParams = list_prams;

        }
        catch (Exception ex)
        {
            dto.RespCode = System.Net.HttpStatusCode.InternalServerError;
            dto.Message = ex.Message;
        }

        return dto;
    }
}