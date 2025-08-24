using System.Net;
using System.Transactions;
using log4net.Core;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using TripBookingBE.Commons.Configurations;
using TripBookingBE.Commons.DTO.EmailDTO;
using TripBookingBE.Commons.DTO.TicketDTO;
using TripBookingBE.Commons.TicketDTO;
using TripBookingBE.Commons.VnPayLibrary;
using TripBookingBE.Dal.DalImplementations;
using TripBookingBE.Dal.DalInterfaces;
using TripBookingBE.DTO.TicketDTO;
using TripBookingBE.Models;
using TripBookingBE.Services.ServiceInterfaces;

namespace TripBookingBE.Services.ServiceImplementations;

public class TicketService : ITicketService
{
    private readonly ITicketDAL ticketDAL;
    private readonly IBookingsDal bookingDAL;
    private readonly IEmailService emailService;
    private readonly VnPayConfigs vnPayConfigs;

    private readonly VnPayLibrary vnpay;
    private readonly Utils utils;
    private readonly ILogger log;

    public TicketService(ITicketDAL ticketDAL, IBookingsDal bookingDAL, IOptions<VnPayConfigs> vnPayConfigs, VnPayLibrary vnPayLibrary = null, Utils utils = null, ILogger log = null, ISendGridClient sendGridClient = null, IEmailService emailService = null)
    {
        this.ticketDAL = ticketDAL;
        this.bookingDAL = bookingDAL;
        this.vnPayConfigs = vnPayConfigs.Value;
        vnpay = vnPayLibrary;
        this.utils = utils;
        this.log = log;
        this.emailService = emailService;
    }

    public async Task<TicketCheckOwnerDTO> CheckTicketOwner(long id, long userId)
    {
        var dto = new TicketCheckOwnerDTO();

        var ticketdto = await GetTicketById(id);
        if (ticketdto.Ticket == null || ticketdto.Ticket.CustomerBookTrip == null || ticketdto.Ticket.CustomerBookTrip.Customer == null || ticketdto.RespCode != HttpStatusCode.OK)
        {
            dto.RespCode = (int)ticketdto.RespCode;
            dto.Message = ticketdto.Message;
            return dto;
        }

        var ticket = ticketdto.Ticket;
        dto.IsOwner = userId == ticket.CustomerBookTrip.Customer.Id;
        return dto;
    }

    public async Task<TicketCheckSellerDTO> CheckTicketSeller(long id, string sellerCode)
    {
        var dto = new TicketCheckSellerDTO();

        var ticketdto = await GetTicketById(id);
        if (ticketdto.Ticket == null || ticketdto.RespCode != HttpStatusCode.OK)
        {
            dto.RespCode = (int)ticketdto.RespCode;
            dto.Message = ticketdto.Message;
            return dto;
        }

        var ticket = ticketdto.Ticket;
        dto.IsSeller = sellerCode == ticket.SellerCode;
        return dto;
    }

    public async Task<TicketCreateOrUpdateDTO> CreateOrUpdate(Ticket ticket)
    {
        TicketCreateOrUpdateDTO dto = new();

        if (ticket.CustomerBookTripId == 0)
        {
            var idDTO = await bookingDAL.GetIdByCustIdAndTripId(ticket.CustomerId, ticket.TripId);
            if (idDTO == null || idDTO.Ids == null || idDTO.Ids.Count == 0)
            {
                dto.Ticket = ticket;
                dto.RespCode = System.Net.HttpStatusCode.NotFound;
                dto.Message = $"Customer {ticket.CustomerId} doesn't book Trip {ticket.TripId}";
                return dto;
            }

            // var already_exist = await ticketDAL.GetTicketById(idDTO.Ids.FirstOrDefault());
            // if (already_exist.Ticket != null)
            // {
            //     dto.Ticket = ticket;
            //     dto.RespCode = System.Net.HttpStatusCode.Conflict;
            //     dto.Message = $"{already_exist.Ticket.CustomerBookTrip?.Customer.Name} already has a Ticket for {already_exist.Ticket.CustomerBookTrip?.Trip.Route?.RouteDescription} - {already_exist.Ticket.CustomerBookTrip?.Trip.DepartureTime.GetValueOrDefault().ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)}";
            //     return dto;
            // }

            ticket.CustomerBookTripId = idDTO.Ids.FirstOrDefault();

            dto = await ticketDAL.Create(ticket);
        }
        else
        {
            dto = await ticketDAL.Update(ticket);
        }

        EmailSendDTO maildto = new();
        if (dto.RespCode == System.Net.HttpStatusCode.Created)
        {
            maildto = await SendMailTicketCreationSuccessful(ticket);
            if (maildto.RespCode != 200)
            {
                dto.Message += maildto.Message;
            }
        }
        else
        {
            maildto = await SendMailTicketCreationFail(ticket);
            if (maildto.RespCode != 200)
            {
                dto.Message += maildto.Message;
            }
        }
        dto.Message += maildto.Message;

        return dto;
    }

    public async Task<TicketDeleteDTO> DeleteTicket(long id)
    {
        TicketDeleteDTO dto = new();
        using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            try
            {

                var ticketDTO = await ticketDAL.DeleteTicket(id);
                if (ticketDTO.RespCode != HttpStatusCode.NoContent)
                {
                    dto.RespCode = ticketDTO.RespCode;
                    dto.Message += $"\n{ticketDTO.Message}";
                }

                await bookingDAL.DeleteBooking(ticketDTO.Ticket.CustomerBookTrip);

                scope.Complete();
            }
            catch (Exception ex)
            {
                if (dto.RespCode == HttpStatusCode.NoContent)
                {
                    dto.RespCode = HttpStatusCode.InternalServerError;
                    dto.Message = ex.Message;
                }
            }

        }
        return dto;
    }

    public async Task<TicketGetCreateOrUpdateDTO> GetCreateOrUpdateModel(long? id)
    {
        TicketGetCreateOrUpdateDTO dto = new();
        if (id == null)
            dto.Ticket = new Models.Ticket();
        else
        {
            var dtoDAL = await ticketDAL.GetTicketById(id.GetValueOrDefault());
            dto.Ticket = dtoDAL.Ticket;
            dto.RespCode = dtoDAL.RespCode;
            dto.Message = dtoDAL.Message;
        }

        return dto;
    }

    public async Task<TicketGetByIdDTO> GetTicketById(long id)
    {
        var dto = await ticketDAL.GetTicketById(id);
        return dto;
    }

    public async Task<TicketGetTicketsDTO> GetTickets(long? customerId, long? tripId, decimal? fromPrice, decimal? toPrice, string? sellerCode, DateTime? departureTime, long? generalParamId)
    {
        var dto = await ticketDAL.GetTickets(customerId, tripId, fromPrice, toPrice, sellerCode, departureTime, generalParamId);
        return dto;
    }

    public async Task<TicketIPNDTO> IPN(string vnp_TmnCode, string vnp_Amount, string vnp_BankCode, string vnp_BankTranNo, string vnp_CardType, string vnp_PayDate, string vnp_OrderInfo, string vnp_TransactionNo, string vnp_ResponseCode, string vnp_TransactionStatus, string vnp_TxnRef, string vnp_SecureHash, string url)
    {
        var dto = new TicketIPNDTO();

        string vnp_HashSecret = vnPayConfigs.vnp_HashSecret;
        var parts = vnp_TxnRef.Split('|');
        long ticketId = Convert.ToInt64(parts[0]);
        long lvnp_Amount = Convert.ToInt64(vnp_Amount) / 100;
        long vnpayTranId = Convert.ToInt64(vnp_TransactionNo);
        bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
        if (checkSignature)
        {
            var modeldto = await ticketDAL.GetTicketById(ticketId);
            if (modeldto.RespCode != HttpStatusCode.OK)
            {
                dto.RespCode = (int)modeldto.RespCode;
                dto.Message = modeldto.Message;
                return dto;
            }

            var ticket = modeldto.Ticket;
            if (ticket != null)
            {
                if (ticket.Price == Convert.ToDecimal(vnp_Amount))
                {
                    if (ticket.Paid == 0)
                    {
                        EmailSendDTO emaildto = new();
                        if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                        {
                            //Thanh toan thanh cong
                            Console.WriteLine("Thanh toan thanh cong, TicketId={0}, VNPAY TranId={1}", ticketId,
                                vnpayTranId);
                            ticket.Paid = 1;

                            emaildto = await SendMailTicketPaySuccessful(ticket);
                            if (emaildto.RespCode != (int)HttpStatusCode.OK)
                            {
                                dto.Message += emaildto.Message;
                            }
                        }
                        else
                        {
                            //Thanh toan khong thanh cong. Ma loi: vnp_ResponseCode
                            //  displayMsg.InnerText = "Có lỗi xảy ra trong quá trình xử lý.Mã lỗi: " + vnp_ResponseCode;
                            Console.WriteLine("Thanh toan loi, TicketId={0}, VNPAY TranId={1},ResponseCode={2}",
                                ticketId,
                                vnpayTranId, vnp_ResponseCode);
                            ticket.Paid = 2;
                            emaildto = await SendMailTicketPayFail(ticket);
                            if (emaildto.RespCode != (int)HttpStatusCode.OK)
                            {
                                dto.Message += emaildto.Message;
                            }
                        }

                        var updatedto = await ticketDAL.Update(ticket);
                        if (updatedto.RespCode != HttpStatusCode.OK)
                        {
                            dto.Message += updatedto.Message;
                        }

                        dto.RspCode = "00";
                        dto.Message = "Confirm Success";
                        return dto;
                    }
                    else
                    {
                        dto.RspCode = "02";
                        dto.Message = "Ticket already confirmed";
                        return dto;
                    }
                }
                else
                {
                    dto.RspCode = "04";
                    dto.Message = "invalid amount";
                    return dto;
                }
            }
            else
            {
                dto.RspCode = "01";
                dto.Message = "Ticket not found";
                return dto;
            }
        }
        else
        {
            Console.WriteLine("Invalid signature, InputData={0}", url);
            dto.RspCode = "97";
            dto.Message = "Invalid signature";
            return dto;
        }
    }

    public async Task<TicketPayDTO> Pay(long id)
    {
        var dto = new TicketPayDTO();

        var modeldto = await ticketDAL.GetTicketById(id);

        if (modeldto.RespCode != HttpStatusCode.OK)
        {
            dto.RespCode = (int)modeldto.RespCode;
            dto.Message = dto.Message;
            return dto;
        }

        var ticket = modeldto.Ticket;

        //Get Config Info
        string vnp_Returnurl = vnPayConfigs.vnp_Returnurl; //URL nhan ket qua tra ve 
        string vnp_Url = vnPayConfigs.vnp_Url; //URL thanh toan cua VNPAY 
        string vnp_TmnCode = vnPayConfigs.vnp_TmnCode; //Ma định danh merchant kết nối (Terminal Id)
        string vnp_HashSecret = vnPayConfigs.vnp_HashSecret; //Secret Key

        vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
        vnpay.AddRequestData("vnp_Command", "pay");
        vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
        vnpay.AddRequestData("vnp_Amount", (Convert.ToInt64(ticket.Price) * 100).ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000

        vnpay.AddRequestData("vnp_CreateDate", ticket.DateCreated.Value.ToString("yyyyMMddHHmmss"));
        vnpay.AddRequestData("vnp_ExpireDate", DateTime.Now.AddDays(7).ToString("yyyyMMddHHmmss"));
        vnpay.AddRequestData("vnp_CurrCode", "VND");
        vnpay.AddRequestData("vnp_IpAddr", utils.GetIpAddress());

        vnpay.AddRequestData("vnp_Locale", "vn");
        vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang: " + ticket.CustomerBookTripId);
        vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other

        vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
        vnpay.AddRequestData("vnp_TxnRef", $"{ticket.CustomerBookTripId}|{Guid.NewGuid().ToString()}"); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày

        string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
        Console.WriteLine($"VNPAY URL: {paymentUrl}");

        dto.PaymentURL = paymentUrl;

        return dto;
    }

    public async Task<TicketReturnUrlDTO> ReturnUrl(string vnp_TmnCode, string vnp_Amount, string vnp_BankCode, string vnp_BankTranNo, string vnp_CardType, string vnp_PayDate, string vnp_OrderInfo, string vnp_TransactionNo, string vnp_ResponseCode, string vnp_TransactionStatus, string vnp_TxnRef, string vnp_SecureHash, string url)
    {
        var dto = new TicketReturnUrlDTO();
        string vnp_HashSecret = vnPayConfigs.vnp_HashSecret; //Chuoi bi mat

        //vnp_TxnRef: Ma don hang merchant gui VNPAY tai command=pay    
        //vnp_TransactionNo: Ma GD tai he thong VNPAY
        //vnp_ResponseCode:Response code from VNPAY: 00: Thanh cong, Khac 00: Xem tai lieu
        //vnp_SecureHash: HmacSHA512 cua du lieu tra ve

        var parts = vnp_TxnRef.Split('|');
        long ticketId = Convert.ToInt64(parts[0]);
        long vnpayTranId = Convert.ToInt64(vnp_TransactionNo);
        long lvnp_Amount = Convert.ToInt64(vnp_Amount) / 100;

        bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);

        Console.WriteLine("[TicketService.ReturnUrl] vnp_SecureHash: {0}", vnp_SecureHash);
        Console.WriteLine("[TicketService.ReturnUrl] vnp_HashSecret: {0}", vnp_HashSecret);
        Console.WriteLine("[TicketService.ReturnUrl] checkSignature: {0}", checkSignature);

        if (checkSignature)
        {
            if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
            {
                //Thanh toan thanh cong
                dto.displayMsg = "Giao dịch được thực hiện thành công. Cảm ơn quý khách đã sử dụng dịch vụ";
                Console.WriteLine("Thanh toan thanh cong, OrderId={0}, VNPAY TranId={1}", ticketId, vnpayTranId);
            }
            else
            {
                //Thanh toan khong thanh cong. Ma loi: vnp_ResponseCode
                dto.displayMsg = "Có lỗi xảy ra trong quá trình xử lý.Mã lỗi: " + vnp_ResponseCode;
                Console.WriteLine("Thanh toan loi, OrderId={0}, VNPAY TranId={1},ResponseCode={2}", ticketId,
                vnpayTranId, vnp_ResponseCode);
            }
            dto.displayTmnCode = "Mã Website (Terminal ID):" + vnp_TmnCode;
            dto.displayTxnRef = "Mã giao dịch thanh toán:" + ticketId.ToString();
            dto.displayVnpayTranNo = "Mã giao dịch tại VNPAY:" + vnpayTranId.ToString();
            dto.displayAmount = "Số tiền thanh toán (VND):" + lvnp_Amount.ToString();
            dto.displayBankCode = "Ngân hàng thanh toán:" + vnp_BankCode;
        }
        else
        {
            Console.WriteLine("Invalid signature, InputData={0}", url);
            dto.displayMsg = "Có lỗi xảy ra trong quá trình xử lý";
        }

        return dto;
    }

    public async Task<EmailSendDTO> SendTicketCreationMailToTicketOwner(long ticketId)
    {
        var dto = new EmailSendDTO();

        try
        {
            var ticketdto = await ticketDAL.GetTicketById(ticketId);
            var ticket = ticketdto.Ticket;

            if (ticket != null)
            {
                dto = await SendMailTicketCreationSuccessful(ticket);
            }
            else
            {
                dto = await SendMailTicketCreationFail(ticket);
            }

        }
        catch (Exception ex)
        {
            dto.RespCode = 500;
            dto.Message += $"{ex.Message};{ex.InnerException.Message}";
        }

        return dto;
    }

    public async Task<EmailSendDTO> SendMailTicketCreationSuccessful(Ticket ticket)
    {
        string message = File.ReadAllText("EmailTemplates/SuccessfulTicketCreation.html")
        .Replace("[DateCreated]", ticket.DateCreated.Value.ToString("dd/MM/yyyy"))
        .Replace("[CustomerName]", ticket.CustomerBookTrip.Customer.Name)
        .Replace("[CustomerPhone]", ticket.CustomerBookTrip.Customer.Phone)
        .Replace("[Route]", ticket.CustomerBookTrip.Trip.Route.RouteDescription)
        .Replace("[PlateNumber]", ticket.CustomerBookTrip.Trip.RegistrationNumber)
        .Replace("[SellerCode]", ticket.SellerCode);

        string plain = $@"
                - Created Date: {ticket.DateCreated.Value.ToString("dd/MM/yyyy")}
                - Customer Name: {ticket.CustomerBookTrip.Customer.Name}
                - Customer Phone: {ticket.CustomerBookTrip.Customer.Phone}
                - Route: {ticket.CustomerBookTrip.Trip.Route.RouteDescription}
                - Vehicle Plate Number: {ticket.CustomerBookTrip.Trip.RegistrationNumber}
                - Seller Code: {ticket.SellerCode}
        ";

        var maildto = await emailService.SendMail(ticket.CustomerBookTrip.Customer.Email, "[TripBooking] Ticket Input Completed!", message, plain);
        if (maildto.RespCode != 200)
        {
            maildto.Message += $"The ticket has been created/updated, but the mail send fails. {maildto.Message}";
            Console.WriteLine(maildto.Message);
            return maildto;
        }

        return maildto;
    }

    public async Task<EmailSendDTO> SendMailTicketCreationFail(Ticket ticket)
    {
        var bookingdto = await bookingDAL.GetBookingById(ticket.CustomerBookTripId);
        var sellers = bookingdto.CustomerBookTrip.Trip.Sellers;
        var sellerNames = string.Join(";", sellers.Select(x => x.Name).ToList());
        var sellerPhones = string.Join(";", sellers.Select(x => x.Phone).ToList());

        string message = File.ReadAllText("EmailTemplates/UnsuccessfulTicketCreation.html");

        message=message.Replace("[BookingId]", ticket.CustomerBookTripId.ToString())
        .Replace("[SellerName]", sellerNames)
        .Replace("[SellerPhone]", sellerPhones);

        string plain = $@"
                - Booking Id: {ticket.CustomerBookTripId.ToString()}
                - Seller Name: {sellerNames}
                - Seller Phone: {sellerPhones}
        ";

        var maildto = await emailService.SendMail(ticket.CustomerBookTrip.Customer.Email, "[TripBooking] Ticket Creation Failed!", message, plain);
        if (maildto.RespCode != 200)
        {
            maildto.Message += $"The ticket creation failed and the mail send fails too. {maildto.Message}";
            Console.WriteLine(maildto.Message);
            return maildto;
        }

        return maildto;
    }

    public async Task<EmailSendDTO> SendMailTicketPaySuccessful(Ticket ticket)
    {
        string message = File.ReadAllText("EmailTemplates/SuccessfulTicketPay.html")

        .Replace("[CustomerName]", ticket.CustomerBookTrip.Customer.Name)
        .Replace("[CustomerPhone]", ticket.CustomerBookTrip.Customer.Phone)
        .Replace("[Route]", ticket.CustomerBookTrip.Trip.Route.RouteDescription)
        .Replace("[PlateNumber]", ticket.CustomerBookTrip.Trip.RegistrationNumber)
        .Replace("[Price]", ticket.Price.ToString())
        .Replace("[PaymentDate]", DateTime.Now.ToString("dd/MM/yyyy"));

        string plain = $@"
                - Customer Name: {ticket.CustomerBookTrip.Customer.Name}
                - Customer Phone: {ticket.CustomerBookTrip.Customer.Phone}
                - Route: {ticket.CustomerBookTrip.Trip.Route.RouteDescription}
                - Vehicle Plate Number: {ticket.CustomerBookTrip.Trip.RegistrationNumber}
                - Price: {ticket.Price}
                - Payment Date: {DateTime.Now.ToString("dd/MM/yyyy")}
        ";

        var maildto = await emailService.SendMail(ticket.CustomerBookTrip.Customer.Email, "[TripBooking] Ticket Payment Completed!", message, plain);
        if (maildto.RespCode != 200)
        {
            maildto.Message += $"The ticket has been paid, but the mail send fails. {maildto.Message}";
            Console.WriteLine(maildto.Message);
            return maildto;
        }

        return maildto;
    }

    public async Task<EmailSendDTO> SendMailTicketPayFail(Ticket ticket)
    {
        var bookingdto = await bookingDAL.GetBookingById(ticket.CustomerBookTripId);
        var sellers = bookingdto.CustomerBookTrip.Trip.Sellers;
        var sellerNames = string.Join(";", sellers.Select(x => x.Name).ToList());
        var sellerPhones = string.Join(";", sellers.Select(x => x.Phone).ToList());

        string message = File.ReadAllText("EmailTemplates/UnsuccessfulTicketPay.html")
        .Replace("[TicketId]", ticket.CustomerBookTripId.ToString())
        .Replace("[Price]", ticket.Price.ToString());

        string plain = $@"
                - Ticket Id: {ticket.CustomerBookTripId}
                - Price: {ticket.Price}
        ";

        var maildto = await emailService.SendMail(ticket.CustomerBookTrip.Customer.Email, "[TripBooking] Ticket Payment Failed!", message, plain);
        if (maildto.RespCode != 200)
        {
            maildto.Message += $"The ticket payment failed and the mail send fails too. {maildto.Message}";
            Console.WriteLine(maildto.Message);
            return maildto;
        }

        return maildto;
    }

    public async Task<EmailSendDTO> SendTicketPayMailToTicketOwner(long ticketId)
    {
        var dto = new EmailSendDTO();

        try
        {
            var ticketdto = await ticketDAL.GetTicketById(ticketId);
            var ticket = ticketdto.Ticket;

            if (ticket.Paid == 1)
            {
                dto = await SendMailTicketPaySuccessful(ticket);
            }
            else if (ticket.Paid == 2)
            {
                dto = await SendMailTicketCreationFail(ticket);
            }
            else
            {
                dto.RespCode = (int)HttpStatusCode.BadRequest;
                dto.Message = "The ticket is not paid yet!";
            }

        }
        catch (Exception ex)
        {
            dto.RespCode = 500;
            dto.Message += $"{ex.Message};{ex.InnerException.Message}";
        }

        return dto;
    }
}