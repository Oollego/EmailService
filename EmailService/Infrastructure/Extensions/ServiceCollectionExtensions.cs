using EmailService.Configuration;
using EmailService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EmailService.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEmailService(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<EmailOptions>(configuration.GetSection("EmailOptions"));
            services.Configure<SmtpOptions>(configuration.GetSection("SmtpOptions"));
            services.Configure<ServiceBusOptions>(configuration.GetSection("ServiceBusOptions"));

            services.AddDbContext<EmailDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("EmailDb")));

            return services;
        }
    }
}
