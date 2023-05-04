using CarvedRock.Api.Data.Entities;

namespace CarvedRock.Api.Repository
{
    public interface ICarvedRockRepository
    {
        Task<List<Product>> GetProductListAsync(string category);
        List<Product> GetProductList(string category);
        Task<Product?> GetProductByIdAsync(int id);
        Product? GetProductById(int id);
        Task<string> GetSequentialLongQuery(int sequenceNumber, CancellationToken token = default);
    }
}
