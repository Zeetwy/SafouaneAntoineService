using System.ComponentModel.DataAnnotations;

namespace SafouaneAntoineService.ViewModels
{
    public class ServiceOfferViewModel
    {
        private string type;
        private string? description;
        private int category_id;

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
        public int CategoryId
        {
            get => this.category_id;
            set => this.category_id = value;
        }
    }
}
