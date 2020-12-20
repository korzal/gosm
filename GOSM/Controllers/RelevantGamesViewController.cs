using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GOSM.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace GOSM.Controllers
{
    public class RelevantGamesViewController : Controller
    {
        private string APIUri = "https://localhost:44366/api/RelevantGames/";
        public async Task<IActionResult> RelevantGamesList()
        {
            List<RelevantGames> gamesList = new List<RelevantGames>();
            using (var httpClient = new HttpClient())
            {
                PassTokenToHeader(httpClient);

                using (var response = await httpClient.GetAsync(APIUri))
                {
                    if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        RedirectToAction("Index", "Home");
                    }
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    gamesList = JsonConvert.DeserializeObject<List<RelevantGames>>(apiResponse);
                }
            }
            
            return View(gamesList);
        }

        public ViewResult GetRelevantGame() => View();

        [HttpPost]
        public async Task<IActionResult> GetRelevantGame(int id)
        {
            RelevantGames game = new RelevantGames();
            using (var httpClient = new HttpClient())
            {
                PassTokenToHeader(httpClient);
                using (var response = await httpClient.GetAsync(APIUri + id))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    game = JsonConvert.DeserializeObject<RelevantGames>(apiResponse);
                }
            }
            return View(game);
        }

        public ViewResult AddRelevantGame() => View();

        [HttpPost]
        public async Task<IActionResult> AddRelevantGame(RelevantGames game)
        {
            RelevantGames relGame = new RelevantGames();
            using (var httpClient = new HttpClient())
            {
                PassTokenToHeader(httpClient);
                StringContent content = new StringContent(JsonConvert.SerializeObject(game), Encoding.UTF8, "application/json");
                using (var response = await httpClient.PostAsync(APIUri, content))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                    {
                        return RedirectToAction("RelevantGamesList");
                    }
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    relGame = JsonConvert.DeserializeObject<RelevantGames>(apiResponse);
                }
            }
            return View(relGame);
        }

        public async Task<IActionResult> UpdateRelevantGame(int id)
        {
            RelevantGames game = new RelevantGames();
            using (var httpClient = new HttpClient())
            {
                PassTokenToHeader(httpClient);
                using (var response = await httpClient.GetAsync(APIUri + id))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    game = JsonConvert.DeserializeObject<RelevantGames>(apiResponse);
                }
                return View(game);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRelevantGame(RelevantGames game)
        {
            RelevantGames relGame = new RelevantGames();
            using (var httpClient = new HttpClient())
            {
                PassTokenToHeader(httpClient);
                StringContent content = new StringContent(JsonConvert.SerializeObject(game), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PutAsync(APIUri + game.ID, content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    ViewBag.Result = "Success";
                    using (var getResponse = await httpClient.GetAsync(APIUri + game.ID))
                    {
                        apiResponse = await getResponse.Content.ReadAsStringAsync();
                        relGame = JsonConvert.DeserializeObject<RelevantGames>(apiResponse);
                    }

                }
            }
            return View(relGame);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRelevantGame(int id)
        {
            using (var httpClient = new HttpClient())
            {
                PassTokenToHeader(httpClient);
                using (var response = await httpClient.DeleteAsync(APIUri + id))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                }
            }
            return RedirectToAction("RelevantGamesList");
        }

        public void PassTokenToHeader(HttpClient httpClient)
        {
            var token = HttpContext.Request.Cookies.FirstOrDefault(c => c.Key == "jwtToken").Value;
            if (token != null)
            {
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                JwtSecurityToken tok = handler.ReadJwtToken(token);
                //var username = tok.Claims.FirstOrDefault(t => t.Type == "Username")?.Value;
                //ViewBag.Auth = username;
            }
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }
}
