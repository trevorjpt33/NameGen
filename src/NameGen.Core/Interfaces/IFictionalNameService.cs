using NameGen.Core.Models;

namespace NameGen.Core.Interfaces;

public interface IFictionalNameService
{
    Task<FictionalNameResponse> GenerateAsync(FictionalNameRequest request);
}