using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetgoEcommerce.Dtos;
using LetgoEcommerce.Models;

namespace LetgoEcommerce.Data
{
    public interface IAuthRepository
    {
        Task<User> Register(User user);
        Task<LoginResult> Login(string userName, string password);
        Task<bool> UserExists(string userName);
    }
}
