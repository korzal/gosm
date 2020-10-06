using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GOSM.Models
{
    public class Comment
    {
        public int ID { get; set; }
        public string Text { get; set; }
        public DateTime TimeStamp { get; set; }
        public int UserID { get; set; }
        public virtual User User { get; set; }
    }
}
