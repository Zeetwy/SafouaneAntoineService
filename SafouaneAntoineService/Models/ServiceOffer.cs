using SafouaneAntoineService.DAL;
using SafouaneAntoineService.DAL.IDAL;
using SafouaneAntoineService.ViewModels;
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

        //Constructeur pour pouvoir afficher les détails d'un service : 
        public ServiceOffer(int id, string type, string? description, ServiceCategory? category = null, User? provider = null)
        {
            this.id = id;
            this.type = type;
            this.description = description;
            this.provider = provider;
            this.category = category;
        }

        public ServiceOffer(ServiceOfferViewModel so, User user)
        {
            this.type = so.Type;
            this.description = so.Description;
            this.category = new ServiceCategory(so.CategoryName);
            this.provider = user;
        }

        public bool Publish(IServiceOfferDAL service_offer_dal)
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

        public void MakeRequest(User customer, INotificationDAL notification_DAL)
        {
            if (customer.Id != this.Provider.Id)
            {
                // Créer le message à envoyer dans la notification
                string message = $"Le client {customer.Firstname} {customer.Lastname} a fait une demande pour votre service.";
                Notification notif = new Notification(Provider, message);
                notif.Send(notification_DAL);
            }
        }
    }
}
