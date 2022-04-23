using AutoMapper.Configuration;
using LetgoEcommerce.Data;
using LetgoEcommerce.Dtos;
using LetgoEcommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LetgoEcommerce.Controllers
{

    [Produces("application/json")]
    [Route("api/Profile")]
    public class ProfileController : Controller
    {


        private DataContext _context;

        public ProfileController(DataContext context)
        {
            _context = context;
        }



        [HttpGet("GetProfile")]
        public async Task<IActionResult> GetProfile(int id)
        {

            var user = await _context.Userr.Where(u => u.id.Equals(id)).FirstOrDefaultAsync();

            var image = await _context.images.Where(i => i.uid.Equals(user.id) && i.description.Equals("profile")).FirstOrDefaultAsync();

            var city = await _context.City.Where(c => c.id.Equals(user.city_id)).FirstOrDefaultAsync();

            var photo = "";
            if (image != null)
            {

                if (image.photo_url != null)
                    photo = image.photo_url;
                else
                {
                    string base64Photo = Convert.ToBase64String(image.image);

                    var photoUrl = "data:image/png;base64," + base64Photo;

                    photo = photoUrl;
                }
            }

            var _user = new UserProfile()
            {
                id = user.id,
                city_id = user.city_id,
                surname = user.surname,
                name = user.name,
                email = user.email,
                image = photo,
                city = city.city_name
            };

            return Ok(_user);
        }




        [HttpPost("AddPhoto")]
        public async Task<IActionResult> AddPhoto([FromBody] UpdatePhoto photo)
        {


            var image = await _context.images.Where(i => i.uid.Equals(photo.id) && i.description.Equals("profile")).FirstOrDefaultAsync();


            if (image == null)
            {
                var _image = new Image()
                {
                    uid = (int)photo.id,
                    image = null,
                    photo_url = photo.PhotoUrl,
                    description = "profile",
                    product_id = null


                };
                await _context.images.AddAsync(_image);
                await _context.SaveChangesAsync();

                return Ok("Profil fotoğrafı eklendi");
            }
            else
            {

                image.photo_url = photo.PhotoUrl;
                await _context.SaveChangesAsync();

                return Ok("Profil fotoğrafı güncellendi");
            }




        }







    }
}
