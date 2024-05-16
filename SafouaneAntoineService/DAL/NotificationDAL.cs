using SafouaneAntoineService.DAL.IDAL;
using SafouaneAntoineService.Models;
using System.Data;
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
            const string query = "INSERT INTO [Notification] ([content], [user_id]) VALUES (@content, @user_id)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("content", notification.Content);
                command.Parameters.AddWithValue("user_id", notification.Target.Id);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public List<Notification> GetNotifications(User user)
        {
            const string query = "SELECT [id], [content] FROM [Notification] WHERE [user_id] = @user_id";

            List<Notification> notifications = new List<Notification>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, connection);

                cmd.Parameters.AddWithValue("user_id", user.Id);

                connection.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        notifications.Add(
                            new Notification(
                                reader.GetInt32("id"),
                                user,
                                reader.GetString("content")
                            )
                        );
                    }
                }
            }
            return notifications;
        }
    }
}
