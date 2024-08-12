using SafouaneAntoineService.Models;

namespace SafouaneAntoineService.DAL.IDAL
{
    public interface IUserDAL
    {
        public User? Authenticate(string username, string password);
        public bool SaveAccount(User user);
        // public bool Credit(User u, int amount);
        public bool Credit(User u);
        // public bool Debit(User u, int amount);
        public bool Debit(User u);
        public void RefreshInfo(ref User user);
        public bool ChangeContact(User user, string email);
    }
}
