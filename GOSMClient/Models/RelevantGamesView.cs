using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GOSMClient.Models
{
    public class RelevantGamesView
    {
        public int ID { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        [Range(0, 8)]
        public GameGenreView Genre { get; set; }
    }
}
