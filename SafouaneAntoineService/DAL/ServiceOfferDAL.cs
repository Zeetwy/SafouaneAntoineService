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

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ServiceOffer serviceOffer = new ServiceOffer(
                                reader.GetInt32("id"),
                                reader.GetString("type"),
                                reader.IsDBNull("description") ? null : reader.GetString("description")
                            );
                            serviceOffers.Add(serviceOffer);
                        }
                    }
                }

                return serviceOffers;
            }
            catch (Exception)
            {
                return new List<ServiceOffer>(); //retourne une liste vide en cas d'erreur
            }
        }

        //Pour afficher un service en particulier
        public ServiceOffer? GetService(int id)
        {
            const string query = @"SELECT [so].[id], [so].[type], [so].[description], [so].[category_id], [so].[user_id], [sc].[name], [u].[Firstname], [u].[Lastname], [u].[Email]
    FROM [ServiceOffer] so
    JOIN [User] u ON [so].user_id = [u].[Id]
    JOIN [ServiceCategory] sc ON [so].[category_id] = [sc].[id]
    WHERE [so].[id] = @id";

            ServiceOffer? serviceOffer = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("id", id);

                    connection.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
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
            }
            catch (SqlException) { return null; }

            return serviceOffer;
        }

        public List<ServiceOffer> GetOffersByUser(User user)
        {
            const string query = @"SELECT [so].[id], [so].[type], [so].[description], [sc].[name]
    FROM [ServiceOffer] so
    JOIN [ServiceCategory] sc ON [sc].[id] = [so].[category_id]
    WHERE [user_id]=@user_id";

            List<ServiceOffer> ret = new List<ServiceOffer>();
            try
            {
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
            }
            catch (SqlException) { }
            return ret;
        }

        public bool PublishOffer(ServiceOffer so)
        {
            const string query = @"INSERT INTO [ServiceOffer] ([type], [description], [user_id], [category_id])
    VALUES (@type, @description, @user_id, (
        SELECT [id]
            FROM [ServiceCategory]
            WHERE [name]=@category_name
        )
    )";

            int rows_affected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(this.connection_string))
                {
                    SqlCommand cmd = new SqlCommand(query, connection);

                    cmd.Parameters.AddWithValue("type", so.Type);
                    if (so.Description is not null)
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
            }
            catch (SqlException) { return false; }

            return rows_affected > 0;
        }

        public bool RequestService(ServiceOffer offer, User customer)
        {
            const string query = @"INSERT INTO [ServiceRendered] ([Status], [Date], [NumberOfHours], [serviceOffer_id], [customer_id])
    VALUES (@status, NULL, NULL, @serviceOfferId, @customerId)";

            int rows_affected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(this.connection_string))
                {
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("status", (int)ServiceRendered.Status.Requested);
                    cmd.Parameters.AddWithValue("serviceOfferId", offer.Id);
                    cmd.Parameters.AddWithValue("customerId", customer.Id);

                    connection.Open();
                    rows_affected = cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException) { return false; }
            return rows_affected > 0;
        }

        public bool ServiceWasRequested(ServiceOffer offer, User customer)
        {
            const string query = @"SELECT COUNT([id]) AS ""RequestedCount"" FROM [ServiceRendered]
    WHERE [Status] = @status
    AND [customer_id] = @customerId
    AND [serviceOffer_id] = @serviceOfferId";

            bool was_requested = false;
            try
            {
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
            }
            catch (SqlException) { return false; }
            return was_requested;
        }

		public bool DeleteOffer(ServiceOffer offer)
        {
            const string query = "";
            return false; // Unimplemented
        }
	}
}
