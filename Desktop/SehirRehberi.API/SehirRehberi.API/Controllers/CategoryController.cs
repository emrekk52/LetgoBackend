using LetgoEcommerce.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace LetgoEcommerce.Controllers
{

    [Produces("application/json")]
    [Route("api/category")]
    public class CategoryController:Controller
    {
        private DataContext _context;
        public CategoryController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("GetCategories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Category.ToListAsync();
            return Ok(categories);
        }

    }
}
