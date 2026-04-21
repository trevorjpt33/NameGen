using NameGen.Core.Models;

namespace NameGen.Core.Interfaces;

public interface IHumanNameService
{
    Task<HumanNameResponse> GenerateAsync(HumanNameRequest request);
}