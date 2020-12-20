using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace GOSM.Models
{
    public class AuthenticateResponse
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string JwtToken { get; set; }

        //[JsonIgnore] // refresh token is returned in http only cookie
        public string RefreshToken { get; set; }

        //public AuthenticateResponse(User user, string jwtToken, string refreshToken)
        //{
        //    ID = user.ID;
        //    Username = user.Username;
        //    JwtToken = jwtToken;
        //    RefreshToken = refreshToken;
        //}
    }
}
