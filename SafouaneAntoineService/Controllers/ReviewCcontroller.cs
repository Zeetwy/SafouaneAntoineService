using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SafouaneAntoineService.DAL.IDAL;
using SafouaneAntoineService.Models;

namespace SafouaneAntoineService.Controllers
{
    public class ReviewController : Controller
    {
        private readonly IReviewDAL _review;
        private readonly IServiceRenderedDAL _service;

        public ReviewController(IReviewDAL review, IServiceRenderedDAL service)
        {
            _review = review;
            _service = service;
        }


        public IActionResult RateAService(int id)
        {
            User? currentUser = ControllerHelper.GetUserLoggedIn(this);
            if (currentUser is null) { return ControllerHelper.NeedToBeLoggedIn(this); }


            ServiceRendered? serviceRendered = ServiceRendered.GetServiceById(id, _service);

            if (serviceRendered == null)
            {
                TempData["Message"] = "Service rendered not found.";
                return RedirectToAction("Index", "Home");
            }

            HttpContext.Session.SetString("ServiceRendered", JsonConvert.SerializeObject(serviceRendered));

            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult RateAService(int rating, string comment)
        {
            if (ModelState.IsValid)
            {
                User? currentUser = ControllerHelper.GetUserLoggedIn(this);
                if (currentUser is null) { return ControllerHelper.NeedToBeLoggedIn(this); }

                string? serviceRenderedSession = HttpContext.Session.GetString("ServiceRendered");

                if (serviceRenderedSession == null)
                {
                    TempData["Message"] = "Error: Service rendered not found in session.";
                    return RedirectToAction("Index", "Home"); 
                }

                ServiceRendered? serviceRendered = JsonConvert.DeserializeObject<ServiceRendered>(serviceRenderedSession);

                if (serviceRendered != null)
                {
                    Review review = new Review(rating, comment, currentUser, serviceRendered);

                    if (review.SaveReview(_review))
                    {
                        TempData["Message"] = "Review created successfully!";
                    }
                    else
                    {
                        TempData["Message"] = "Error occurred during the creation of the review.";
                    }

                    return RedirectToAction("ViewServices", "ServiceOffer");
                }
                else
                {
                    TempData["Message"] = "Error: Service rendered could not be loaded from session.";
                    return RedirectToAction("Index", "Home");
                }
            }

            return View();
        }
    }
}
