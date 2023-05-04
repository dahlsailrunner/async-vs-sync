using CarvedRock.Api.Data;
using CarvedRock.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace CarvedRock.Api.Repository;

public class CarvedRockRepository : ICarvedRockRepository
{
    private readonly LocalContext _ctx;

    public CarvedRockRepository(LocalContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<List<Product>> GetProductListAsync(string category)
    {
        await Task.Delay(400); // simulates heavy query
        return await _ctx.Products.Where(p => p.Category == category || category == "all")
            .ToListAsync();
    }

    public List<Product> GetProductList(string category)
    {
        Thread.Sleep(400); // simulates heavy query
        return _ctx.Products.Where(p => p.Category == category || category == "all")
            .ToList();
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        await Task.Delay(50); // simulates heavy query
        return await _ctx.Products.FindAsync(id);
    }

    public Product? GetProductById(int id)
    {
        Thread.Sleep(50); // simulates heavy query
        return _ctx.Products.Find(id);
    }

    public async Task<string> GetSequentialLongQuery(int sequenceNumber, CancellationToken token = default)
    {
        await Task.Delay(1000, token); // simulates long single query
        Log.Information($"Query {sequenceNumber} completed.");
        return $"Query {sequenceNumber} completed.\n";
    }
}
