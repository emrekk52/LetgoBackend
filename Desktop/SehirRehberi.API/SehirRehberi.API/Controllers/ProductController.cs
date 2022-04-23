using LetgoEcommerce.Data;
using LetgoEcommerce.Dtos;
using LetgoEcommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LetgoEcommerce.Controllers
{


    [Produces("application/json")]
    [Route("api/Product")]
    public class ProductController : Controller
    {
        private DataContext _context;
        public ProductController(DataContext context)
        {

            _context = context;
        }




        [HttpGet("GetMyProducts")]
        public async Task<IActionResult> GetMyProducts(int user_id)
        {

            var products = await _context.Product.Where(p => p.user_id.Equals(user_id)).OrderBy(p => p.created_date).ToListAsync();


            if (products == null)
            {
                return Ok(new ResultProduct() { status = false, message = "Herhangi bir satışınız bulunmamaktadır" });
            }



            var _products = new List<ReturnProduct>();

            products.ForEach(p =>
            {
                var _category = _context.Category.Where(c => c.id.Equals(p.category_id)).FirstOrDefault();

                var rProducts = new ReturnProduct()
                {
                    id = p.id,
                    category_id = p.category_id,
                    user_id = p.user_id,
                    header = p.header,
                    description = p.description,
                    price = p.price,
                    state = p.state,
                    created_date = p.created_date,
                    image_list = getProductImageList((int)p.id),
                    category = _category != null ? _category.name : "",
                };

                _products.Add(rProducts);

            });




            return Ok(new ResultProduct() { productList = _products, status = true, message = "Satışlar başarılı bir şekilde getirildi" });
        }



        [HttpGet("GetProductById")]
        public async Task<IActionResult> GetProductById(int id)
        {

            var product = await _context.Product.Where(p => p.id.Equals(id)).OrderBy(p => p.created_date).FirstOrDefaultAsync();


            if (product == null)
            {
                return Ok(new ResultProduct() { status = false, message = "İstenilen ürüne ait detay bulunamadı" });
            }

            var category = await _context.Category.Where(c => c.id.Equals(product.category_id)).FirstOrDefaultAsync();

            var user = await _context.Userr.Where(u => u.id.Equals(product.user_id)).FirstOrDefaultAsync();

            var image = await _context.images.Where(i => i.uid.Equals(user.id) && i.description == "profile").FirstOrDefaultAsync();

            var city = await _context.City.Where(c => c.id.Equals(user.city_id)).FirstOrDefaultAsync();
            var photoUrl = "";
            if (image.photo_url == null)
            {
                string base64Photo = Convert.ToBase64String(image.image);
                photoUrl = "data:image/png;base64," + base64Photo;
            }
            else
            {
                photoUrl = image.photo_url;
            }

            var _user = new UserProfile()
            {
                id = user.id,
                name = user.name,
                surname = user.surname,
                city_id = user.city_id,
                email = user.email,
                city = city.city_name,
                image = photoUrl
            };

            var rProducts = new ReturnProduct()
            {
                id = product.id,
                category_id = product.category_id,
                user_id = product.user_id,
                header = product.header,
                description = product.description,
                price = product.price,
                state = product.state,
                created_date = product.created_date,
                image_list = getProductImageList((int)product.id),
                category = category == null ? "" : category.name,

            };

            return Ok(new ResultProduct() { user = _user, product = rProducts, status = true, message = "İstenilen ürüne ait detay başarılı bir şekilde getirildi" });
        }


        public List<string> getProductImageList(int prId)
        {
            var images = _context.images.Where(i => i.product_id.Equals(prId)).ToList();


            if (images == null) return null;

            var stringList = new List<string>();

            images.ForEach(i =>
            {
                var photoUrl = "";
                if (i.photo_url == null)
                {
                    string base64Photo = Convert.ToBase64String(i.image);
                    photoUrl = "data:image/png;base64," + base64Photo;
                }
                else
                {
                    photoUrl = i.photo_url;
                }


                stringList.Add(photoUrl);
            });



            return stringList;

        }

        [HttpPost("UpdateProductState")]
        public async Task<IActionResult> UpdateProductState([FromBody] UpdateProductState updateProductState)
        {
            var product = await _context.Product.Where(p => p.id.Equals(updateProductState.product_id)).FirstOrDefaultAsync();


            if (product == null)
                return Ok("*Ürünün id bilgisine ulaşılamadı");

            product.state = updateProductState.state;

            await _context.SaveChangesAsync();


            return Ok(updateProductState.state == 1 ? "*İlan yayından kaldırıldı" : "*İlan tekrar yayına alındı");
        }


        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct([FromBody] ReturnProduct product)
        {


            var _product = new Product()
            {
                category_id = product.category_id,
                header = product.header,
                description = product.description,
                user_id = product.user_id,
                price = product.price,

            };

            await _context.Product.AddAsync(_product);
            await _context.SaveChangesAsync();

            product.image_list.ForEach(async i =>
            {

                var image = new Image()
                {
                    uid = (int)_product.user_id,
                    description = product.description,
                    product_id = _product.id,
                    photo_url = i

                };

                await _context.images.AddAsync(image);

            });
            await _context.SaveChangesAsync();



            return Ok(_product.id);

        }

        [HttpGet("GetProductByCity")]
        public async Task<IActionResult> GetProductByCity(int city_id)
        {
            var user = await _context.Userr.Where(p => p.city_id.Equals(city_id)).ToListAsync();

            List<Product> products = new List<Product>();

            user.ForEach( u =>
            {

                var product =  _context.Product.Where(p => p.user_id.Equals(u.id)).FirstOrDefault();
                products.Add(product);

            });

            if (products == null)
            {
                return Ok(new ResultProduct() { status = false, message = "Herhangi bir satışınız bulunmamaktadır" });
            }



            var _products = new List<ReturnProduct>();

            products.ForEach(p =>
            {
                var _category = _context.Category.Where(c => c.id.Equals(p.category_id)).FirstOrDefault();

                var rProducts = new ReturnProduct()
                {
                    id = p.id,
                    category_id = p.category_id,
                    user_id = p.user_id,
                    header = p.header,
                    description = p.description,
                    price = p.price,
                    state = p.state,
                    created_date = p.created_date,
                    image_list = getProductImageList((int)p.id),
                    category = _category != null ? _category.name : "",
                };

                _products.Add(rProducts);

            });





            return Ok(new ResultProduct() { productList = _products, status = true, message = "Satışlar başarılı bir şekilde getirildi" });

        }






    }
}
