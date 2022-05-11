using LetgoEcommerce.Dtos;
using System.Collections.Generic;

namespace LetgoEcommerce.Models
{
    public class ReturnMessage
    {
        public UserProfile user;
        public List<Message> messageList;
        public ReturnProduct product;
        
    }
}
