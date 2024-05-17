using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SafouaneAntoineService.DAL.IDAL;
using SafouaneAntoineService.Models;
using SafouaneAntoineService.ViewModels;

namespace SafouaneAntoineService.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserDAL _user;

        public UserController(IUserDAL _user)
        {
            this._user = _user;
        }

        public IActionResult CreateAccount()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateAccount(UserViewModel u)
        {
            if (ModelState.IsValid)
            {
                User user = new User(u);

                if (user.SaveAccount(_user))
                    TempData["Message"] = "Account created successfully!";
                else
                    TempData["Message"] = "Error during the creation of your account! (Email or Username maybe already used)";
               
                return RedirectToAction("Index", "Home");
            }
            return View(u);
        }

        public IActionResult Authenticate()
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Authenticate(string username, string password)
        {
            User? u = Models.User.Authenticate(username, password, _user);

            if (u is null)
            {
                return RedirectToAction("Authenticate", "User");
            }

            // les informations de l'utilisateur (u) sont sérialisées en JSON et stockées dans la session avec la clé "User".
            HttpContext.Session.SetString("User", JsonConvert.SerializeObject(u));

            return Redirect("/ServiceOffer/ViewServices");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["State"] = "Disconnect";
            return RedirectToAction("Authenticate");
        }

        public IActionResult Account()
        {
            User? user = ControllerHelper.GetUserLoggedIn(this);
            if (user is null) { return ControllerHelper.NeedToBeLoggedIn(this); }
            _user.RefreshInfo(ref user);
            HttpContext.Session.SetString("User", JsonConvert.SerializeObject(user));
            return View(user);
        }

        public IActionResult ModifyContact()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ModifyContact(ModifyContactViewModel mcvm)
        {
            User? user = ControllerHelper.GetUserLoggedIn(this);
            if (user is null) { return ControllerHelper.NeedToBeLoggedIn(this); }
            if (ModelState.IsValid)
            {
                user.ChangeContact(mcvm.Email, this._user);
                return RedirectToAction("Account");
            }
            return View(mcvm);
        }
    }
}
