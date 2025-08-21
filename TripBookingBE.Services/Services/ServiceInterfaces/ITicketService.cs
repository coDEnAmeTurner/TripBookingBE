using TripBookingBE.Commons.DTO.TicketDTO;
using TripBookingBE.Commons.TicketDTO;
using TripBookingBE.DTO.TicketDTO;
using TripBookingBE.Models;

namespace TripBookingBE.Services.ServiceInterfaces;

public interface ITicketService
{
    Task<TicketGetTicketsDTO> GetTickets(long? customerId, long? tripId, decimal? fromPrice, decimal? toPrice, string? sellerCode, DateTime? departureTime, long? generalParamId);

    Task<TicketGetCreateOrUpdateDTO> GetCreateOrUpdateModel(long? id);

    Task<TicketCreateOrUpdateDTO> CreateOrUpdate(Ticket trip);

    Task<TicketGetByIdDTO> GetTicketById(long id);

    Task<TicketDeleteDTO> DeleteTicket(long id);

    Task<TicketCheckOwnerDTO> CheckTicketOwner(long id, long userId);
    Task<TicketCheckSellerDTO> CheckTicketSeller(long id, string sellerCode);
    Task<TicketPayDTO> Pay(long id);
    Task<TicketIPNDTO> IPN(
        string vnp_TmnCode,
        string vnp_Amount,
        string vnp_BankCode,
        string vnp_BankTranNo,
        string vnp_CardType,
        string vnp_PayDate,
        string vnp_OrderInfo,
        string vnp_TransactionNo,
        string vnp_ResponseCode,
        string vnp_TransactionStatus,
        string vnp_TxnRef,
        string vnp_SecureHash,
        string url
    );

    Task<TicketReturnUrlDTO> ReturnUrl(
        string vnp_TmnCode,
        string vnp_Amount,
        string vnp_BankCode,
        string vnp_BankTranNo,
        string vnp_CardType,
        string vnp_PayDate,
        string vnp_OrderInfo,
        string vnp_TransactionNo,
        string vnp_ResponseCode,
        string vnp_TransactionStatus,
        string vnp_TxnRef,
        string vnp_SecureHash,
        string url
    );
}