using SafouaneAntoineService.Models;

namespace SafouaneAntoineService.DAL.IDAL
{
    public interface IReviewDAL
    {
        public bool SaveReview(Review review);
        public List<Review> GetReviewsByUser(User user);
    }
}
