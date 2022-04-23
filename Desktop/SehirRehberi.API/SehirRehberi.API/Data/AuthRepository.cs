using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetgoEcommerce.Dtos;
using LetgoEcommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace LetgoEcommerce.Data
{
    public class AuthRepository : IAuthRepository
    {
        private DataContext _context;

        public AuthRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<User> Register(User user)
        {


            await _context.Userr.AddAsync(user);
            await _context.SaveChangesAsync();


            return user;



        }



        public async Task<LoginResult> Login(string email, string password)
        {
            var user = await _context.Userr.FirstOrDefaultAsync(u => u.email.Equals(email));

            if (user == null)
            {
                return new LoginResult() { user = null, status = false, message = "Böyle bir kullanıcı mevcut değil" };
            }

            if (!user.password.Equals(password))
            {
                return new LoginResult() { user = null, status = false, message = "Parola ya da email hatalı" };

            }

            

            return new LoginResult() { user = user, status = true, message = "Giriş başarılı", };

        }



        public async Task<bool> UserExists(string email)
        {
            if (await _context.Userr.AnyAsync(u => u.email.Equals(email)))
            {
                return true;
            }

            return false;
        }
    }
}
