using SafouaneAntoineService.DAL.IDAL;

namespace SafouaneAntoineService.Models
{
    public class Notification
    {
        private readonly int id;
        private readonly string content;
        private readonly User target;

        public int Id { get => id; }
        public string Content { get => content; }
        public User Target { get => target; }

        public Notification(int id, User target, string content)
        {
            this.id = id;
            this.content = content;
            this.target = target;
        }

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
