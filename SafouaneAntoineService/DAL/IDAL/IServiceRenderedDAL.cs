using SafouaneAntoineService.Models;

namespace SafouaneAntoineService.DAL.IDAL
{
    public interface IServiceRenderedDAL
    {
        public bool ConfirmService(ServiceRendered service);
        public bool ValidateService(ServiceRendered service);
        public List<ServiceRendered>? GetRequests(ServiceOffer offer);
        ServiceRendered? GetRequest(int id);
        public List<ServiceRendered> GetServicesRenderedByUser(User user);
        public ServiceRendered? GetServiceRendered(int id);
    }
}
