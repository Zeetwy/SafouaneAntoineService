using System.Data.SqlClient;
using SafouaneAntoineService.Models;
using SafouaneAntoineService.DAL.IDAL;

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
    }
}
