using SafouaneAntoineService.DAL.IDAL;

namespace SafouaneAntoineService.Models
{
    public class Notification
    {
        private int id;
        private string content;

        public int Id { get; set; }
        public int Content { get; set; }

        public Notification()
        {
        }

        public static void SendNotification(User user, string message)
        {
            //ça affiche une erreur en disant que user est null 
           // string content = $"{message}. Voici l'email de l'utilisateur : {recipient.Email}";

            string content = $"{message}";
           
        }

        public void SendNotification(INotificationDAL notificationDAL)
        {
            return notificationDAL.SendNotification(this);
        }

    }
}
