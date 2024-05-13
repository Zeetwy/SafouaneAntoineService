using System.Data;
using System.Data.SqlClient;
using SafouaneAntoineService.DAL.IDAL;
using SafouaneAntoineService.Models;

namespace SafouaneAntoineService.DAL
{
    public class ServiceCategoryDAL : IServiceCategoryDAL
    {
        private readonly string connection_string;
        public ServiceCategoryDAL(string connection_string)
        {
            this.connection_string = connection_string;
        }

        public List<ServiceCategory> GetCategories()
        {
            const string query = "SELECT [name] FROM [ServiceCategory]";

            List<ServiceCategory> ret = new List<ServiceCategory>();

            using (SqlConnection connection = new SqlConnection(this.connection_string))
            {
                SqlCommand cmd = new SqlCommand(query, connection);
                connection.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ret.Add(new ServiceCategory(reader.GetString("name")));
                    }
                }
            }
            return ret;
        }
    }
}
