using EmailService.Application.Handlers;
using EmailService.Application.Interfaces;
using EmailService.Application.Services;
using EmailService.Configuration;
using EmailService.Infrastructure.Persistence;
using EmailService.Infrastructure.Templates;
using EmailService.Worker;
using Microsoft.EntityFrameworkCore;
using RazorLight;

namespace EmailService.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEmailService(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<EmailOptions>(configuration.GetSection("EmailOptions"));
            services.Configure<SmtpOptions>(configuration.GetSection("Smtp"));
            services.Configure<ServiceBusOptions>(configuration.GetSection("ServiceBus"));

            services.AddDbContext<EmailDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("EmailDb")));

            services.AddHostedService<EmailRetryWorker>();

            services.AddSingleton<IRazorTemplateRenderer, RazorTemplateRenderer>();
            services.AddSingleton<ITemplateResolver, TemplateResolver>();

            services.AddScoped<IEmailSendService, EmailSendService>();

            services.AddScoped<IEmailHandler, TransactionalEmailHandler>();
            services.AddScoped<IEmailHandler, VerificationEmailHandler>();

            return services;
        }
    }
}
