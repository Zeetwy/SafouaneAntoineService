using System.Data.SqlClient;
using SafouaneAntoineService.Models;
using SafouaneAntoineService.DAL.IDAL;
using System.Data;
using static SafouaneAntoineService.Models.ServiceRendered;

namespace SafouaneAntoineService.DAL
{
    public class ServiceRenderedDAL : IServiceRenderedDAL
    {
        private readonly string connection_string;

        public ServiceRenderedDAL(string connection_string)
        {
            this.connection_string = connection_string;
        }

        public bool ConfirmService(ServiceRendered service)
        {
            const string query = @"UPDATE [ServiceRendered]
    SET [Status] = @status, [Date] = @date, [NumberOfHours] = @numberOfHours
    WHERE [id] = @id";

            int rows_affected = 0;

            using (SqlConnection connection = new SqlConnection(this.connection_string))
            {
                SqlCommand cmd = new SqlCommand(query, connection);

                cmd.Parameters.AddWithValue("status", (int)ServiceRendered.Status.Completed);
                cmd.Parameters.AddWithValue("date", service.Date);
                cmd.Parameters.AddWithValue("numberOfHours", service.NumberOfHours);

                cmd.Parameters.AddWithValue("id", service.Id);

                connection.Open();
                rows_affected = cmd.ExecuteNonQuery();
            }
            return rows_affected > 0;
        }

        public bool ValidateService(ServiceRendered service)
        {
            const string query = "UPDATE [ServiceRendered] SET [Status] = @status WHERE [id] = @id";

            int rows_affected = 0;

            using (SqlConnection connection = new SqlConnection(this.connection_string))
            {
                SqlCommand cmd = new SqlCommand(query, connection);

                cmd.Parameters.AddWithValue("status", (int)ServiceRendered.Status.Archived);

                cmd.Parameters.AddWithValue("id", service.Id);

                connection.Open();
                rows_affected = cmd.ExecuteNonQuery();
            }
            return rows_affected > 0;
        }

        public List<ServiceRendered>? GetRequests(ServiceOffer offer)
        {
            const string query = @"SELECT [sr].[id], [sr].[customer_id], [u].[Firstname], [u].[Lastname]
    FROM [ServiceRendered] sr
	JOIN [User] u ON [sr].[customer_id] = [u].[id]
	WHERE [Status] = @status
	AND [serviceOffer_id] = @offer_id";

            List<ServiceRendered>? requests = new List<ServiceRendered>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    SqlCommand cmd = new SqlCommand(query, connection);

                    cmd.Parameters.AddWithValue("status", ServiceRendered.Status.Requested);
                    cmd.Parameters.AddWithValue("offer_id", offer.Id);

                    connection.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            requests.Add(
                                new ServiceRendered(
                                    reader.GetInt32("id"),
                                    ServiceRendered.Status.Requested,
                                    offer,
                                    offer.Provider,
                                    new User(
                                        reader.GetInt32("customer_id"),
                                        reader.GetString("Lastname"),
                                        reader.GetString("Firstname")
                                    )
                                )
                            );
                        }
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

            return requests;
        }


        //On doit récuperer une requete en particulier pour pouvoir la confirmer : 
        public ServiceRendered? GetRequest(int id)
        {
            const string query = @"SELECT [sr].[Date], [sr].[NumberOfHours], [sr].[Status],
	[so].[id] serviceoffer_id, [so].[type] serviceoffer_type, [so].[description] serviceoffer_description,
	[sr].[customer_id], [uc].[Firstname] customer_firstname, [uc].[Lastname] customer_lastname, [uc].[Timecredits] customer_timecredits,
	[so].[user_id] provider_id, [up].[Firstname] provider_firstname, [up].[Lastname] provider_lastname, [up].[Timecredits] provider_timecredits,
	[sc].[name] category_name
	FROM [ServiceRendered] sr
	JOIN [ServiceOffer] so ON [sr].[serviceOffer_id] = [so].[id]
	JOIN [User] uc ON [sr].[customer_id] = [uc].[Id]
	JOIN [User] up ON [so].[user_id] = [up].[Id]
	JOIN [ServiceCategory] sc ON [so].[category_id] = [sc].[Id]
	WHERE [sr].[id] = @rendered_id";

            ServiceRendered? servicerendered = null;

            using (SqlConnection connection = new SqlConnection(connection_string))
            {
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("rendered_id", id);

                connection.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        User provider = new User(
                            reader.GetInt32("provider_id"),
                            reader.GetString("provider_lastname"),
                            reader.GetString("provider_firstname"),
                            null,
                            reader.GetInt32("provider_timecredits")
                        );
                        servicerendered = new ServiceRendered(
                            id,
                            (Status)reader.GetInt32("Status"),
                            new ServiceOffer(
                                reader.GetInt32("serviceoffer_id"),
                                reader.GetString("serviceoffer_type"),
                                reader.IsDBNull("serviceoffer_description") ? null : reader.GetString("serviceoffer_description"),
                                new ServiceCategory(reader.GetString("category_name")),
                                provider
                            ),
                            provider,
                            new User(
                                reader.GetInt32("customer_id"),
                                reader.GetString("customer_lastname"),
                                reader.GetString("customer_firstname"),
                                null,
                                reader.GetInt32("customer_timecredits")
                            ),
                            reader.IsDBNull("NumberOfHours") ? 0 : reader.GetInt32("NumberOfHours"),
                            reader.IsDBNull("Date") ? null : reader.GetDateTime("Date")
                        );
                    }
                }
            }

            return servicerendered;
        }

        //Pour afficher les services rendus d'un user en particulier : 
        public List<ServiceRendered> GetServicesRenderedByUser(User user)
        {
            const string query = @"SELECT [sr].Id, [sr].[Status], [sr].[Date], [sr].[NumberOfHours]
    FROM [ServiceRendered] sr
    JOIN [User] u ON [sr].[customer_id] = [u].[Id]
    WHERE [sr].[customer_id] = @customer_id";

            List<ServiceRendered> servicesrendered = new List<ServiceRendered>();

            using (SqlConnection connection = new SqlConnection(this.connection_string))
            {
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@customer_id", user.Id);

                connection.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32("Id");
                        Status status = (Status)reader.GetInt32("Status");
                        DateTime? date = reader.IsDBNull("Date") ? null : reader.GetDateTime("Date");
                        int numberOfHours = reader.IsDBNull("NumberOfHours") ? 0 : reader.GetInt32("NumberOfHours");

                        ServiceRendered serviceRendered = new ServiceRendered(id, status, null, null, user, numberOfHours, date);
                        servicesrendered.Add(serviceRendered);
                    }
                }
            }
            return servicesrendered;
        }
    }
}
