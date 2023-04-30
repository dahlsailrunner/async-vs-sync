using CarvedRock.Core;

namespace CarvedRock.Domain;

public interface IProductLogic 
{
    Task<IEnumerable<ProductModel>> GetProductListForCategoryAsync(string category);
    IEnumerable<ProductModel> GetProductListForCategory(string category);
    Task<ProductModel?> GetProductByIdAsync(int id);
    ProductModel? GetProductById(int id);
}