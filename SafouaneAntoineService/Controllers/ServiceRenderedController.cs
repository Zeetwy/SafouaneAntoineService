using Microsoft.AspNetCore.Mvc;
using SafouaneAntoineService.DAL.IDAL;
using SafouaneAntoineService.Models;

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
    }
}
