using System.Data;
using System.Data.SqlClient;
using SafouaneAntoineService.DAL.IDAL;
using SafouaneAntoineService.Models;
using SafouaneAntoineService.ViewModels;

namespace SafouaneAntoineService.DAL
{
    public class ServiceOfferDAL : IServiceOfferDAL
    {
        private readonly string connection_string;

        public ServiceOfferDAL(string connection_string)
        {
            this.connection_string = connection_string;
        }

        //Pour afficher la liste des offres de services
        public List<ServiceOffer> GetServices()
        {
            List<ServiceOffer> serviceOffers = new List<ServiceOffer>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    SqlCommand cmd = new SqlCommand("SELECT * FROM ServiceOffer", connection);

                    connection.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader()) // ExecuteReader() est appelée pour exécuter la commande SQL et obtenir un objet SqlDataReader pour lire les résultats.
                    {
                        while (reader.Read())
                        {
                            //chaque ligne de résultat est lue à l'aide de la méthode Read() du SqlDataReader. Pour chaque ligne lue, un nouvel objet ServiceOffer est créé et initialisé avec les valeurs des colonnes de la base de données correspondantes

                            ServiceOffer serviceOffer = new ServiceOffer
                            {
                                Id = reader.GetInt32("id"),
                                Type = reader.GetString("type"),
                                Description = reader.GetString("description")
                            };
                            serviceOffers.Add(serviceOffer);
                        }
                    }
                }

                return serviceOffers; // Une fois toutes les offres de service récupérées et ajoutées à la liste serviceOffers, la liste est renvoyée comme résultat de la méthode.
            }
            catch (Exception)
            {
                return null;
            }

        }

        //Pour afficher un service en particulier
        public ServiceOffer GetService(int id)
        {
            ////jointure pour récupérer les informations de l'utilisateur fournisseur et de la catégorie de service correspondante.
            string query = "SELECT * " +
                           "FROM [ServiceOffer] " +
                           "JOIN [User] ON [ServiceOffer].user_id = [User].Id " +
                           "JOIN [ServiceCategory] ON [ServiceOffer].category_id= [ServiceCategory].id " +
                           "WHERE ServiceOffer.id = @id";

            ServiceOffer serviceOffer = null; //C'est cette variable qui sera renvoyée à la fin de la méthode avec les détails de l'offre de service récupérée.

            try
            {
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@id", id); //on ajoute un paramètre nommé @id avec la valeur de l'ID du service qu'on souhaite récupérer

                    connection.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader()) //on lit la requete
                    {
                        while (reader.Read())
                        {
                            User user = new User(
                                reader.GetInt32("Id"),
                                reader.GetString("Firstname"),
                                reader.GetString("Lastname"),
                                null,
                                0,
                                null,
                                null
                            ); //on évite de charger inutilement des données supplémentaires à partir de la base de données lorsqu'elles ne sont pas nécessaires pour l'affichage dans la vue. 

                            ServiceCategory serviceCategory = new ServiceCategory(

                                reader.GetString("name")
                            );

                            serviceOffer = new ServiceOffer
                            {
                                Id = reader.GetInt32("id"),
                                Type = reader.GetString("type"),
                                Description = reader.GetString("description"),
                                Category = serviceCategory,
                                Provider = user
                            };
                        }
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

            return serviceOffer;
        }



        public List<ServiceOffer> GetOffersByUser(User user)
        {
            const string query = "SELECT [so].[id], [so].[type], [so].[description], [sc].[name] FROM [ServiceOffer] so JOIN [ServiceCategory] sc ON [sc].[id] = [so].[category_id] WHERE [user_id]=@user_id";

            List<ServiceOffer> ret = new List<ServiceOffer>();

            using (SqlConnection connection = new SqlConnection(this.connection_string))
            {
                SqlCommand cmd = new SqlCommand(query, connection);

                cmd.Parameters.AddWithValue("user_id", user.Id);

                connection.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ret.Add(
                            new ServiceOffer(
                                reader.GetInt32("id"),
                                reader.GetString("type"),
                                reader.GetString("description"),
                                new ServiceCategory(reader.GetString("name"))
                            )
                        );
                    }
                }
            }
            return ret;
        }

        public bool PublishOffer(User user, ServiceOffer so)
        {
            const string query = "INSERT INTO [ServiceOffer] ([type], [description], [user_id], [category_id]) VALUES (@type, @description, @user_id, (SELECT [id] FROM [ServiceCategory] WHERE [name]=@category_name))";

            int rows_affected = 0;

            using (SqlConnection connection = new SqlConnection(this.connection_string))
            {
                SqlCommand cmd = new SqlCommand(query, connection);

                cmd.Parameters.AddWithValue("type", so.Type);
                cmd.Parameters.AddWithValue("description", so.Description??"");
                cmd.Parameters.AddWithValue("user_id", user.Id);
                cmd.Parameters.AddWithValue("category_name", so.Category.Name);

                connection.Open();
                rows_affected = cmd.ExecuteNonQuery();
            }

            return rows_affected > 0;
        }
    }
}
