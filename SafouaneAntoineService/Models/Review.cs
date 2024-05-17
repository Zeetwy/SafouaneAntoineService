using SafouaneAntoineService.DAL.IDAL;
using System.ComponentModel.DataAnnotations;

namespace SafouaneAntoineService.Models
{
    public class Review
    {
        private int id;
        private int rating;
        private string comment;
        private User customer;
        private ServiceOffer service;
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        [Required(ErrorMessage = "Rating invalid!"), Range(1, 5, ErrorMessage = "Enter a rating between 1 and 5 !")]
        public int Rating
        {
            get { return rating; }
            set { rating = value; }
        }
        [Required(ErrorMessage = "Enter a comment."), StringLength(100, MinimumLength = 1, ErrorMessage = "Enter a comment between 1 and 100 characteres")]
        public string Comment
        {
            get { return comment; }
            set { comment = value; }
        }
        public User Customer
        {
            get { return customer; }
            set { customer = value; }
        }
        public ServiceOffer Service
        {
            get { return service; }
            set { service = value; }
        }
        public Review(int rating, string comment, User user, ServiceOffer service)
        {
            this.rating = rating;
            this.comment = comment;
            this.customer = user;
            this.service = service;
            this.service.AddReview(this);
        }
        public Review(int id, int rating, string comment)
        {
            this.id = id;
            this.rating = rating;
            this.comment = comment;
        }
        public static List<Review> GetReviewByUser(IReviewDAL review_DAL, User user)
        {
            return review_DAL.GetReviewsByUser(user);
        }
        public bool SaveReview(IReviewDAL review_DAL)
        {
            return review_DAL.SaveReview(this);
        } 
    }
}
