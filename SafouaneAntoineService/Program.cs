using SafouaneAntoineService.DAL;
using SafouaneAntoineService.DAL.IDAL;

var builder = WebApplication.CreateBuilder(args);

//on r�cup�re la chaine de connexion
string connectionString = builder.Configuration.GetConnectionString("default"); 

// Add services to the container.
builder.Services.AddControllersWithViews(); //Cette ligne ajoute les services n�cessaires pour activer les contr�leurs MVC et les vues dans l'application

//on lui dit que dans l�appli quand on te demandera un objec de type IUserDAL tu vas construire un objet UserDAL avec cette chaine de connexion l�E
builder.Services.AddTransient<IUserDAL>(ud => new UserDAL(connectionString)); /*c�d Cette ligne enregistre le service UserDAL avec l'interface IUserDAL dans le conteneur d'injection de d�pendances (DI) de l'application.
                                                                              Elle sp�cifie �galement que chaque fois qu'une d�pendance de type IUserDAL est requise, une nouvelle instance de UserDAL sera cr��e avec la cha��e de connexion sp�cifi�e. */
builder.Services.AddTransient<IServiceOfferDAL>(sod => new ServiceOfferDAL(connectionString));
builder.Services.AddTransient<IServiceCategoryDAL>(scd => new ServiceCategoryDAL(connectionString));
builder.Services.AddTransient<INotificationDAL>(nd => new NotificationDAL(connectionString));
builder.Services.AddDistributedMemoryCache(); //ette ligne ajoute le service de cache distribu�Een m�moire �El'application. Ce service est utilis�Epour stocker temporairement des donn�es en m�moire pour am�liorer les performances de l'application.

// Cette ligne configure le service de session dans l'application.
// Elle sp�cifie notamment le d�lai d'expiration de la session (30 minutes dans cet exemple), ainsi que d'autres options li�es aux cookies de session.
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession(); //pour activer le middleware de session

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
