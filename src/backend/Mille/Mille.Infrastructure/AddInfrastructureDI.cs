using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mille.Application.Common.Interfaces;
using Mille.Infrastructure.Persistence.Data;
using Mille.Infrastructure.Persistence.Repositories;
using Mille.Infrastructure.Security;
using Mille.Infrastructure.Services;

namespace Mille.Infrastructure
{
    public static class AddInfrastructureDI
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<ITokenGenerator, JwtTokenGenerator>();
            services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
            services.AddScoped<IFileUploadService, CloudinaryService>();

            return services;
        }
    }
}
