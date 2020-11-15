using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace GOSM.Models
{
    public class FriendRequest
    {
        public int ID { get; set; }
        public bool IsAccepted { get; set; }
        public DateTime RequestDate { get; set; }

        [Required]
        [ForeignKey("SenderID")]
        public int? SenderID { get; set; }
        public virtual User Sender { get; set; }

        [Required]
        [ForeignKey("RecipientID")]
        public int? RecipientID { get; set; }
        public virtual User Recipient { get; set; }
    }
}
