using SafouaneAntoineService.Models;

namespace SafouaneAntoineService.DAL.IDAL
{
    public interface INotificationDAL
    {
        public void SendNotification(Notification notification);
    }
}
