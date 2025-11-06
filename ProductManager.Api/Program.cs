global using FluentValidation;

using FastEndpoints;
using FastEndpoints.Swagger;
using ProductManager.Api.Middleware;
using ProductManager.Application.Endpoints.Product.Create;
using ProductManager.Application.Endpoints.Product.List;
using ProductManager.Infrastructure.IoC;

var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddOpenApi();

builder.Services.RegisterInfrastructureDependencies(builder.Configuration);

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseFastEndpoints()
    .UseSwaggerGen();

await app.RunAsync();