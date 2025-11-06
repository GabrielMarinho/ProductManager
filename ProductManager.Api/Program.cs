global using FluentValidation;

using FastEndpoints;
using FastEndpoints.Swagger;
using ProductManager.Api.BackgroundService;
using ProductManager.Api.Middleware;
using ProductManager.Application.Endpoints.Product.Create;
using ProductManager.Infrastructure.IoC;

var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddOpenApi();

builder.Services.RegisterInfrastructureDependencies(builder.Configuration);

builder.Services.AddHostedService<ProductCreatedConsumer>();

builder.Services
    .AddFastEndpoints(options =>
    {
        options.Assemblies = new[]
        {
            typeof(ProductCreateHandler).Assembly
        };
    })
    .SwaggerDocument();

builder.Services.AddExceptionHandler<GlobalHandlingMiddleware>();
builder.Services.AddProblemDetails();

// CORS: disable restrictions by allowing any origin, header, and method
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.MapOpenApi();
}

await app.Services.ExecuteDataBaseMigrationAsync();

app.UseHttpsRedirection();

app.UseCors();

app.UseFastEndpoints()
    .UseSwaggerGen();

await app.RunAsync();