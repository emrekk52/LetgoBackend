using LetgoEcommerce.Dtos;
using System;
using System.Collections.Generic;

namespace LetgoEcommerce.Models
{
    public class ReturnProduct
    {
        public int? id { get; set; }
        public int category_id { get; set; }

        public int user_id { get; set; }

        public string header { get; set; }

        public string description { get; set; }

        public double price { get; set; }

        public int? state { get; set; }

        public DateTime? created_date { get; set; }
        public List<string> image_list { get; set; }
        public string category { get; set; }
        
    }
}
