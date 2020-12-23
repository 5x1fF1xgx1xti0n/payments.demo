namespace Payments.API.Models.ViewModels
{
    public class SessionData
    {
        public SessionData(int userId, string userRole)
        {
            UserId = userId;
            UserRole = userRole;
        }

        public int UserId { get; set; }
        public string UserRole { get; set; }
    }
}
