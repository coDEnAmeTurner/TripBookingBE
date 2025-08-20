namespace TripBookingBE.Commons.DTO.TicketDTO;

public class TicketReturnUrlDTO
{
    public int RespCode { get; set; } = 200;
    public string displayMsg { get; set; } = "";
    public string displayTmnCode { get; set; } = "";
    public string displayTxnRef { get; set; } = "";
    public string displayVnpayTranNo { get; set; } = "";
    public string displayAmount { get; set; } = "";
    public string displayBankCode { get; set; } = "";
}