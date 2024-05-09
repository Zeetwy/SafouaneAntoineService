using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SafouaneAntoineService.DAL.IDAL;
using SafouaneAntoineService.Models;
using SafouaneAntoineService.ViewModels;

namespace SafouaneAntoineService.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserDAL _user;

        public UserController(IUserDAL _user, IServiceOfferDAL _serviceOffer)
        {
            this._user = _user;
        }

        // Cette méthode action est utilisée pour afficher la vue de création de compte. Elle retourne simplement la vue correspondante.
        public IActionResult CreateAccount()
        {
            return View();
        }

        [HttpPost] //on indique que la méthode suivante est destinée à gérer les requêtes POST.
        [ValidateAntiForgeryToken] //C'est un attribut de sécurité qui protège contre les attaques CSRF (Cross-Site Request Forgery).
        public IActionResult CreateAccount(UserViewModel u) //Cette méthode action est appelée lorsque le formulaire de création de compte est soumis. Elle prend en paramètre un objet UserViewModel qui contient les données saisies par l'utilisateur dans le formulaire
        {
            if (ModelState.IsValid) //Principe du DRY (on a déjà mit les data annotation) et bien coté serveur on utilisera la propriete ModelState.IsValid du controlleur et on redirigera vers la page d’acuceil si les données sont correctes 
                                    //car enft cette condition vérifie si le modèle passé à la méthode est valide, c'est-à-dire s'il respecte les règles de validation définies dans la classe UserViewModel.
            {
                User user = new User(u); //On crée un nouvel objet User à partir des données fournies dans l'objet UserViewModel.

                //On tente de sauvegarder le nouvel utilisateur dans la base de données. Si la sauvegarde réussit, un message de succès est stocké dans TempData, sinon un message d'erreur est stocké
                if (user.SaveAccount(_user))
                    TempData["Message"] = "Account created successfully!";
                else
                    TempData["Message"] = "Error during the creation of your account! (Email or Username maybe already used)";
               
                return RedirectToAction("Index", "Home"); // On redirige l'utilisateur vers l'action Index du contrôleur Home après la création du compte.
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
            //On appelle la méthode statique Authenticate de la classe User pour tenter d'authentifier l'utilisateur en utilisant les informations fournies. Le résultat est stocké dans la variable u
            User u = Models.User.Authenticate(username, password, _user);

            // si l'authentification a échoué, l'user est redirigé vers la page d'authentification pour qu'il puisse réessayer.
            if (u is null)
            {
                return RedirectToAction("Authenticate", "User");
            }

            // Si l'authentification réussit, les informations de l'utilisateur (u) sont sérialisées en JSON à l'aide de la bibliothèque Json.NET (JsonConvert.SerializeObject) et stockées dans la session avec la clé "User".
            HttpContext.Session.SetString("User", JsonConvert.SerializeObject(u));

            //return Redirect("/Carpool/SeeAllOffers");

            return RedirectToAction("Home", "Home");
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
