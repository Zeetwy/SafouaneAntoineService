using Microsoft.AspNetCore.Mvc;
using SafouaneAntoineService.DAL;
using SafouaneAntoineService.DAL.IDAL;
using SafouaneAntoineService.Models;
using SafouaneAntoineService.ViewModels;

namespace SafouaneAntoineService.Controllers
{
    public class ServiceRenderedController : Controller
    {
        private readonly IServiceRenderedDAL _serviceRendered;
        private readonly IServiceOfferDAL _serviceOffer;
        public ServiceRenderedController(IServiceRenderedDAL _serviceRendered, IServiceOfferDAL _serviceOffer)
        {
            this._serviceRendered = _serviceRendered;
            this._serviceOffer = _serviceOffer;
        }

        [HttpGet]
        public IActionResult ViewRequests(int offer_id)
        {
            User? user = ControllerHelper.GetUserLoggedIn(this);
            if (user is null) { return ControllerHelper.NeedToBeLoggedIn(this); }

            ServiceOffer? offer = this._serviceOffer.GetService(offer_id);
            if (offer is null || offer.Provider.Id != user.Id)
            {
                return View(null);
            }
            ViewBag.OfferId = offer_id;
            return View(this._serviceRendered.GetRequests(offer));
        }



        public IActionResult ConfirmRequest(int id)
        {
          
            if (ControllerHelper.GetUserLoggedIn(this) is null) { return ControllerHelper.NeedToBeLoggedIn(this); }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmRequest(ServiceRenderedViewModel sr)
        {
            User? user = ControllerHelper.GetUserLoggedIn(this);
            if (user is null) { return ControllerHelper.NeedToBeLoggedIn(this); }

            ServiceRendered? request = this._serviceRendered.GetRequest(sr.Id);
           
            if (ModelState.IsValid && request is not null)
            {
                if (request.Confirm(sr.NumberOfHours, sr.Date, this._serviceRendered))
                {
                    TempData["Message"] = "Service confirmed with success.";
                    return RedirectToAction("ViewRequests", "ServiceRendered");
                }
                else
                {
                    TempData["Message"] = "Error with the confirmation of the service.";
                }
            }

            return View(sr);
        }


    }
}
