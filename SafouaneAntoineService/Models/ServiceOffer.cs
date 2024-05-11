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
        private string description;
        private User provider;
        private ServiceCategory category;

        /* public int Id
         {
             get { return this.id; }
         } */

        //Mieux d'écrire comme ça : 

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        [Required(ErrorMessage = "Empty Field!.")]
        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        [Required(ErrorMessage = "Empty Field!.")]
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public User Provider
        {
            get { return provider; }
            set { provider = value; }
        }
        public ServiceCategory Category
        {
            get { return category; }
            set { category = value; }
        }

        public ServiceOffer()
        {

        }

        //Constructeur pour pouvoir afficher les détails d'un service : 
        public ServiceOffer(int id, string type, string description, User provider, ServiceCategory category)
        {
            this.id = id;
            this.type = type;
            this.description = description;
            this.provider = provider;
            this.category = category;
        }

        public ServiceOffer(int id, string type, string description)
        {
            this.id = id;
            this.type = type;
            this.description = description;
        }

        //Constructeur pour pouvoir publier une offre : 
        public ServiceOffer(int id, string type, string description, ServiceCategory category)
        {
            this.id = id;
            this.type = type;
            this.description = description;
            this.category = category;
        }

        public ServiceOffer(ServiceOfferViewModel so)
        {
            this.type = so.Type;
            this.description = so.Description;
            this.category = new ServiceCategory(so.CategoryName);
        }

        public static List<ServiceOffer> GetServices(IServiceOfferDAL serviceoffer)
        {
            List<ServiceOffer> serviceoffers = serviceoffer.GetServices();
            return serviceoffers;
        }

        public static ServiceOffer GetDetails(IServiceOfferDAL serviceOfferDAL, int id)
        {
            return serviceOfferDAL.GetService(id);
        }
    }
}
