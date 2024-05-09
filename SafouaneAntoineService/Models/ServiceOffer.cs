using SafouaneAntoineService.DAL.IDAL;
using SafouaneAntoineService.ViewModels;
namespace SafouaneAntoineService.Models
{
    public class ServiceOffer
    {
        private int id;
        private string type;
        private string description;
        private ServiceCategory category;

        public int Id
        {
            get { return this.id; }
        }
        public string Type
        {
            get { return this.type; }
        }

        public string Description
        {
            get { return this.description; }
        }

        public ServiceCategory Category
        {
            get { return this.category; }
        }

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
    }
}
