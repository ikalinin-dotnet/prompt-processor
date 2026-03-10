using PromptProcessor.Application;
using PromptProcessor.Infrastructure;
using PromptProcessor.Infrastructure.Data;
using PromptProcessor.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseExceptionHandling();
app.UseCors();

PromptProcessor.Infrastructure.DependencyInjection.ApplyMigrations(app.Services);

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();
