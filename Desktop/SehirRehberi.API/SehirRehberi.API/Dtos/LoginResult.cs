using LetgoEcommerce.Models;

namespace LetgoEcommerce.Dtos
{
    public class LoginResult
    {
        public string message { get; set; }
        public bool status { get; set; }
        public User user { get; set; }
    }
}
