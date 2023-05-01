using CarvedRock.Api.Models;

namespace CarvedRock.Api.BusinessLogic;

public interface IProductLogic 
{
    Task<IEnumerable<ProductModel>> GetProductListForCategoryAsync(string category);
    IEnumerable<ProductModel> GetProductListForCategory(string category);
    Task<ProductModel?> GetProductByIdAsync(int id);
    ProductModel? GetProductById(int id);
}