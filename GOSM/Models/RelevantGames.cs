using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GOSM.Models
{
    public class RelevantGames
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public int GameGenreID { get; set; }
        public virtual GameGenre GameGenre { get; set; }
    }
}
