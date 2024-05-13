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
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult SendNotification(string message)
        {
            try
            {
                // Récupérer l'utilisateur à partir de la session
                string userSession = HttpContext.Session.GetString("User");

                // Vérifier si la session contient des données utilisateur
                if (string.IsNullOrEmpty(userSession))
                {
                    TempData["ErrorMessage"] = "User session is empty.";
                    return RedirectToAction("Index", "Home");
                }

                // Désérialiser les données de session en objet User
                User user = JsonConvert.DeserializeObject<User>(userSession);

                // Vérifier si l'utilisateur a été trouvé dans la session
                if (user == null)
                {
                    TempData["ErrorMessage"] = "User not found in session data.";
                    return RedirectToAction("Index", "Home");
                }

                // Appeler la méthode SendNotification de la DAL
               Notification.SendNotification(user, message);

                TempData["SuccessMessage"] = "Notification sent successfully.";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred while sending notification: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }





    }
}
