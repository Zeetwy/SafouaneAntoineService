using SafouaneAntoineService.Models;

namespace SafouaneAntoineService.ViewModels
{
    public class ServiceRenderedDetailsViewModel
    {
        public ServiceRendered? ServiceRendered { get; set; }
        public User? CurrentUser
        {
            get; set;
        }
    }
}
