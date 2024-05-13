using SafouaneAntoineService.DAL.IDAL;
using SafouaneAntoineService.Models;
using System.Data.SqlClient;

namespace SafouaneAntoineService.DAL
{
    public class NotificationDAL : INotificationDAL
    {
        private string connectionString; // Initialisez votre chaîne de connexion à la base de données

        public NotificationDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void SendNotification(User user, string message)
        {
            
            string content = $"{message}. Voici l'e-mail de l'utilisateur : {user.Email}";

           
            string query = "INSERT INTO Notification (content) VALUES (@content)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@content", content);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

    }
}
