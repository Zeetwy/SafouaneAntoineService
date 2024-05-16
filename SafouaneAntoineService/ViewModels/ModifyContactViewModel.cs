using System.ComponentModel.DataAnnotations;

namespace SafouaneAntoineService.ViewModels
{
    public class ModifyContactViewModel
    {
        [DataType(DataType.EmailAddress), Required(ErrorMessage = "Email Invalid!")]
        public string Email
        {
            get;
            set;
        }
    }
}
