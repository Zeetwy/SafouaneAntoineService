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

        public ServiceOfferController(IServiceOfferDAL _serviceOffer, IServiceCategoryDAL _serviceCategory, INotificationDAL _notification)
        {
            this._serviceOffer = _serviceOffer;
            this._serviceCategory = _serviceCategory;
            this._notification = _notification;
        }

        private User? GetUserLoggedIn()
        {
            string? user_session_string = HttpContext.Session.GetString("User");
            if (string.IsNullOrEmpty(user_session_string))
            {
                return null;
            }
            return JsonConvert.DeserializeObject<User>(user_session_string);
        }

        private IActionResult NeedToBeLoggedIn()
        {
            TempData["Message"] = "You need to be logged in to view this page.";
            return RedirectToAction("Authenticate", "User");
        }

        [HttpGet]
        public IActionResult MakeARequest(int id)
        {
            User? customer = GetUserLoggedIn();
            if (customer is null) { return NeedToBeLoggedIn(); }

            // Créer une instance de ServiceOffer (assurez-vous d'injecter IServiceOfferDAL dans votre contrôleur)
            ServiceOffer? serviceOffer = this._serviceOffer.GetService(id);

            // Appeler la méthode Request sur l'instance de ServiceOffer
            return View("Request", serviceOffer is not null && serviceOffer.Request(customer, this._serviceOffer, this._notification));
        }


        public IActionResult ViewServices()
        {
            User? user = GetUserLoggedIn();
            if (user is null) { return NeedToBeLoggedIn(); }

            ViewData["user"] = user.Username;

            return View(ServiceOffer.GetServices(_serviceOffer));
           
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            // Appelle la méthode GetService pour obtenir les détails du service avec l'ID spécifié
            ServiceOffer? service = _serviceOffer.GetService(id);

            if (GetUserLoggedIn() is null) { return NeedToBeLoggedIn(); }

            // Si tout est correct, passe le service à la vue pour l'affichage
            return View(service);
        }

        public IActionResult ManageOffers()
        {
            User? user = GetUserLoggedIn();
            if (user is null) { return NeedToBeLoggedIn(); }
            return View(user.GetOffers(this._serviceOffer));
        }

        public IActionResult PublishOffer()
        {
            ViewBag.Categories = this._serviceCategory.GetCategories();
            if (GetUserLoggedIn() is null) { return NeedToBeLoggedIn(); }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PublishOffer(ServiceOfferViewModel so)
        {
            User? user = GetUserLoggedIn();
            if (user is null) { return NeedToBeLoggedIn(); }

            ServiceOffer offer = new ServiceOffer(so, user);
            if (ModelState.IsValid && offer.Publish(this._serviceOffer))
            {
                TempData["Message"] = "Offer published successfully.";
                return RedirectToAction("ManageOffers", "ServiceOffer");
            }
            return RedirectToAction("PublishOffer", "ServiceOffer");
        }

        [HttpGet]
        public string DeleteOffer(int id)
        {
            return id.ToString();
        }
    }
}
