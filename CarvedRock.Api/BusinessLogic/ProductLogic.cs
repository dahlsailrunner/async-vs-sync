using CarvedRock.Api.Data.Entities;
using CarvedRock.Api.Models;
using CarvedRock.Api.Repository;
using CarvedRock.Data;

namespace CarvedRock.Api.BusinessLogic;

public class ProductLogic : IProductLogic
{
    private readonly ICarvedRockRepository _repo;

    public ProductLogic(ICarvedRockRepository repo)
    {
        _repo = repo;
    }

    public async Task<ProductModel?> GetProductByIdAsync(int id)
    {
        var product = await _repo.GetProductByIdAsync(id);
        return product != null ? ConvertToProductModel(product) : null;
    }

    public async Task<IEnumerable<ProductModel>> GetProductListForCategoryAsync(string category)
    {
        var products =  await _repo.GetProductListAsync(category);

        var results = new List<ProductModel>();
        foreach (var product in products)
        {
            var productToAdd = ConvertToProductModel(product);
            results.Add(productToAdd);
        }

        return results;
    }

    public IEnumerable<ProductModel> GetProductListForCategory(string category)
    {
        var products = _repo.GetProductList(category);

        var results = new List<ProductModel>();
        foreach (var product in products)
        {
            var productToAdd = ConvertToProductModel(product);
            results.Add(productToAdd);
        }

        return results;
    }

    public ProductModel? GetProductById(int id)
    {
        var product = _repo.GetProductById(id);
        return product != null ? ConvertToProductModel(product) : null;
    }

    private static ProductModel ConvertToProductModel(Product product)
    {
        var productToAdd = new ProductModel
        {
            Id = product.Id,
            Category = product.Category,
            Description = product.Description,
            ImgUrl = product.ImgUrl,
            Name = product.Name,
            Price = product.Price
        };
        var rating = product.Rating;
        if (rating != null)
        {
            productToAdd.Rating = rating.AggregateRating;
            productToAdd.NumberOfRatings = rating.NumberOfRatings;
        }

        return productToAdd;
    } }