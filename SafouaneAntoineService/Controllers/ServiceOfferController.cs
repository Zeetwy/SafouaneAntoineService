using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SafouaneAntoineService.DAL.IDAL;
using SafouaneAntoineService.Models;
using SafouaneAntoineService.ViewModels;

namespace SafouaneAntoineService.Controllers
{
    public class ServiceOfferController : Controller
    {
        private readonly IServiceOfferDAL _serviceOffer;
        private readonly IServiceCategoryDAL _serviceCategory;
        private readonly INotificationDAL _notification;
        private readonly IUserDAL _user;

        public ServiceOfferController(IServiceOfferDAL _serviceOffer, IServiceCategoryDAL _serviceCategory, INotificationDAL _notification, IUserDAL user)
        {
            this._serviceOffer = _serviceOffer;
            this._serviceCategory = _serviceCategory;
            this._notification = _notification;
            this._user = user;
        }

        [HttpGet]
        public IActionResult MakeARequest(int id)
        {
            User? customer = ControllerHelper.GetUserLoggedIn(this);
            if (customer is null) { return ControllerHelper.NeedToBeLoggedIn(this); }

            // Créer une instance de ServiceOffer (assurez-vous d'injecter IServiceOfferDAL dans votre contrôleur)
            ServiceOffer? serviceOffer = ServiceOffer.GetOffer(id, _serviceOffer);

            _user.RefreshInfo(ref customer);
            HttpContext.Session.SetString("User", JsonConvert.SerializeObject(customer));

            // Appeler la méthode Request sur l'instance de ServiceOffer
            return View("Request", serviceOffer is not null && customer.Timecredits > 0 && serviceOffer.Request(customer, this._serviceOffer, this._notification));
        }

        public IActionResult ViewServices()
        {
            User? user = ControllerHelper.GetUserLoggedIn(this);
            if (user is null) { return ControllerHelper.NeedToBeLoggedIn(this); }

            ViewData["user"] = user.Username;

            return View(ServiceOffer.GetServices(_serviceOffer));
           
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            User? currentUser = ControllerHelper.GetUserLoggedIn(this);
            if (currentUser is null)
            {
                return ControllerHelper.NeedToBeLoggedIn(this);
            }

            ServiceOffer? serviceOffer = ServiceOffer.GetOffer(id, _serviceOffer);

            ServiceDetailsViewModel svm = new ServiceDetailsViewModel
            {
                ServiceOffer = serviceOffer,
                CurrentUser = currentUser
            };

            return View(svm);
        }
       
        public IActionResult ManageOffers()
        {
            User? user = ControllerHelper.GetUserLoggedIn(this);
            if (user is null) { return ControllerHelper.NeedToBeLoggedIn(this); }
            return View(user.GetOffers(_serviceOffer));
        }

        public IActionResult PublishOffer()
        {
            if (ControllerHelper.GetUserLoggedIn(this) is null) { return ControllerHelper.NeedToBeLoggedIn(this); }
			ViewBag.Categories = ServiceCategory.GetCategories(_serviceCategory);
			return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PublishOffer(ServiceOfferViewModel so)
        {
            User? user = ControllerHelper.GetUserLoggedIn(this);
            if (user is null) { return ControllerHelper.NeedToBeLoggedIn(this); }

            if (ModelState.IsValid)
            {
				ServiceOffer offer = new ServiceOffer(so.Type, so.Description, new ServiceCategory(so.CategoryName), user);
                if (user.Publish(offer, _serviceOffer))
                {
                    TempData["Message"] = "Offer published successfully.";
                    return RedirectToAction("ManageOffers", "ServiceOffer");
                }
            }
            return View(so);
        }

        [HttpGet]
        public IActionResult DeleteOffer(int id)
        {
            User? user = ControllerHelper.GetUserLoggedIn(this);
            if (user is null) { return ControllerHelper.NeedToBeLoggedIn(this); }
            ServiceOffer? offer = ServiceOffer.GetOffer(id, _serviceOffer);

            return View(offer is not null && user.DeleteOffer(offer, _serviceOffer)); // Unimplemented
        }
    }
}
