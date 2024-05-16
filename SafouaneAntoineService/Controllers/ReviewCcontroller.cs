using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SafouaneAntoineService.DAL;
using SafouaneAntoineService.DAL.IDAL;
using SafouaneAntoineService.Models;

namespace SafouaneAntoineService.Controllers
{
    public class ReviewCcontroller : Controller
    {
        private readonly IReviewDAL _review;
        private readonly IServiceOfferDAL _offer;

        public ReviewCcontroller(IReviewDAL review, IServiceOfferDAL offer)
        {
            this._review = review;
            _offer = offer;
        }

        public IActionResult RateAService(int id)
        {
            string userSession = HttpContext.Session.GetString("User");
            if (string.IsNullOrEmpty(userSession))
            {
                TempData["Message"] = "Please authenticate first.";
                return RedirectToAction("Authenticate", "User");
            }

            User currentUser = JsonConvert.DeserializeObject<User>(userSession);

            //Mauvais, il faut récuperer l'offre qui est attachée au service !
            ServiceOffer? serviceOffer = ServiceOffer.GetDetails(_offer, id);

            if (serviceOffer == null)
            {
                TempData["Message"] = "Service offer not found.";
                return RedirectToAction("Index", "Home");
            }

            HttpContext.Session.SetString("ServiceOffer", JsonConvert.SerializeObject(serviceOffer));

            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult RateAService(int rating, string comment)
        {
            if (ModelState.IsValid)
            {
                string userSession = HttpContext.Session.GetString("User");
                User currentUser = JsonConvert.DeserializeObject<User>(userSession);

                string serviceOfferSession = HttpContext.Session.GetString("ServiceOffer");
                ServiceOffer serviceOffer = JsonConvert.DeserializeObject<ServiceOffer>(serviceOfferSession);

                Review review = new Review(rating, comment, currentUser, serviceOffer);

                if (review.SaveReview(_review))
                    TempData["Message"] = "Review created successfully!";
                else
                    TempData["Message"] = "Error occurred during the creation of the review.";

                return RedirectToAction("SeeAllOffers", "Carpool");
            }
            return View();
        }



    }
}
