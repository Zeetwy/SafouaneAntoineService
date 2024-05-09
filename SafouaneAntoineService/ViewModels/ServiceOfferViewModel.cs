using System.ComponentModel.DataAnnotations;

namespace SafouaneAntoineService.ViewModels
{
    public class ServiceOfferViewModel
    {
        private string type;
        private string? description;
        private string category_name;

        [Required(ErrorMessage = "Please enter the type of service."), StringLength(50, ErrorMessage = "Type length cannot exceed 50 characters.")]
        public string Type
        {
            get => this.type;
            set => this.type = value;
        }

        public string? Description
        {
            get => this.description;
            set => this.description = value;
        }

        [Required(ErrorMessage = "Please select a category for the offer")]
        public string CategoryName
        {
            get => this.category_name;
            set => this.category_name = value;
        }
    }
}
