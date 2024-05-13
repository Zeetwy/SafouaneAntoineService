using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SafouaneAntoineService.DAL;
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

        public IActionResult Index()
        {
            return View();
        }
    }
}