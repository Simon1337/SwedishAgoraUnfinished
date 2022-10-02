using Microsoft.AspNetCore.Mvc;
using SwedishAgora.Data.DataServices.Interfaces;

namespace SwedishAgora.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILoginDataService _loginDataService;

        public LoginController(ILoginDataService loginDataService)
        {
            _loginDataService = loginDataService;
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                return BadRequest();

            var result = await _loginDataService.VerifyCredentialsAsync(userName, password);
            if (result?.AreCredentialsValid == true)
            {
                await _loginDataService.SetUserLoggedInAsync(userName);
                return RedirectToRoute("Home/MarketList");
            }

            ViewBag.LoginResult = result;
            return RedirectToRoute("Home/Login");
        }
    }
}
