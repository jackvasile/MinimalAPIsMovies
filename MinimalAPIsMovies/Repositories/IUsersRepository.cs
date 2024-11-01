using Microsoft.AspNetCore.Identity;

namespace MinimalAPIsMovies.Repositories
{
    public interface IUsersRepository
    {
        Task<string> Create(IdentityUser user);
        Task<IdentityUser?> GetByEmail(string normalizedEmail);
    }
}