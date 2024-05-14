using System.Data.SqlClient;
using SafouaneAntoineService.Models;
using SafouaneAntoineService.DAL.IDAL;
using System.Data;

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
            const string query =
        @"UPDATE [ServiceRendered]
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
            const string query =
        @"SELECT [sr].[id], [sr].[customer_id], [u].[Firstname], [u].[Lastname]
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
    }
}
