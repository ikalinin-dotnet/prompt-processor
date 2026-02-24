using Anthropic.SDK;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using PromptProcessor.Domain;
using PromptProcessor.Infrastructure;
using PromptProcessor.Infrastructure.Consumers;

var builder = WebApplication.CreateBuilder(args);

//Services
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Database
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

//Repository
builder.Services.AddScoped<IPromptJobRepository, PromptJobRepository>();

//LLM - Anthropic
builder.Services.AddSingleton(new AnthropicClient(new APIAuthentication(builder.Configuration["Anthropic:ApiKey"])));

//MassTransit + RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<PromptJobConsumer>();

    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ:Host"] ?? "localhost", "/", h =>
        {
            h.Username(builder.Configuration["RabbitMQ:Username"] ?? "guest");
            h.Password(builder.Configuration["RabbitMQ:Password"] ?? "guest");
        });

        cfg.ConfigureEndpoints(ctx);
    });
});

var app = builder.Build();

//Auto-migrate on startup 
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();
