using System.ComponentModel.DataAnnotations;

namespace SafouaneAntoineService.ViewModels
{
    public class UserViewModel
    {
        // ViewModel pour create account
        private string lastname;
        private string firstname;
        private string username;
        private int timecredits = 10;
        private string email;
        private string password;

       
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

        //public int Timecredits { get; set; } = 10;

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
    }
}
