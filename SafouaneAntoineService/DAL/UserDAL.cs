using SafouaneAntoineService.DAL.IDAL;
using SafouaneAntoineService.Models;
using System.Collections.Generic;
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

            using (SqlConnection connection = new SqlConnection(connectionString)) //on crée la chaîne de connection 
            {
                SqlCommand cmd = new SqlCommand(query, connection); //ça permet de préparer la resuete 
                //ou :       SqlCommand cmd = new SqlCommand("SELECT * FROM [User] where Username = @username and Password = @password", connection);

                //Les paramètres @username et @password sont ajoutés à la commande SQL en utilisant AddWithValue pour éviter les injections SQL et sécuriser la requête.
                cmd.Parameters.AddWithValue("username", username); 
                cmd.Parameters.AddWithValue("password", password);

                connection.Open();

                using (SqlDataReader reader = cmd.ExecuteReader()) //on exécute notre commande SQL avec executeReader
                {
                    if (reader.Read()) //Pour chaque enregistrement lu dans le résultat de la requête, un nouvel objet User est créé en utilisant les valeurs lues dans la BD
                    {
                        u = new User(
                            reader.GetInt32("Id"),
                            reader.GetString("Lastname"),
                            reader.GetString("Firstname"),
                            reader.GetString("Username"),
                            reader.GetInt32("Timecredits"),
                            reader.GetString("Email"),
                            null //le mdp n'est pas inclus dans la requête SQL pour des raisons de sécurité. Il est généralement comparé à celui fourni par l'utilisateur à l'aide d'une vérification de hachage.
                        );
                    }
                }
            }
            return u;
        }

        public bool SaveAccount(User u)
        {
            // Initialisation de la variable de succès
            bool success = false;

            // Définition de la requête SQL pour rechercher un utilisateur existant
            const string query = @"INSERT INTO [User](LastName, Firstname, Username, Password, Email, Timecredits)
    VALUES (@LastName, @Firstname, @Username, @Password, @Email, @Timecredits)";

            // Requête SQL pour vérifier si un utilisateur existe déjà avec le même nom d'utilisateur ou la même adresse e-mail
            const string query2 = "SELECT * FROM [User] WHERE Username = @Username OR Email = @Email";

            // Ouverture de la connexion à la base de données
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                // Création de la commande SQL pour la première requête de recherche
                SqlCommand cmd = new SqlCommand(query2, connection);
                cmd.Parameters.AddWithValue("Username", u.Username);
                cmd.Parameters.AddWithValue("Email", u.Email);
                connection.Open();

                // Exécution de la commande et vérification de l'existence d'un utilisateur correspondant
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        // Un utilisateur correspondant a été trouvé, ne pas insérer
                        success = false;
                    }
                    else
                    {
                        // Aucun utilisateur correspondant trouvé, insérer le nouvel utilisateur
                        success = true;

                        // Ouverture d'une nouvelle connexion à la base de données pour l'insertion
                        using (SqlConnection insertConnection = new SqlConnection(this.connectionString))
                        {
                            try
                            {
                                // Création de la commande SQL pour l'insertion d'un nouvel utilisateur
                                SqlCommand insertCmd = new SqlCommand(query, insertConnection);
                                insertCmd.Parameters.AddWithValue("Lastname", u.Lastname);
                                insertCmd.Parameters.AddWithValue("Firstname", u.Firstname);
                                insertCmd.Parameters.AddWithValue("Username", u.Username);
                                insertCmd.Parameters.AddWithValue("Password", u.Password);
                                insertCmd.Parameters.AddWithValue("Email", u.Email);
                                insertCmd.Parameters.AddWithValue("Timecredits", u.Timecredits);
                                insertConnection.Open();

                                // Exécution de la commande et vérification du succès de l'insertion
                                success = insertCmd.ExecuteNonQuery() > 0;
                            }
                            catch (Exception ex)
                            {
                                // Une erreur s'est produite lors de l'insertion
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

        /*public User RefreshInfo(User user)
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
            return user;
        }*/
    }
}
