using SafouaneAntoineService.DAL.IDAL;

namespace SafouaneAntoineService.Models
{
    public class ServiceCategory
    {
        private int id;
        private string name;
        private ServiceOffer offer;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public ServiceOffer Offer
        {
            get { return offer; }
            set { offer = value; }
        }
        public ServiceCategory(string name)
        {
            this.name = name;
        }

        public static List<ServiceCategory> GetCategories(IServiceCategoryDAL serviceCategoryDAL)
        {
            return serviceCategoryDAL.GetCategories();
        }
    }
}
