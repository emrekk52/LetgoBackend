using LetgoEcommerce.Models;
using System;
using System.Collections.Generic;

namespace LetgoEcommerce.Dtos
{
    public class ResultProduct
    {
        public List<ReturnProduct> productList { get; set; }
        public ReturnProduct product { get; set; }
        public UserProfile user { get; set; }
        public Boolean status { get; set; }
        public String message { get; set; }
    }
}
