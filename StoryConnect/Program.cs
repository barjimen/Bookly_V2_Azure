using System.Configuration;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using StoryConnect.Context;
using StoryConnect.Repositories;
using StoryConnect_V2.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
});

builder.Services.AddAzureClients(factory =>
{
    factory.AddSecretClient
    (builder.Configuration.GetSection("KeyVault"));
});

SecretClient secretClient = builder.Services.BuildServiceProvider().GetService<SecretClient>();
KeyVaultSecret SecretOne = await secretClient.GetSecretAsync("StorySQL");

string SecretSQL = SecretOne.Value;

// Add services to the container.
builder.Services.AddDbContext<StoryContext>(options =>
{
    options.UseSqlServer(SecretSQL);
});
builder.Services.AddTransient<BooklyService>();
builder.Services.AddTransient<IRepositoryLibros, RepositoryLibros>();
//builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Libros/Index";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); options.SlidingExpiration = true; // Opcional: renueva el tiempo si el usuario es activo

        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToLogin = context =>
            {
                // Si el usuario está autenticado pero la cookie expiró, redirige a login
                if (context.Request.Path.StartsWithSegments("/api/Usuarios/Login"))
                {
                    // Si es API, devuelves 401 en vez de redirigir
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                }
                else
                {
                    context.Response.Redirect(context.RedirectUri);
                }
                return Task.CompletedTask;
            },
            OnRedirectToAccessDenied = context =>
            {
                context.Response.Redirect("/Libros/Index"); // Opcional si quieres controlar el acceso denegado
                return Task.CompletedTask;
            }
        };
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

app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
