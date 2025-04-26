using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using StoryConnect_V2.Services;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using BooklyNugget.Models;

namespace StoryConnect_V2.Controllers
{
    public class ManagedController : Controller
    {
        private BooklyService service;
        public ManagedController(BooklyService service)
        {
            this.service = service;
        }
        public IActionResult LogIn()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LogIn model)
        {
            string token = await this.service.GetTokenAsync(model.email, model.password);
            if (token == null)
            {
                ViewData["MENSAJE"] = "Login incorrecto";
                return View();
            }
            else
            {
                /// Leemos el token
                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                // Sacamos los valores del token
                var idUser = jwtToken.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
                var userName = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                var email = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
                var imagen = jwtToken.Claims.FirstOrDefault(c => c.Type == "imagen")?.Value;

                // Creamos las claims
                ClaimsIdentity identity = new ClaimsIdentity(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    ClaimTypes.Name, // Claim de nombre
                    ClaimTypes.Role  // Claim de rol si algún día añades roles
                );

                // Añadimos las claims que vamos a usar
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, idUser ?? ""));
                identity.AddClaim(new Claim(ClaimTypes.Name, userName ?? ""));
                identity.AddClaim(new Claim(ClaimTypes.Email, email ?? ""));
                identity.AddClaim(new Claim("imagen", imagen));
                identity.AddClaim(new Claim("TOKEN", token)); // Guardamos el token si queremos usarlo luego

                // Creamos el principal
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                // Iniciamos sesión
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    new AuthenticationProperties
                    {
                        ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
                    }
                );

                return RedirectToAction("Index", "Home");
            }
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync
                (CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
