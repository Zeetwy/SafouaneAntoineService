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

        public ServiceOfferController(IServiceOfferDAL _serviceOffer, IServiceCategoryDAL _serviceCategory)
        {
            this._serviceOffer = _serviceOffer;
            this._serviceCategory = _serviceCategory;
        }

        public IActionResult ViewServices()
        {
            string? userSession = HttpContext.Session.GetString("User");
            if (string.IsNullOrEmpty(userSession))
            {
                TempData["Message"] = "Please Authenticate in first";
                return RedirectToAction("Authenticate", "User");
            }
            User u = JsonConvert.DeserializeObject<User>(userSession);

            ViewData["user"] = u.Username;

            return View(ServiceOffer.GetServices(_serviceOffer));
           
        }

        public IActionResult Details(int id)
        {
            // Appelle la méthode GetService pour obtenir les détails du service avec l'ID spécifié
            ServiceOffer service = _serviceOffer.GetService(id);

            //on récupère la chaîne de session appelée 'User'
            string? user_session_string = HttpContext.Session.GetString("User");

            // on vérifie si cette chaîne de session est nulle ou vide 
            if (string.IsNullOrEmpty(user_session_string))
            {
                //comme aucun utilisateur n'est actuellement authentifié, on définit un message dans TempData indiquant à l'utilisateur qu'il doit être connecté pour voir la page.
                TempData["Message"] = "You need to be logged in to view this page.";
                return RedirectToAction("Authenticate", "User");
            }
            //On désérialise la chaîne JSON en un objet User à l'aide de JsonConvert. Cela permet d'obtenir les info de l'user connecté à partir de la session.
            User user = JsonConvert.DeserializeObject<User>(user_session_string);

            // Si tout est correct, passe le service à la vue pour l'affichage
            return View(service);
        }
        public IActionResult ManageOffers()
        {
            string? user_session_string = HttpContext.Session.GetString("User");
            if (string.IsNullOrEmpty(user_session_string))
            {
                TempData["Message"] = "You need to be logged in to view this page.";
                return RedirectToAction("Authenticate", "User");
            }
            User user = JsonConvert.DeserializeObject<User>(user_session_string);
            return View(user.GetOffers(this._serviceOffer));
        }

        public IActionResult PublishOffer()
        {
            ViewBag.Categories = this._serviceCategory.GetCategories();

            string? user_session_string = HttpContext.Session.GetString("User");
            if (string.IsNullOrEmpty(user_session_string))
            {
                TempData["Message"] = "You need to be logged in to view this page.";
                return RedirectToAction("Authenticate", "User");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PublishOffer(ServiceOfferViewModel so)
        {
            string? user_session_string = HttpContext.Session.GetString("User");
            if (string.IsNullOrEmpty(user_session_string))
            {
                TempData["Message"] = "You need to be logged in to view this page.";
                return RedirectToAction("Authenticate", "User");
            }
            User user = JsonConvert.DeserializeObject<User>(user_session_string);

            ServiceOffer offer = new ServiceOffer(so);
            if (ModelState.IsValid && user.PublishOffer(offer, this._serviceOffer))
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
