using Microsoft.AspNetCore.Mvc;
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

        public ServiceOfferController(IServiceOfferDAL _serviceOffer, IServiceCategoryDAL _serviceCategory, INotificationDAL _notification)
        {
            this._serviceOffer = _serviceOffer;
            this._serviceCategory = _serviceCategory;
            this._notification = _notification;
        }

        [HttpGet]
        public IActionResult MakeARequest(int id)
        {
            User? customer = ControllerHelper.GetUserLoggedIn(this);
            if (customer is null) { return ControllerHelper.NeedToBeLoggedIn(this); }

            // Créer une instance de ServiceOffer (assurez-vous d'injecter IServiceOfferDAL dans votre contrôleur)
            ServiceOffer? serviceOffer = this._serviceOffer.GetService(id);

            // Appeler la méthode Request sur l'instance de ServiceOffer
            return View("Request", serviceOffer is not null && serviceOffer.Request(customer, this._serviceOffer, this._notification));
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
            if (currentUser == null)
            {
                return ControllerHelper.NeedToBeLoggedIn(this);
            }

            ServiceOffer? serviceOffer = this._serviceOffer.GetService(id);

            // return View(new { ServiceOffer = serviceOffer, CurrentUser = currentUser });
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
            return View(user.GetOffers(this._serviceOffer));
        }

        public IActionResult PublishOffer()
        {
            ViewBag.Categories = this._serviceCategory.GetCategories();
            if (ControllerHelper.GetUserLoggedIn(this) is null) { return ControllerHelper.NeedToBeLoggedIn(this); }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PublishOffer(ServiceOfferViewModel so)
        {
            User? user = ControllerHelper.GetUserLoggedIn(this);
            if (user is null) { return ControllerHelper.NeedToBeLoggedIn(this); }

            ServiceOffer offer = new ServiceOffer(so, user);
            if (ModelState.IsValid && offer.Publish(this._serviceOffer))
            {
                TempData["Message"] = "Offer published successfully.";
                return RedirectToAction("ManageOffers", "ServiceOffer");
            }
            return RedirectToAction("PublishOffer", "ServiceOffer");
        }

        [HttpGet]
        public IActionResult DeleteOffer(int id)
        {
            return View(); // TODO
        }
    }
}
