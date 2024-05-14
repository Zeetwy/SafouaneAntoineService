using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace SafouaneAntoineService.Models // Not entirely sure if it should be here
{
    public static class ControllerHelper
    {
        public static User? GetUserLoggedIn(Controller controller)
        {
            string? user_session_string = controller.HttpContext.Session.GetString("User");
            if (string.IsNullOrEmpty(user_session_string))
            {
                return null;
            }
            return JsonConvert.DeserializeObject<User>(user_session_string);
        }

        public static IActionResult NeedToBeLoggedIn(Controller controller)
        {
            controller.TempData["Message"] = "You need to be logged in to view this page.";
            return controller.RedirectToAction("Authenticate", "User");
        }
    }
}
