using Microsoft.EntityFrameworkCore;

namespace RWA.Web.Application.Services.Seeding
{
    public interface IDatabaseSeeder
    {
        Task SeedAsync();
        Task<bool> IsSeededAsync();
    }
}
