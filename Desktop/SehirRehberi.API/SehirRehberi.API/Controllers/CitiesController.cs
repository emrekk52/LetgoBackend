using LetgoEcommerce.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LetgoEcommerce.Controllers
{

    [Produces("application/json")]
    [Route("api/city")]
    public class CitiesController : Controller
    {
        private DataContext _context;

        public CitiesController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("getcities")]
        public async Task<IActionResult> GetCities()
        {
            var cities = await _context.City.ToListAsync();
            return Ok(cities);
        }

        [HttpGet("getcity")]
        public async Task<IActionResult> GetCity(int id)
        {
            var city = await _context.City.Where(c=>c.id.Equals(id)).FirstOrDefaultAsync();
            return Ok(city);
        }

    }
}
