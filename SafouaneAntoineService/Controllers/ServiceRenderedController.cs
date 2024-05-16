using Microsoft.AspNetCore.Mvc;
using SafouaneAntoineService.DAL.IDAL;
using SafouaneAntoineService.Models;
using SafouaneAntoineService.ViewModels;

namespace SafouaneAntoineService.Controllers
{
    public class ServiceRenderedController : Controller
    {
        private readonly IServiceRenderedDAL _serviceRendered;
        private readonly IServiceOfferDAL _serviceOffer;
        private readonly IUserDAL _user;
        public ServiceRenderedController(IServiceRenderedDAL _serviceRendered, IServiceOfferDAL _serviceOffer, IUserDAL _user)
        {
            this._serviceRendered = _serviceRendered;
            this._serviceOffer = _serviceOffer;
            this._user = _user;  
        }

        [HttpGet]
        public IActionResult ViewRequests(int offer_id)
        {
            User? user = ControllerHelper.GetUserLoggedIn(this);
            if (user is null) { return ControllerHelper.NeedToBeLoggedIn(this); }

            ServiceOffer? offer = ServiceOffer.GetOffer(offer_id, this._serviceOffer);
            if (offer is null || offer.Provider.Id != user.Id)
            {
                return View(null);
            }
            ViewBag.OfferId = offer_id;
            return View(offer.GetRequests(this._serviceRendered));
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

            ServiceRendered? request = ServiceRendered.GetServiceById(sr.Id, this._serviceRendered);

            if (ModelState.IsValid && request is not null)
            {
                if (user.Id == request.Provider.Id && request.Confirm(sr.NumberOfHours, sr.Date, this._serviceRendered))
                {
                    TempData["Message"] = "Service confirmed with success.";
                    return RedirectToAction("ManageOffers", "ServiceOffer");
                }
                else
                {
                    TempData["Message"] = "Error with the confirmation of the service.";
                }
            }

            return View(sr);
        }

        //Pour voir les services fourni à un utilisateur en particulier
        public IActionResult ViewProvidedServices()
        {
            User? user = ControllerHelper.GetUserLoggedIn(this);
            if (user is null) { return ControllerHelper.NeedToBeLoggedIn(this); }

            return View(user.GetServicesRenderedByUserr(this._serviceRendered));
        }

        public IActionResult ValidateService(int id)
        {
            User? user = ControllerHelper.GetUserLoggedIn(this);
            if (user is null) { return ControllerHelper.NeedToBeLoggedIn(this); }

            ServiceRendered? service = ServiceRendered.GetServiceById(id, this._serviceRendered);

            if (service is not null)
            {
                bool success = user.Id == service.Customer.Id && service.Validate(this._serviceRendered, this._user);
                return View((success, service.Id));
            }
            else
            {
                TempData["Message"] = "Service rendered not found.";
            }

            return RedirectToAction("ViewProvidedServices", "ServiceRendered");
        }
    }
}
