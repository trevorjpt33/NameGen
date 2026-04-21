using NameGen.Core.Models;

namespace NameGen.Core.Interfaces;

public interface IUsernameService
{
    Task<UsernameResponse> GenerateAsync(UsernameRequest request);
}