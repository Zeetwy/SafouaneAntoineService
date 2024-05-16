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
        private readonly IUserDAL _user;
        public ServiceRenderedController(IServiceRenderedDAL _serviceRendered, IServiceOfferDAL _serviceOffer, IUserDAL user)
        {
            this._serviceRendered = _serviceRendered;
            this._serviceOffer = _serviceOffer;
           this._user = user;  
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
                if (user.Id == request.Provider.Id && request.Confirm(sr.NumberOfHours, sr.Date, this._serviceRendered))
                {
                    TempData["Message"] = "Service confirmed with success.";
                    return RedirectToAction("ManageOffers", "ServiceOffer");
                    //return RedirectToAction("ViewRequests", "ServiceRendered", new { id = sr.ServiceOfferId });

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

            ServiceRendered? service = this._serviceRendered.GetRequest(id);

            if (service is not null)
            {
                if (user.Id == service.Customer.Id && service.Validate(this._serviceRendered, this._user))
                {

                    TempData["Message"] = "Service validated successfully.";
                    return RedirectToAction("ValidateService"); // Redirige vers la vue ViewProvidedServices
                }
                else
                {
                    TempData["Message"] = "You are not authorized to validate this service or an error occurred while validating the service.";
                }
            }
            else
            {
                TempData["Message"] = "Service rendered not found.";
            }

            return RedirectToAction("ViewProvidedServices", "ServiceRendered"); // Redirige vers la page d'accueil ou une autre page appropriée


        }

      /*  [HttpGet]
        public IActionResult ViewDetailsServiceRendered(int id)
        {
            User? currentUser = ControllerHelper.GetUserLoggedIn(this);
            if (currentUser == null)
            {
                return ControllerHelper.NeedToBeLoggedIn(this);
            }

            ServiceRendered? serviceRendered = this._serviceRendered.GetServiceRendered(id);

            // return View(new { ServiceOffer = serviceOffer, CurrentUser = currentUser });
            ServiceRenderedDetailsViewModel srvm = new ServiceRenderedDetailsViewModel
            {
                ServiceRendered = serviceRendered,
                CurrentUser = currentUser
            };

            return View(srvm);
        }
       */






    }
}
