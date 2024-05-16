using SafouaneAntoineService.DAL.IDAL;
using SafouaneAntoineService.ViewModels;

namespace SafouaneAntoineService.Models
{
    public class ServiceRendered
    {
        public enum Status
        {
            Requested = 0,
            Completed = 1,
            Archived = 2
        }

        private readonly int id;
        private Status servicestatus;
        private DateTime? date;
        private int numberofhours;
        private readonly ServiceOffer serviceoffer;
        private readonly User provider;
        private readonly User customer;

        public int Id { get => id; }
        public Status Servicestatus { get => servicestatus; }

        public DateTime Date
        {
            get
            {
                if (date == null || servicestatus == Status.Requested)
                {
                    throw new Exception("No date was set.");
                }
                if (date >= DateTime.Now)
                {
                    throw new Exception("The date cannot be in the future.");
                }

                return date.Value; 
            }
            set { date = value; }
        }




        public int NumberOfHours
        {
            get
            {
                if (servicestatus == Status.Requested)
                {
                    throw new Exception("No time was set.");
                }
                return numberofhours;
            }
        }

        public ServiceOffer ServiceOffer { get => serviceoffer; }
        public User Provider { get => provider; }
        public User Customer { get => customer; }

        public ServiceRendered(int id, Status status, ServiceOffer serviceoffer, User provider, User customer, int numberofhours = 0, DateTime? date = null)
        {
            this.id = id;
            this.servicestatus = status;
            this.serviceoffer = serviceoffer;
            this.provider = provider;
            this.customer = customer;
            this.numberofhours = numberofhours;
            this.date = date;
        }

        public bool Confirm(int hours, DateTime date, IServiceRenderedDAL service_rendered_DAL)
        {
            Status prev_status = this.servicestatus;
            if (servicestatus == Status.Requested)
            {
                this.date = date;
                this.numberofhours = hours;
                this.servicestatus = Status.Completed;
                if (service_rendered_DAL.ConfirmService(this))
                {
                    return true;
                }
            }
            this.servicestatus = prev_status;
            return false;
        }

        public bool Validate(IServiceRenderedDAL service_rendered_DAL, IUserDAL userDAL)
        {
            if (servicestatus == Status.Completed && service_rendered_DAL.ValidateService(this))
            {
                int amountToDebit = numberofhours;

                // Débiter le fournisseur
                this.provider.Debit(amountToDebit, userDAL);

                // Créditer le client
                this.customer.Credit(amountToDebit, userDAL);

                this.servicestatus = Status.Archived;
                return true;
            }
            return false;
        }

        public static ServiceRendered? GetServiceById(IServiceRenderedDAL serviceRenderedDAL, int id)
        {
            return serviceRenderedDAL.GetServiceRendered(id);
        }





    }
}
