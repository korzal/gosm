using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GOSMClient.Models;
using Newtonsoft.Json;
using System.Net.Http;

namespace GOSMClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            List<RelevantGamesView> gamesList = new List<RelevantGamesView>();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("http://localhost:44366/api/RelevantGames"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    gamesList = JsonConvert.DeserializeObject<List<RelevantGamesView>>(apiResponse);
                }
            }
            return View(gamesList);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
