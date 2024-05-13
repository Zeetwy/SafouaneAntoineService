using SafouaneAntoineService.DAL.IDAL;

namespace SafouaneAntoineService.Models
{
    public class Notification
    {
        private int id;
        private string content;
        private User target;

        public int Id { get => id; }
        public string Content { get => content; }
        public User Target { get => target; }

        public Notification(User target, string content)
        {
            this.target = target;
            this.content = content;
        }

        public void Send(INotificationDAL notificationDAL)
        {
            notificationDAL.SendNotification(this);
        }
    }
}
