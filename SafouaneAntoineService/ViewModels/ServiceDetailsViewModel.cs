using SafouaneAntoineService.Models;

namespace SafouaneAntoineService.ViewModels
{
    public class ServiceDetailsViewModel
    {
        public ServiceOffer? ServiceOffer { get; set; }
        public User? CurrentUser
        {
            get; set;
        }
    }
}
