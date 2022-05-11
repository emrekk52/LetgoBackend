using LetgoEcommerce.Data;
using LetgoEcommerce.Dtos;
using LetgoEcommerce.Models;
using Microsoft.AspNetCore.Cors;
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

                if (p.category_id == 13)
                    rProducts.car_extension = getCarExtension((int)p.id);


                _products.Add(rProducts);

            });




            return Ok(new ResultProduct() { productList = _products, status = true, message = "Satışlar başarılı bir şekilde getirildi" });
        }



        [HttpGet("GetProductById")]
        public async Task<IActionResult> GetProductById(int id)
        {

            var product = await _context.Product.Where(p => p.id.Equals(id) ).OrderBy(p => p.created_date).FirstOrDefaultAsync();


            if (product == null)
            {
                return Ok(new ResultProduct() { status = false, message = "İstenilen ürüne ait detay bulunamadı" });
            }

            var category = await _context.Category.Where(c => c.id.Equals(product.category_id)).FirstOrDefaultAsync();

            var user = await _context.Userr.Where(u => u.id.Equals(product.user_id)).FirstOrDefaultAsync();

            var image = await _context.images.Where(i => i.uid.Equals(user.id) && i.description == "profile").FirstOrDefaultAsync();

            var city = await _context.City.Where(c => c.id.Equals(user.city_id)).FirstOrDefaultAsync();


            var _user = new UserProfile()
            {
                id = user.id,
                name = user.name,
                surname = user.surname,
                city_id = user.city_id,
                email = user.email,
                city = city.city_name,
                photo_url = image.photo_url,
                iframe=city.iframe
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

            if (rProducts.category_id == 13)
                rProducts.car_extension = getCarExtension((int)rProducts.id);

            return Ok(new ResultProduct() { user = _user, product = rProducts, status = true, message = "İstenilen ürüne ait detay başarılı bir şekilde getirildi" });
        }


        public List<string> getProductImageList(int prId)
        {
            var images = _context.images.Where(i => i.product_id.Equals(prId)).ToList();


            if (images == null) return null;

            var stringList = new List<string>();



            images.ForEach(i => stringList.Add(i.photo_url));



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


            if (_product.category_id == 13)
            {
                var car_extension = new CarExtension()
                {
                    product_id = (int)_product.id,
                    car_engine = Convert.ToInt32(product.car_extension.carEngine.Substring(0, product.car_extension.carEngine.Length - 1)),
                    car_color = product.car_extension.carColor,
                    car_fuel = product.car_extension.carFuel,
                    car_gear = product.car_extension.carGear,
                    car_model = product.car_extension.carYear.ToString(),
                    car_traction = product.car_extension.carTraction,
                    car_type = product.car_extension.carType,
                    km=(int)product.car_extension.carKm
                };
                await _context.Car_Extension.AddAsync(car_extension);
                


            }

await _context.SaveChangesAsync();



            return Ok(_product.id);

        }

        [HttpGet("GetProductByCity")]
        public async Task<IActionResult> GetProductByCity(int city_id, int user_id)
        {

            var maxSize = 20;

            var user = await _context.Userr.Take(maxSize).Where(p => p.city_id.Equals(city_id) && !p.id.Equals(user_id)).ToListAsync();

            List<Product> products = new List<Product>();

            var city = await _context.City.Where(c => c.id.Equals(city_id)).FirstOrDefaultAsync();


            user.ForEach(u =>
           {
               var productList = _context.Product.Where(p => p.user_id.Equals(u.id) && p.state.Equals(0)).ToList();
               productList.ForEach(product =>
               {
                   products.Add(product);
               });


           });

            if (products == null)
            {
                return Ok(new ResultProduct() { status = false, message = city.city_name + " şehrine ait satış bulunamadı" });
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
                    city_name = city.city_name
                };

                if (p.category_id == 13)
                    rProducts.car_extension = getCarExtension((int)p.id);

                _products.Add(rProducts);

            });





            return Ok(new ResultProduct() { productList = _products, status = true, message = "Satışlar başarılı bir şekilde getirildi" });

        }


        [HttpGet("GetProductByCategory")]
        public async Task<IActionResult> GetProductByCategory(int category_id, int user_id, int size)
        {



            var products = await _context.Product.Take(size).Where(p => p.category_id.Equals(category_id) && !p.user_id.Equals(user_id) && p.state.Equals(0)).ToListAsync();

            var category = await _context.Category.Where(c => c.id.Equals(category_id)).FirstOrDefaultAsync();


            if (category == null)
            {
                return Ok(new ResultProduct() { status = false, message = "Böyle bir kategori bulunamadı" });
            }

            if (products == null || products.Count == 0)
            {
                return Ok(new ResultProduct() { status = false, message = category.name + " kategorisinde satış bulunamadı" });
            }


            var _products = new List<ReturnProduct>();

            products.ForEach(p =>
            {

                var user = _context.Userr.Where(u => u.id.Equals(p.user_id)).FirstOrDefault();
                var city = _context.City.Where(c => c.id.Equals(user.city_id)).FirstOrDefault();

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
                    category = category.name,
                    city_name = city.city_name
                };

                if (p.category_id == 13)
                    rProducts.car_extension = getCarExtension((int)p.id);

                _products.Add(rProducts);

            });



            return Ok(new ResultProduct() { productList = _products, status = true, message = "Satışlar başarılı bir şekilde getirildi" });

        }




        [HttpGet("GetProductByRandom")]
        public async Task<IActionResult> GetProductByRandom(int user_id, int size)
        {

            var _size = size < 10 ? 20 : size;

            var products = await _context.Product.Where(p => !p.user_id.Equals(user_id) && p.state.Equals(0)).Take((int)_size).OrderBy(c => Guid.NewGuid()).ToListAsync();

            var _products = new List<ReturnProduct>();

            products.ForEach(p =>
            {

                var _category = _context.Category.Where(c => c.id.Equals(p.category_id)).FirstOrDefault();
                var user = _context.Userr.Where(u => u.id.Equals(p.user_id)).FirstOrDefault();
                var city = _context.City.Where(c => c.id.Equals(user.city_id)).FirstOrDefault();


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
                    category = _category.name,
                    city_name = city.city_name
                };

                if (p.category_id == 13)
                    rProducts.car_extension = getCarExtension((int)p.id);

                _products.Add(rProducts);

            });



            return Ok(new ResultProduct() { productList = _products, status = true, message = "Sizin için seçtiklerimiz başarılı bir şekilde getirildi" });

        }



        public ReturnCarExtension getCarExtension(int product_id)
        {

            var extension = _context.Car_Extension.Where(pr => pr.product_id.Equals(product_id)).FirstOrDefault();


            var rt = new ReturnCarExtension
            {
                carColor = extension.car_color,
                carEngine = extension.car_engine + "+",
                carFuel = extension.car_fuel,
                carGear = extension.car_gear,
                carKm = extension.km,
                carTraction = extension.car_traction,
                carType = extension.car_type,
                carYear = Convert.ToInt32(extension.car_model)
            };

            return rt;

        }


    }
}
