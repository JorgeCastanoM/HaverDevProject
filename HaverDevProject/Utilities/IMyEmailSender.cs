using HaverDevProject.ViewModels;

namespace HaverDevProject.Utilities
{
    public interface IMyEmailSender
    {
        Task SendOneAsync(string name, string email, string subject, string htmlMessage);
        Task SendToManyAsync(EmailMessage emailMessage);
    }
}
