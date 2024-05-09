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
            if (ModelState.IsValid && user.PublishOffer(so, this._serviceOffer))
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
