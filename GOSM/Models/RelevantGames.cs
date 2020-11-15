using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace GOSM.Models
{
    public class RelevantGames
    {
        public int ID { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        [Range(0, 8)]
        public GameGenre Genre { get; set; }
        //[JsonIgnore]
        //public virtual ICollection<UserRelevantGames> UserRelevantGamesList { get; set; }
    }
}
