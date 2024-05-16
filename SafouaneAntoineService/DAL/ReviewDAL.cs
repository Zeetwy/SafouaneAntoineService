using SafouaneAntoineService.DAL.IDAL;
using SafouaneAntoineService.Models;
using System.Data;
using System.Data.SqlClient;

namespace SafouaneAntoineService.DAL
{
    public class ReviewDAL : IReviewDAL
    {
        private readonly string connection_string;

        public ReviewDAL(string connection_string)
        {
            this.connection_string = connection_string;
        }
        public List<Review> GetReviewsByUser(User user)
        {
            List<Review> reviews = new List<Review>();
            string query = "SELECT * FROM [Review] " +
                "Join [ServiceOffer] ON [Review].service_id = [ServiceOffer].id " +
                "Join [User] ON [ServiceOffer].user_id = [User].Id " +
                "Where [User].Id = @idUser";
            using (SqlConnection connection = new SqlConnection(connection_string))
            {
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("idUser", user.Id);
                connection.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Review review = new Review(reader.GetInt32("id"), reader.GetInt32("rating"), reader.GetString("comment"));
                        reviews.Add(review);
                    }
                }
            }
            return reviews;
        }

        public bool SaveReview(Review review)
        {
            bool success = false;
            string query = "INSERT INTO [Review] (rating, comment, service_id, user_id) VALUES(@rating,@comment,@service_id,@user_id)";

            using (SqlConnection connection = new SqlConnection(connection_string))
            {
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("rating", review.Rating);
                cmd.Parameters.AddWithValue("comment", review.Comment);
                cmd.Parameters.AddWithValue("service_id", review.Service.Id);
                cmd.Parameters.AddWithValue("user_id", review.Customer.Id);
                connection.Open();
                success = cmd.ExecuteNonQuery() > 0;

                return success;
            }
        }
    }
}
