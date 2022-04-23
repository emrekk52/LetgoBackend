using System;

namespace LetgoEcommerce.Models
{
    public class Product
    {

        public Product()
        {
            created_date = DateTime.Now;
            state = 0;

        }

        public int? id { get; set; }
        public int category_id { get; set; }

        public int user_id { get; set; }

        public string header { get; set; }

        public string description { get; set; }

        public double price { get; set; }

        public int? state { get; set; }

        public DateTime? created_date { get; set; }


    }
}
