using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GOSM.Models
{
    public class UserRelevantGames
    {
        public int UserID { get; set; }
        public int RelevantGamesID { get; set; }
        public virtual User User { get; set; }
        public virtual RelevantGames RelevantGames { get; set; }
    }
}
