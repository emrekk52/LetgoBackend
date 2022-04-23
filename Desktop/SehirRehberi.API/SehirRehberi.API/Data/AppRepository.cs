using LetgoEcommerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LetgoEcommerce.Data
{
    public class AppRepository : IAppRepository
    {
        private DataContext _context;

        public AppRepository(DataContext context)
        {
            _context = context;
        }

        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }


        public List<Product> GetProducts()
        {
            List<Product> products = _context.Product.ToList();
            return products;
        }

        public List<City> GetCities()
        {
            List<City> cities = _context.City.ToList();
            return cities;
        }

        public City GetCityById(int id)
        {
            var city = _context.City.Where(c => c.id.Equals(id)).FirstOrDefault();
            return city;
        }

        public List<Product> GetProductsById(int user_id)
        {
            List<Product> products = _context.Product.Where(p => p.user_id.Equals(user_id)).ToList();
            return products;
        }

        public bool SaveAll()
        {
            return _context.SaveChanges() > 0;
        }

        public void Update<T>(T entity) where T : class
        {
            _context.Update(entity);
        }

       
    }
}

