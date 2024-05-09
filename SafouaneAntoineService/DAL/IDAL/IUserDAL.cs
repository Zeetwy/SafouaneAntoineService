using SafouaneAntoineService.Models;

namespace SafouaneAntoineService.DAL.IDAL
{
    public interface IUserDAL
    {
        public User Authenticate(string username, string password);
        public bool SaveAccount(User user);
    }
}
