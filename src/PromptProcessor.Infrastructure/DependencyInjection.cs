using Anthropic.SDK;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PromptProcessor.Application.Services;
using PromptProcessor.Domain;
using PromptProcessor.Infrastructure.Consumers;
using PromptProcessor.Infrastructure.Data;
using PromptProcessor.Infrastructure.Repositories;
using PromptProcessor.Infrastructure.Services;

namespace PromptProcessor.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IPromptJobRepository, PromptJobRepository>();

        services.AddSingleton(new AnthropicClient(new APIAuthentication(configuration["Anthropic:ApiKey"])));
        services.AddScoped<ILlmService, AnthropicLlmService>();

        services.AddMassTransit(x =>
        {
            x.AddConsumer<PromptJobConsumer>();

            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(configuration["RabbitMQ:Host"] ?? "localhost", "/", h =>
                {
                    h.Username(configuration["RabbitMQ:Username"] ?? "guest");
                    h.Password(configuration["RabbitMQ:Password"] ?? "guest");
                });

                cfg.ConfigureEndpoints(ctx);
            });
        });

        return services;
    }

    public static void ApplyMigrations(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
    }
}
