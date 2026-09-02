namespace CarRentalSystem.Services
{
    public interface IEmailService
    {
        Task SendPasswordResetEmailAsync(
            string email,
            string resetLink);
    }
}