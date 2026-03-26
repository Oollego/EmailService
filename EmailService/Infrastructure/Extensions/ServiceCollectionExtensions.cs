using EmailService.Application.Handlers;
using EmailService.Application.Interfaces;
using EmailService.Application.Services;
using EmailService.Configuration;
using EmailService.Infrastructure.Persistence;
using EmailService.Infrastructure.ServiceBus;
using EmailService.Infrastructure.Templates;
using EmailService.Worker;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;

namespace EmailService.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEmailService(this IServiceCollection services, IConfiguration configuration)
        {
            // Options
            services.Configure<EmailOptions>(configuration.GetSection("EmailOptions"));
            services.Configure<SmtpOptions>(configuration.GetSection("Smtp"));
            services.Configure<ServiceBusOptions>(configuration.GetSection("ServiceBus"));
            services.Configure<TemplateOptions>(configuration.GetSection("Templates"));

            services.Configure<JsonOptions>(options =>
            {
                options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            // DbContext
            services.AddDbContext<EmailDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("EmailDb")));

            // Repositories
            services.AddScoped<IEmailRepository, EmailRepository>();

            // Email Processor
            services.AddScoped<IEmailProcessorService, EmailProcessorService>();

            // Email send service
            services.AddScoped<IEmailSendService, EmailSendService>();

            // Email Handlers
            services.AddScoped<IEmailHandler, TransactionHandler>();
            services.AddScoped<IEmailHandler, VerificationHandler>();

            // Templates (Singleton)
            services.AddSingleton<IRazorTemplateRenderer, RazorTemplateRenderer>();
            services.AddSingleton<ITemplateResolver, TemplateResolver>();

            // ServiceBusClient (Singleton)
            services.AddSingleton(sp =>
            {
                var options = sp.GetRequiredService<IOptions<ServiceBusOptions>>().Value;
                return new Azure.Messaging.ServiceBus.ServiceBusClient(options.ConnectionString);
            });

            //ServiceBus
            services.AddScoped<IMessageBus, ServiceBusPublisher>();

            // Hosted Services
            services.AddHostedService<EmailRetryWorker>();
            services.AddHostedService<EmailWorker>();

            return services;
        }
    }
}
