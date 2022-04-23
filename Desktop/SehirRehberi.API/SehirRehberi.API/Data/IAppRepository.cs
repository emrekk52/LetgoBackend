using LetgoEcommerce.Models;
using System.Collections.Generic;

namespace LetgoEcommerce.Data
{
    public interface IAppRepository
    {
        void Add<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        void Update<T>(T entity) where T : class;
        bool SaveAll();

        List<Product> GetProducts();
        List<Product> GetProductsById(int user_id);
        List<City> GetCities();
        City GetCityById(int id);

        
    }
}
