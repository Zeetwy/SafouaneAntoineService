using SafouaneAntoineService.DAL.IDAL;
using System.ComponentModel.DataAnnotations;
namespace SafouaneAntoineService.Models
{
    public class ServiceOffer
    {
        private int id;
        private string type;
        private string? description;
        private User? provider;
        private ServiceCategory? category;
        private List<Review>? reviews;

        public int Id
        {
            get => id;
            private set => id = value;
        }

        [Required(ErrorMessage = "Empty Field!.")]
        public string Type
        {
            get => type;
            private set => type = value;
        }

        [Required(ErrorMessage = "Empty Field!.")]
        public string? Description
        {
            get => description;
            private set => description = value;
        }

        public User Provider
        {
            get => provider ?? throw new Exception("No provider was set.");
            private set => provider = value;
        }
        public ServiceCategory Category
        {
            get => category ?? throw new Exception("No category was set.");
            set => category = value;
        }

        public ServiceOffer(int id, string type, string? description, ServiceCategory? category = null, User? provider = null)
        {
            this.id = id;
            this.type = type;
            this.description = description;
            this.provider = provider;
            this.category = category;
        }

        public ServiceOffer(string type, string? description, ServiceCategory category, User user)
        {
            this.type = type;
            this.description = description;
            this.category = category;
            this.provider = user;
        }

        public bool SaveOffer(IServiceOfferDAL service_offer_dal)
        {
            return service_offer_dal.PublishOffer(this);
        }

        public static List<ServiceOffer> GetServices(IServiceOfferDAL serviceoffer)
        {
            List<ServiceOffer> serviceoffers = serviceoffer.GetServices();
            return serviceoffers;
        }

        public static ServiceOffer? GetDetails(IServiceOfferDAL serviceOfferDAL, int id)
        {
            return serviceOfferDAL.GetService(id);
        }

        public bool Request(User customer, IServiceOfferDAL service_offer_DAL, INotificationDAL notification_DAL)
        {
            bool ret = false;
            if (customer.Id != this.Provider.Id && !service_offer_DAL.ServiceWasRequested(this, customer))
            {
                ret = service_offer_DAL.RequestService(this, customer);

                string message = $"Le client {customer.Firstname} {customer.Lastname} a fait une demande pour votre service {Id}, voici son email {customer.Email}";
                Notification notif = new Notification(Provider, message);
                notif.Send(notification_DAL);
            }
            return ret;
        }

        public static ServiceOffer? GetOffer(int id, IServiceOfferDAL serviceOfferDAL)
        {
            return serviceOfferDAL.GetService(id);
        }

        public List<ServiceRendered>? GetRequests(IServiceRenderedDAL serviceRenderedDAL)
        {
            return serviceRenderedDAL.GetRequests(this);
        }

        public void AddReview(Review review)
        {
            if (reviews is null)
            {
                reviews = new List<Review>();
            }
            this.reviews.Add(review);
        }
    }
}
