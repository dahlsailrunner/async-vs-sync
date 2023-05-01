using CarvedRock.Api.BusinessLogic;
using CarvedRock.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace CarvedRock.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class SyncProductController : ControllerBase
{
    private readonly ILogger<ProductController> _logger;
    private readonly IProductLogic _productLogic;   

    
    public SyncProductController(ILogger<ProductController> logger, IProductLogic productLogic)
    {
        _logger = logger;
        _productLogic = productLogic;
    }

    [HttpGet]
    public IEnumerable<ProductModel> Get(string category = "all")
    {
        using (_logger.BeginScope("ScopeCat: {ScopeCat}", category))
        {     
            _logger.LogInformation( "Getting products in API.");
            return _productLogic.GetProductListForCategory(category);
        }
    }
}