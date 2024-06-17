using OWL.DataAccess.DB;
using OWL.DataAccess.Repository;
using OWL.Core.Interfaces;
using OWL.Core.Services;
using OWL.Core.Models;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Register DatabaseConnection
builder.Services.AddSingleton<DatabaseConnection>(_ => new DatabaseConnection("Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=OWL;Integrated Security=True"));

// Register Repositories
builder.Services.AddScoped<ICharacterRepository, CharacterRepository>();
builder.Services.AddScoped<IFightstyleRepository, FightstyleRepository>();
builder.Services.AddScoped<IMoveRepository, MoveRepository>();
builder.Services.AddScoped<INewsRepository, NewsRepository>();

// Register Services
builder.Services.AddScoped<CharacterService>();
builder.Services.AddScoped<FightstyleService>();
builder.Services.AddScoped<MoveService>();
builder.Services.AddScoped<NewsService>();

// Register Models
builder.Services.AddScoped<Character>();
builder.Services.AddScoped<Fightstyle>();
builder.Services.AddScoped<Move>();


// Add services to the container.
builder.Services.AddControllersWithViews();

// Add logging services
builder.Logging.ClearProviders();
builder.Logging.AddConsole(); 

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
