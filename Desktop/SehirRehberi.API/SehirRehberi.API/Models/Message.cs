using System;

namespace LetgoEcommerce.Models
{
    public class Message
    {

        public Message()
        {
            created_date = DateTime.Now;
        }

        public int? id { get; set; }
        public int receiver_id { get; set; }
        public int sender_id { get; set; }
        public int product_id { get; set; }
        public string message { get; set; }
        public DateTime? created_date { get; set; }

    }
}
