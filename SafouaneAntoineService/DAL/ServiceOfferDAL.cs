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

        public bool PublishOffer(User user, ServiceOfferViewModel so)
        {
            const string query = "INSERT INTO [ServiceOffer] ([type], [description], [user_id], [category_id]) VALUES (@type, @description, @user_id, @category_id)";

            int rows_affected = 0;

            using (SqlConnection connection = new SqlConnection(this.connection_string))
            {
                SqlCommand cmd = new SqlCommand(query, connection);

                cmd.Parameters.AddWithValue("type", so.Type);
                cmd.Parameters.AddWithValue("description", so.Description);
                cmd.Parameters.AddWithValue("user_id", user.Id);
                cmd.Parameters.AddWithValue("category_id", so.CategoryId);

                connection.Open();
                rows_affected = cmd.ExecuteNonQuery();
            }

            return rows_affected > 0;
        }
    }
}
