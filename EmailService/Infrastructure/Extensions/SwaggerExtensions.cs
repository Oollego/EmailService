using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EmailService.Infrastructure.Extensions
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddSwaggerForDevelopment(this IServiceCollection services, IWebHostEnvironment environment)
        {
            if(environment.IsDevelopment())
            {
                services.AddSwaggerGen(c =>
                {
                    // Можно добавить описание всего API
                    c.SwaggerDoc("default", new Microsoft.OpenApi.Models.OpenApiInfo
                    {
                        Title = "EmailService API",
                        Description = "API для отправки и обработки email сообщений",
                    });
                    // Подключаем XML комментарии для моделей и контроллеров (по желанию)
                    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    if (File.Exists(xmlPath))
                        c.IncludeXmlComments(xmlPath);
                });
            }

            return services;

        }

        public static IApplicationBuilder UseSwaggerForDevelopment(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/default/swagger.json", "EmailService API");
                    c.RoutePrefix = ""; // Swagger UI в корне
                });
            }

            return app;
        }
    }
}
