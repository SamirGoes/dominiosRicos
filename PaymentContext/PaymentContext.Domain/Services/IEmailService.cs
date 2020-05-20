namespace PaymentContext.Domain.Services
{
    public interface IEmailService
    {
        void Send(string sto, string email, string subject, string body);
    }   
}