using SafouaneAntoineService.DAL.IDAL;
using SafouaneAntoineService.Models;
using System.Data;
using System.Data.SqlClient;

namespace SafouaneAntoineService.DAL
{
    public class UserDAL : IUserDAL  
    {
        private string connectionString;

        public UserDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public User? Authenticate(string username, string password)
        {
            User? u = null;
            const string query = "SELECT * FROM [User] where Username = @username and Password = @password";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, connection);

                cmd.Parameters.AddWithValue("username", username); 
                cmd.Parameters.AddWithValue("password", password);

                connection.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        u = new User(
                            reader.GetInt32("Id"),
                            reader.GetString("Lastname"),
                            reader.GetString("Firstname"),
                            reader.GetString("Username"),
                            reader.GetInt32("Timecredits"),
                            reader.GetString("Email"),
                            null //le mdp n'est pas inclus dans la requête SQL pour des raisons de sécurité.
                        );
                    }
                }
            }
            return u;
        }

        public bool SaveAccount(User u)
        {
            bool success = false;

            // pour rechercher un utilisateur existant
            const string query = @"INSERT INTO [User](LastName, Firstname, Username, Password, Email, Timecredits)
    VALUES (@LastName, @Firstname, @Username, @Password, @Email, @Timecredits)";

            // vérifier si un utilisateur existe déjà avec le même nom d'utilisateur ou la même adresse e-mail
            const string query2 = "SELECT * FROM [User] WHERE Username = @Username OR Email = @Email";

            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                SqlCommand cmd = new SqlCommand(query2, connection);
                cmd.Parameters.AddWithValue("Username", u.Username);
                cmd.Parameters.AddWithValue("Email", u.Email);
                connection.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        // Un utilisateur correspondant a été trouvé, ne pas insérer
                        success = false;
                    }
                    else
                    {
                        success = true;

                        using (SqlConnection insertConnection = new SqlConnection(this.connectionString))
                        {
                            try
                            {
                                // insertion d'un nouvel utilisateur
                                SqlCommand insertCmd = new SqlCommand(query, insertConnection);
                                insertCmd.Parameters.AddWithValue("Lastname", u.Lastname);
                                insertCmd.Parameters.AddWithValue("Firstname", u.Firstname);
                                insertCmd.Parameters.AddWithValue("Username", u.Username);
                                insertCmd.Parameters.AddWithValue("Password", u.Password);
                                insertCmd.Parameters.AddWithValue("Email", u.Email);
                                insertCmd.Parameters.AddWithValue("Timecredits", u.Timecredits);
                                insertConnection.Open();

                                success = insertCmd.ExecuteNonQuery() > 0;
                            }
                            catch (Exception ex)
                            {
                                success = false;
                                Console.WriteLine($"Erreur lors de l'insertion de l'utilisateur : {ex.Message}");
                            }
                        }
                    }
                }
            }

            return success;
        }

        public bool Debit(User u, int amount)
        {
            const string query = "UPDATE [User] SET Timecredits = Timecredits - @Amount WHERE Id = @UserId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Amount", amount);
                command.Parameters.AddWithValue("@UserId", u.Id);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public bool Credit(User u, int amount)
        {
            const string query = "UPDATE [User] SET Timecredits = Timecredits + @Amount WHERE Id = @UserId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Amount", amount);
                command.Parameters.AddWithValue("@UserId", u.Id);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public void RefreshInfo(ref User user)
        {
            const string query = "SELECT [Timecredits], [Email] FROM [User] WHERE [Id] = @id";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("id", user.Id);
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user.Timecredits = reader.GetInt32("Timecredits");
                        user.Email = reader.GetString("Email");
                    }
                }
            }
        }

        public bool ChangeContact(User user, string email)
        {
            const string query = "UPDATE [User] SET [Email] = @email WHERE [Id] = @id";

            int rowsAffected = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("id", user.Id);
                command.Parameters.AddWithValue("email", email);
                connection.Open();
                command.ExecuteNonQuery();
            }
            return rowsAffected > 0;
        }
    }
}
