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

namespace GOSM.Controllers
{
    public class PostViewController : Controller
    {
        private string APIUri = "https://localhost:44366/api/Posts/";

        public async Task<IActionResult> PostList()
        {
            List<Post> postList = new List<Post>();
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
                    postList = JsonConvert.DeserializeObject<List<Post>>(apiResponse);
                }
                
            }

            return View(postList);
        }

        [HttpPost]
        public async Task<IActionResult> CreateComment(int id, string comment)
        {
            Comment reqComment = new Comment();
            Post reqPost = new Post();
            using (var httpClient = new HttpClient())
            {
                var username = PassTokenToHeader(httpClient);
                using (var response = await httpClient.GetAsync(APIUri + id))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        RedirectToAction("Index", "Home");
                    }
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    reqPost = JsonConvert.DeserializeObject<Post>(apiResponse);
                }
                reqComment.Text = comment;
                reqComment.UserID = reqPost.UserID;
                reqComment.PostID = id;
                StringContent content = new StringContent(JsonConvert.SerializeObject(reqComment), Encoding.UTF8, "application/json");
                using (var response = await httpClient.PostAsync(APIUri + id + "/Comments", content))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        RedirectToAction("Index", "Home");
                    }
                }
            }
            return RedirectToAction("PostList");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteComment(int postId, int commentId)
        {
            using (var httpClient = new HttpClient())
            {
                PassTokenToHeader(httpClient);
                using (var response = await httpClient.DeleteAsync(APIUri + postId + "/Comments/" + commentId))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();

                }
            }
            return RedirectToAction("PostList");
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
