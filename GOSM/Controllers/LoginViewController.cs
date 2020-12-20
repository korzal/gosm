using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GOSM.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;

namespace GOSM.Controllers
{
    public class LoginViewController : Controller
    {
        private string APIUri = "https://localhost:44366/api/Users/";

        public ViewResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(AuthenticateRequest login)
        {
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json");
                using (var response = await httpClient.PostAsync("https://localhost:44366/api/Authentication/authenticate", content))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        ViewBag.Result = "BadReq";
                    }
                    else if(response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        login = JsonConvert.DeserializeObject<AuthenticateRequest>(apiResponse);

                        var loginResponse = JsonConvert.DeserializeObject<AuthenticateResponse>(apiResponse);
                        setTokenCookie(loginResponse.JwtToken, loginResponse.Username);
                        HttpContext.Session.SetString("Username", loginResponse.Username);
                        return RedirectToAction("Index", "Home");
                    }
                    
                }
            }
            return View(login);
        }

        public ViewResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(User register)
        {
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(register), Encoding.UTF8, "application/json");
                using (var response = await httpClient.PostAsync(APIUri, content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    //register = JsonConvert.DeserializeObject<User>(apiResponse);
                    
                    if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        ViewBag.Result = "BadReq";
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        ViewBag.Result = "Created";
                        return RedirectToAction("Index", "Home");
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                    {
                        ViewBag.Result = "Conflict";
                    }
                }
            }
            return View(register);
        }

        private void setTokenCookie(string token, string username)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = false
                //Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("jwtToken", token, cookieOptions);
            Response.Cookies.Append("Username", username, cookieOptions);
        }
    }
}
