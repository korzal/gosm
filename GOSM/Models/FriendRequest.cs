using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GOSM.Models
{
    public class FriendRequest
    {
        public int ID { get; set; }
        public int? SenderID { get; set; }
        public int? RecipientID { get; set; }
        public bool IsAccepted { get; set; }
        public DateTime RequestDate { get; set; }

        [ForeignKey("SenderID")]
        public virtual User Sender { get; set; }

        [ForeignKey("RecipientID")]
        public virtual User Recipient { get; set; }
    }
}
