using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GOSM.Models
{
    public class Comment
    {
        public int ID { get; set; }
        [Required]
        public string Text { get; set; }
        public DateTime TimeStamp { get; set; }

        [ForeignKey("PostID")]
        public int PostID { get; set; }
        //public virtual Post Post { get; set; }

        [ForeignKey("UserID")]
        public int UserID { get; set; }
        public virtual User User { get; set; }
        

        
        
    }
}
