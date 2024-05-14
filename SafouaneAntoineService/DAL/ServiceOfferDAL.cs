using System.Data;
using System.Data.SqlClient;
using SafouaneAntoineService.DAL.IDAL;
using SafouaneAntoineService.Models;

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

                            ServiceOffer serviceOffer = new ServiceOffer(
                                reader.GetInt32("id"),
                                reader.GetString("type"),
                                reader.IsDBNull("description") ? null : reader.GetString("description")
                            );
                            serviceOffers.Add(serviceOffer);
                        }
                    }
                }

                return serviceOffers; // Une fois toutes les offres de service récupérées et ajoutées à la liste serviceOffers, la liste est renvoyée comme résultat de la méthode.
            }
            catch (Exception)
            {
                return new List<ServiceOffer>(); // return an empty list in case of an error
            }
        }

        //Pour afficher un service en particulier
        public ServiceOffer? GetService(int id)
        {
            ////jointure pour récupérer les informations de l'utilisateur fournisseur et de la catégorie de service correspondante.
            const string query =
        @"SELECT [so].[id], [so].[type], [so].[description], [so].[category_id], [so].[user_id], [sc].[name], [u].[Firstname], [u].[Lastname], [u].[Email]
            FROM [ServiceOffer] so
            JOIN [User] u ON [so].user_id = [u].[Id]
            JOIN [ServiceCategory] sc ON [so].[category_id] = [sc].[id]
            WHERE [so].[id] = @id";

            ServiceOffer? serviceOffer = null; //C'est cette variable qui sera renvoyée à la fin de la méthode avec les détails de l'offre de service récupérée.

            using (SqlConnection connection = new SqlConnection(connection_string))
            {
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("id", id); //on ajoute un paramètre nommé id avec la valeur de l'ID du service qu'on souhaite récupérer

                connection.Open();

                using (SqlDataReader reader = cmd.ExecuteReader()) //on lit la requete
                {
                    if (reader.Read())
                    {
                        serviceOffer = new ServiceOffer(
                            reader.GetInt32("id"),
                            reader.GetString("type"),
                            reader.IsDBNull("description") ? null : reader.GetString("description"),
                            new ServiceCategory(reader.GetString("name")),
                            new User(
                                reader.GetInt32("user_id"),
                                reader.GetString("Lastname"),
                                reader.GetString("Firstname"),
                                null,
                                0,
                                reader.GetString("Email")
                            )
                        );
                    }
                }
            }

            return serviceOffer;
        }



        public List<ServiceOffer> GetOffersByUser(User user)
        {
            const string query =
        @"SELECT [so].[id], [so].[type], [so].[description], [sc].[name]
            FROM [ServiceOffer] so
            JOIN [ServiceCategory] sc ON [sc].[id] = [so].[category_id]
            WHERE [user_id]=@user_id";

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
                                reader.IsDBNull("description") ? null : reader.GetString("description"),
                                new ServiceCategory(reader.GetString("name")),
                                user
                            )
                        );
                    }
                }
            }
            return ret;
        }

        public bool PublishOffer(ServiceOffer so)
        {
            const string query =
        @"INSERT INTO [ServiceOffer] ([type], [description], [user_id], [category_id])
            VALUES (@type, @description, @user_id, (
                SELECT [id]
                    FROM [ServiceCategory]
                    WHERE [name]=@category_name
                )
            )";

            int rows_affected = 0;

            using (SqlConnection connection = new SqlConnection(this.connection_string))
            {
                SqlCommand cmd = new SqlCommand(query, connection);

                cmd.Parameters.AddWithValue("type", so.Type);
                if (so.Description != null)
                {
                    cmd.Parameters.AddWithValue("description", so.Description);
                }
                else
                {
                    cmd.Parameters.AddWithValue("description", DBNull.Value);
                }
                cmd.Parameters.AddWithValue("user_id", so.Provider.Id);
                cmd.Parameters.AddWithValue("category_name", so.Category.Name);

                connection.Open();
                rows_affected = cmd.ExecuteNonQuery();
            }

            return rows_affected > 0;
        }

        public bool RequestService(ServiceOffer offer, User customer)
        {
            const string query =
        $@"INSERT INTO [ServiceRendered] ([Status], [Date], [NumberOfHours], [serviceOffer_id], [customer_id])
            VALUES (@status, NULL, NULL, @serviceOfferId, @customerId)";

            int rows_affected = 0;

            using (SqlConnection connection = new SqlConnection(this.connection_string))
            {
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("status", (int)ServiceRendered.Status.Requested);
                cmd.Parameters.AddWithValue("serviceOfferId", offer.Id);
                cmd.Parameters.AddWithValue("customerId", customer.Id);

                connection.Open();
                rows_affected = cmd.ExecuteNonQuery();
            }
            return rows_affected > 0;
        }

        public bool ServiceWasRequested(ServiceOffer offer, User customer)
        {
            const string query =
        @"SELECT COUNT([id]) AS ""RequestedCount"" FROM [ServiceRendered]
            WHERE [Status] = @status
            AND [customer_id] = @customerId
            AND [serviceOffer_id] = @serviceOfferId";

            bool was_requested = false;

            using (SqlConnection connection = new SqlConnection(this.connection_string))
            {
                SqlCommand cmd = new SqlCommand(query, connection);

                cmd.Parameters.AddWithValue("status", (int)ServiceRendered.Status.Requested);
                cmd.Parameters.AddWithValue("serviceOfferId", offer.Id);
                cmd.Parameters.AddWithValue("customerId", customer.Id);

                connection.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        was_requested = reader.GetInt32("RequestedCount") > 0;
                    }
                }
            }
            return was_requested;
        }
    }
}
