using Microsoft.AspNetCore.Mvc;
using SafouaneAntoineService.DAL.IDAL;
using SafouaneAntoineService.Models;

namespace SafouaneAntoineService.Controllers
{
    public class NotificationController : Controller
    {
        private readonly INotificationDAL _notification;
        private readonly IUserDAL _user;

        public NotificationController(INotificationDAL _notification, IUserDAL _user)
        {
            this._notification = _notification;
            this._user = _user;
        }

        public IActionResult ViewNotifications()
        {
            User? user = ControllerHelper.GetUserLoggedIn(this);
            if (user is null) { return ControllerHelper.NeedToBeLoggedIn(this); }
            return View(user.GetNotifications(this._notification));
        }
    }
}