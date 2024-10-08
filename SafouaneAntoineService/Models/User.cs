﻿using SafouaneAntoineService.DAL.IDAL;
using SafouaneAntoineService.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SafouaneAntoineService.Models
{
    public class User
    {
        private int id;
        private string lastname;
        private string firstname;
        private string username;
        private int timecredits = 10;
        private string email;
        private string password;

        private List<ServiceOffer>? offers;
        private List<ServiceRendered>? renders;

        private List<Notification>? notifications;

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
            set { timecredits = value; }
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

        [JsonConstructor]
        public User() { } // Needed to deserialize JSON from session

        public User(int id, string lastname, string firstname, string? username = null, int timecredits = 0, string? email = null, string? password = null)
        {
            this.id = id;
            this.lastname = lastname;
            this.firstname = firstname;
            this.username = username ?? string.Empty;
            this.timecredits = timecredits;
            this.email = email ?? string.Empty;
            this.password = password ?? string.Empty;
        }

        public User(UserViewModel userVm)
        {
            lastname = userVm.Lastname;
            firstname = userVm.Firstname;
            username = userVm.Username;
            timecredits = userVm.Timecredits;
            email = userVm.Email;
            password = userVm.Password;
        }

        public static User? Authenticate(string username, string password, IUserDAL userDAL)
        {
            return userDAL.Authenticate(username, password);
        }
        public bool SaveAccount(IUserDAL userDAL)
        {
            return userDAL.SaveAccount(this);
        }

        public List<ServiceOffer> GetOffers(IServiceOfferDAL service_offer_DAL)
        {
            if (this.offers is null)
            {
                this.offers = service_offer_DAL.GetOffersByUser(this);
            }
            return this.offers;
        }

        public List<ServiceRendered> GetServicesRenderedByUser(IServiceRenderedDAL service_rendered_DAL)
        {
            if (this.renders is null)
            {
                this.renders = service_rendered_DAL.GetServicesRenderedByUser(this);
            }
            return this.renders;
        }

        public List<Notification> GetNotifications(INotificationDAL notificationDAL)
        {
            if (this.notifications is null)
            {
                this.notifications = notificationDAL.GetNotifications(this);
            }
            return this.notifications;
        }

        public bool Publish(ServiceOffer so, IServiceOfferDAL serviceOfferDAL)
        {
            this.GetOffers(serviceOfferDAL).Add(so);
            return so.SaveOffer(serviceOfferDAL);
        }



        public bool Debit(int amount, IUserDAL userDAL)
        {
            this.timecredits -= amount; 
            return userDAL.Debit(this);
        }

  
        public bool Credit(int amount, IUserDAL userDAL)
        {
            this.timecredits += amount;
            return userDAL.Credit(this);
        }

        public bool DeleteOffer(ServiceOffer offer, IServiceOfferDAL serviceOfferDAL)
        {
            this.GetOffers(serviceOfferDAL).Find(so => so.Id == offer.Id);
            return serviceOfferDAL.DeleteOffer(offer);
        }

        public bool ChangeContact(string email, IUserDAL userDAL)
        {
            return userDAL.ChangeContact(this, email);
        }
    }
}
