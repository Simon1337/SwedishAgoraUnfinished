using Microsoft.AspNetCore.Mvc;
using SwedishAgora.Models;
using System.Diagnostics;

namespace SwedishAgora.Controllers
{
    public sealed class HomeController : Controller
    {
        public IActionResult Index() => View();
        public IActionResult Login() => View();
        public IActionResult Register() => View();
        public IActionResult MarketList() => View(); // not implemented


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
            => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}