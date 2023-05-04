using CarvedRock.Api.BusinessLogic;
using Microsoft.AspNetCore.Mvc;

namespace CarvedRock.Api.Controllers;
[Route("[controller]")]
[ApiController]
public class LongReadController : ControllerBase
{
    private readonly ILongReadLogic _longReadLogic;

    public LongReadController(ILongReadLogic longReadLogic)
    {
        _longReadLogic = longReadLogic;
    }

    [HttpGet]
    public async Task<string> Get(bool includeCancellation, CancellationToken token)
    {
        if (!includeCancellation)
        {
            token = CancellationToken.None;
        }
        return await _longReadLogic.GetSequentialLongQueryAsync(token);
    }
}
