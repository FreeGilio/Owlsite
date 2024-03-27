using OWL.DataAccess.DB;
using OWL.DataAccess.Repository;
using OWL.Core.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Register DatabaseConnection
builder.Services.AddSingleton<DatabaseConnection>(_ => new DatabaseConnection("Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=OWL;Integrated Security=True"));

builder.Services.AddScoped<ICharacterRepository, CharacterRepository>();

// Add services to the container.
builder.Services.AddControllersWithViews();

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
