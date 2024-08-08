using SafouaneAntoineService.DAL;
using SafouaneAntoineService.DAL.IDAL;

var builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("default"); 

builder.Services.AddControllersWithViews();

builder.Services.AddTransient<IUserDAL>(ud => new UserDAL(connectionString));
builder.Services.AddTransient<IServiceOfferDAL>(sod => new ServiceOfferDAL(connectionString));
builder.Services.AddTransient<IServiceCategoryDAL>(scd => new ServiceCategoryDAL(connectionString));
builder.Services.AddTransient<INotificationDAL>(nd => new NotificationDAL(connectionString));
builder.Services.AddTransient<IServiceRenderedDAL>(srd => new ServiceRenderedDAL(connectionString));
builder.Services.AddTransient<IReviewDAL>(rd => new ReviewDAL(connectionString));
builder.Services.AddDistributedMemoryCache();

// Cette ligne configure le service de session dans l'application.
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
