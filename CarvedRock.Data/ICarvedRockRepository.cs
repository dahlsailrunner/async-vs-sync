using CarvedRock.Data.Entities;

namespace CarvedRock.Data
{
    public interface ICarvedRockRepository
    {
        Task<List<Product>> GetProductListAsync(string category);
        List<Product> GetProductList(string category);
        Task<Product?> GetProductByIdAsync(int id);
        Product? GetProductById(int id);
    }
}
