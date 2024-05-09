using System.Data;
using System.Data.SqlClient;
using SafouaneAntoineService.DAL.IDAL;

namespace SafouaneAntoineService.DAL
{
    public class ServiceCategoryDAL : IServiceCategoryDAL
    {
        private readonly string connection_string;
        public ServiceCategoryDAL(string connection_string)
        {
            this.connection_string = connection_string;
        }

        public List<string> GetCategories()
        {
            const string query = "SELECT [name] FROM [ServiceCategory]";

            List<string> ret = new List<string>();

            using (SqlConnection connection = new SqlConnection(this.connection_string))
            {
                SqlCommand cmd = new SqlCommand(query, connection);
                connection.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ret.Add(reader.GetString("name"));
                    }
                }
            }
            return ret;
        }
    }
}
