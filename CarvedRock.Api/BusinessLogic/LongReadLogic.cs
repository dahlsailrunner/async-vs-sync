using System.Text;
using CarvedRock.Api.Repository;

namespace CarvedRock.Api.BusinessLogic;

public interface ILongReadLogic 
{
    Task<string> GetSequentialLongQueryAsync(CancellationToken token = default);
}

public class LongReadLogic : ILongReadLogic
{
    private readonly ICarvedRockRepository _carvedRockRepository;

    public LongReadLogic(ICarvedRockRepository carvedRockRepository)
    {
        _carvedRockRepository = carvedRockRepository;
    }

    public async Task<string> GetSequentialLongQueryAsync(CancellationToken token = default)
    {
        var resultBuilder = new StringBuilder();
        for (var i = 1; i <= 10; i++)
        {
            resultBuilder.Append(await _carvedRockRepository.GetSequentialLongQuery(i, token));
        }
        return resultBuilder.ToString();
    }
}
