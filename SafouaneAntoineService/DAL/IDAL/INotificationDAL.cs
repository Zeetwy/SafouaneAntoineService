using SafouaneAntoineService.Models;

namespace SafouaneAntoineService.DAL.IDAL
{
    public interface INotificationDAL
    {
        public void SendNotification(Notification notification);
        public List<Notification> GetNotifications(User user);
    }
}
