using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GOSM.Models;
using System.Net.Http;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GOSM.Controllers
{
    public class UserViewController : Controller
    {
        private string APIUri = "https://localhost:44366/api/Users/";
        public async Task<IActionResult> UserList()
        {

            List<User> userList = new List<User>();
            using (var httpClient = new HttpClient())
            {
                var username = PassTokenToHeader(httpClient);
                
                using (var response = await httpClient.GetAsync(APIUri))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        RedirectToAction("Index", "Home");
                    }
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    userList = JsonConvert.DeserializeObject<List<User>>(apiResponse);
                }
                if (username != "admin")
                {
                    User oneUser = new User();
                    oneUser = userList.FirstOrDefault(u => u.Username == username);
                    userList.Clear();
                    userList.Add(oneUser);
                }
            }

            return View(userList);
        }

        //public async Task<IActionResult> EditUser()
        //{

        //}

        public async Task<IActionResult> UserProfile(int id)
        {
            User user = new User();
            using (var httpClient = new HttpClient())
            {
                PassTokenToHeader(httpClient);
                using(var response = await httpClient.GetAsync(APIUri + id))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    user = JsonConvert.DeserializeObject<User>(apiResponse);
                }
            }
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> UserProfile(User user)
        {
            User returnUser = new User();
            using (var httpClient = new HttpClient())
            {
                PassTokenToHeader(httpClient);
                StringContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PutAsync(APIUri + user.ID, content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    ViewBag.Result = "Success";
                    using (var getResponse = await httpClient.GetAsync(APIUri + user.ID))
                    {
                        apiResponse = await getResponse.Content.ReadAsStringAsync();
                        returnUser = JsonConvert.DeserializeObject<User>(apiResponse);
                    }

                }
            }
            return View(returnUser);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            using (var httpClient = new HttpClient())
            {
                PassTokenToHeader(httpClient);
                using (var response = await httpClient.DeleteAsync(APIUri + id))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                }
            }
            return RedirectToAction("UserList");
        }

        public async Task<IActionResult> UserAddGame()
        {
            List<RelevantGames> gamesList = new List<RelevantGames>();
            using (var httpClient = new HttpClient())
            {
                PassTokenToHeader(httpClient);

                using (var response = await httpClient.GetAsync("https://localhost:44366/api/RelevantGames"))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        RedirectToAction("Index", "Home");
                    }
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    gamesList = JsonConvert.DeserializeObject<List<RelevantGames>>(apiResponse);
                    ViewBag.GameID = new SelectList(gamesList, "ID", "Title");
                }
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UserAddGame(RelevantGames game)
        {
            RelevantGames relGame = new RelevantGames();
            List<User> userList = new List<User>();
            User user = new User();
            using (var httpClient = new HttpClient())
            {
                var username = PassTokenToHeader(httpClient);
                
                using (var response = await httpClient.GetAsync("https://localhost:44366/api/RelevantGames/" + game.ID))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    relGame = JsonConvert.DeserializeObject<RelevantGames>(apiResponse);
                }

                using (var response = await httpClient.GetAsync(APIUri))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    userList = JsonConvert.DeserializeObject<List<User>>(apiResponse);
                    user = userList.FirstOrDefault(u => u.Username == username);
                }
                UserRelevantGames games = new UserRelevantGames() { RelevantGamesID = relGame.ID, UserID = user.ID};
                StringContent content = new StringContent(JsonConvert.SerializeObject(games), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync(APIUri + user.ID + "/AddRelevantGame/", content))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    //relGame = JsonConvert.DeserializeObject<RelevantGames>(apiResponse);
                }
            }
            return RedirectToAction("UserList");
        }

        [HttpPost]
        public async Task<IActionResult> UserDeleteGame(int gameId)
        {
            List<User> userList = new List<User>();
            User user = new User();
            using (var httpClient = new HttpClient())
            {
                var username = PassTokenToHeader(httpClient);
                using(var response = await httpClient.GetAsync(APIUri))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    userList = JsonConvert.DeserializeObject<List<User>>(apiResponse);
                    user = userList.FirstOrDefault(u => u.Username == username);
                }
                using (var response = await httpClient.DeleteAsync(APIUri + user.ID + "/DeleteRelevantGame/" + gameId))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                }
            }
            return RedirectToAction("UserList");
        }



        public string PassTokenToHeader(HttpClient httpClient)
        {
            string username = "";
            var token = HttpContext.Request.Cookies.FirstOrDefault(c => c.Key == "jwtToken").Value;
            if (token != null)
            {
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                JwtSecurityToken tok = handler.ReadJwtToken(token);
                username = tok.Claims.FirstOrDefault(t => t.Type == "Username")?.Value;
                //ViewBag.Auth = username;
            }
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            return username;
        }
    }
}
