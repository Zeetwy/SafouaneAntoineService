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

        public void SendNotification(Notification notification)
        {
            string query = "INSERT INTO [Notification] ([content], [user_id]) VALUES (@content, @user_id)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("content", notification.Content);
                command.Parameters.AddWithValue("user_id", notification.Target.Id);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

    }
}
