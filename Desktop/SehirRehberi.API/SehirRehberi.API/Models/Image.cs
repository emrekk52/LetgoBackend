using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LetgoEcommerce.Models
{
    public class Image
    {

        public int? id { get; set; }
        public byte[] image { get; set; }
        public int uid { get; set; }
        public string description { get; set; }
        public int? product_id { get; set; }
        public string photo_url { get; set; }
    }
}
