using SafouaneAntoineService.DAL;
using SafouaneAntoineService.DAL.IDAL;
using SafouaneAntoineService.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace SafouaneAntoineService.Models
{
    public class User
    {
        private int id;
        private string lastname;
        private string firstname;
        private string username;
        public int timecredits = 10;
        private string email;
        private string password;

        private List<ServiceOffer>? offers;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        [Required(ErrorMessage = "Lastname Invalid."), StringLength(20, MinimumLength = 3, ErrorMessage = " Enter between 3 and 20 characters")]
        public string Lastname
        {
            get { return lastname; }
            set { lastname = value; }
        }

        [Required(ErrorMessage = "Firstname Invalid."), StringLength(20, MinimumLength = 3, ErrorMessage = " Enter between 3 and 20 characters")]
        public string Firstname
        {
            get { return firstname; }
            set { firstname = value; }
        }

        [Required(ErrorMessage = "Username Invalid."), StringLength(15, MinimumLength = 3, ErrorMessage = " Enter between 3 and 15 characters")]
        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        public int Timecredits
        {
            get { return timecredits; }
            set { timecredits = 10; }
        }

        [DataType(DataType.EmailAddress), Required(ErrorMessage = "Email Invalid!")]
        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password, ErrorMessage = "Invalid password.")]
        [RegularExpression(@"^(?=.{10,}$)(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*\W).*$", ErrorMessage = "Password must contain at least 1 uppercase, 1 lowercase, 1 digit, 1 special character, and be at least 10 characters long.")]
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public User()
        {
        }

        public User(int id, string lastname, string firstname, string username, int timecredits, string email, string password)
        {
            this.id = id;
            this.lastname = lastname;
            this.firstname = firstname;
            this.username = username;
            this.timecredits = timecredits;
            this.email = email;
            this.password = password;
        }

        // constructeur pour mon UserViewModel
        public User(UserViewModel userVm)
        {
            lastname = userVm.Lastname;
            firstname = userVm.Firstname;
            username = userVm.Username;
            timecredits = userVm.Timecredits;
            email = userVm.Email;
            password = userVm.Password;
        }

        //ces méthodes délèguent la logique métier à une implémentation spécifique de l'interface IUserDAL, qui est injectée en tant que dépendance.
        public static User Authenticate(string username, string password, IUserDAL userDAL)
        {
            return userDAL.Authenticate(username, password);
        }
        public bool SaveAccount(IUserDAL userDAL)
        {
            return userDAL.SaveAccount(this);
        }

        public List<ServiceOffer> GetOffers(IServiceOfferDAL service_offer_DAL)
        {
            if (this.offers != null)
            {
                return this.offers;
            }
            this.offers = service_offer_DAL.GetOffersByUser(this);
            return this.offers;
        }

        public bool PublishOffer(ServiceOffer so, IServiceOfferDAL service_offer_dal)
        {
            return service_offer_dal.PublishOffer(this, so);
        }

    }
}
