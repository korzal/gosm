using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace GOSM.Models
{
    public class User
    {
        public int ID { get; set; }
        [Required]
        [StringLength(20, ErrorMessage ="{0} length must be between {2} and {1}.", MinimumLength = 4)]
        public string Username { get; set; }
        [Required]
        //[IgnoreDataMember]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }
        public DateTime CreationDate { get; set; }
        public virtual ICollection<UserRelevantGames> UserRelevantGamesList { get; set; }
        [IgnoreDataMember]
        public string JwtToken { get; set; }

        [IgnoreDataMember]
        public List<RefreshToken> RefreshTokens { get; set; }

    }
}
