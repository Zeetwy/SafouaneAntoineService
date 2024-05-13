using SafouaneAntoineService.DAL.IDAL;

namespace SafouaneAntoineService.Models
{
    public class Notification
    {
        private int id;
        private string content;
        private User target;

        public int Id
        {
            get => id;
            private set => id = value;
        }
        public string Content
        {
            get => content;
        }

        public User Target
        {
            get => target;
        }

        public Notification(User target, string content)
        {
            this.target = target;
            this.content = content;
        }

        public static void SendNotification(User user, string message)
        {
            //ça affiche une erreur en disant que user est null 
            // string content = $"{message}. Voici l'email de l'utilisateur : {recipient.Email}";

            string content = $"{message}";
        }

        public void Send(INotificationDAL notificationDAL)
        {
            notificationDAL.SendNotification(this);
        }

    }
}
