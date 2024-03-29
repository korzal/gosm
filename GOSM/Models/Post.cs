﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GOSM.Models
{
    public class Post
    {
        public int ID { get; set; }
        [Required]
        public string Text { get; set; }
        public GameGenre Tag { get; set; }
        public DateTime TimeStamp { get; set; }
        [Required]
        public int UserID { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Comment> CommentList { get; set; }

    }
}
