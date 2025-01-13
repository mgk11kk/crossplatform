using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Microsoft.AspNetCore.Mvc;
using Lab5.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace Lab5.Controllers
{
    public class AccountController : Controller
    {
        // Внедрение клиента API управления Auth0
        private readonly IManagementApiClient _managementApiClient;

        public AccountController(IManagementApiClient managementApiClient)
        {
            _managementApiClient = managementApiClient;
        }

        // Метод для входа в систему с использованием Auth0
        public async Task Login(string returnUrl = "/")
        {
            // Настройка параметров аутентификации для перехода после успешного входа
            var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
                .WithRedirectUri(returnUrl)
                .Build();

            await HttpContext.ChallengeAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
        }

        // Метод для выхода из системы
        [Authorize]
        public async Task Logout()
        {
            // Настройка параметров аутентификации для выхода
            var authenticationProperties = new LogoutAuthenticationPropertiesBuilder()
                .WithRedirectUri(Url.Action("Index", "Home"))
                .Build();

            await HttpContext.SignOutAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        // Отображение формы регистрации
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(AccountModel model)
        {
            // Обработка логики регистрации пользователя
            if (ModelState.IsValid)
            {
                try
                {
                    var userCreateRequest = new UserCreateRequest
                    {
                        Email = model.Email,
                        Password = model.Password,
                        FullName = model.FullName,
                        PhoneNumber = model.PhoneNumber,
                        Connection = "Username-Password-Authentication",
                        EmailVerified = true, // Устанавливаем email как подтвержденный
                        PhoneVerified = true  // Устанавливаем номер телефона как подтвержденный
                    };

                    // Создание пользователя через API управления Auth0
                    var user = await _managementApiClient.Users.CreateAsync(userCreateRequest);

                    // Перенаправление на страницу входа или отображение сообщения об успешной регистрации
                    return RedirectToAction("Login");
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = "Произошла ошибка при регистрации: " + ex.Message;
                    return View(model);
                }
            }
            return View(model);
        }

        // Отображение профиля пользователя
        [Authorize]
        public IActionResult Profile()
        {
            // Вывод всех данных о пользователе из его утверждений (claims)
            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"{claim.Type}: {claim.Value}");
            }
            return View(new
            {
                Name = User.Identity.Name,
                PhoneNumber = User.Claims.FirstOrDefault(c => c.Type == "phone_number")?.Value,
                EmailAddress = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value,
                ProfileImage = User.Claims.FirstOrDefault(c => c.Type == "picture")?.Value
            });
        }
    }
}