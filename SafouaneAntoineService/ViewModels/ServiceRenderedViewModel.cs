using System.ComponentModel.DataAnnotations;

namespace SafouaneAntoineService.ViewModels
{
    public class ServiceRenderedViewModel
    {
        private int id;
        private DateTime? date;
        private int numberofhours;

        public int Id
        {
            get; set;
        }

        [Required(ErrorMessage = "Empty Field!."), DataType(DataType.DateTime)]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Empty Field!.")]
        public int NumberOfHours
        {
            get; set;
        }

        public ServiceRenderedViewModel()
        {
        }

        public ServiceRenderedViewModel(int id, DateTime date, int numberOfHours)
        {
            this.id = id;
            this.Date = date;
            this.NumberOfHours = numberOfHours;
        }

    }
}
