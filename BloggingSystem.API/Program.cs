using BloggingSystem.API;
using BloggingSystem.API.Filters;
using BloggingSystem.Application;
using BloggingSystem.Infrastructure;
using BloggingSystem.Infrastructure.Authentication;
using BloggingSystem.Infrastructure.Middlewares;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCompressionServices();
builder.Services.AddHttpContextAccessor();
builder.Services.AddApiExceptionFilter();
builder.Services.AddApplicationServices();
builder.Services.AddMemoryCache();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddPasswordHasher();


// Add authentication and authorization services
builder.Services.AddAuthenticationServices(builder.Configuration);
builder.Services.AddAuthorizationServices();
builder.Services.AddControllers();

// Add API versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Ghi ra file
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Blogging System API", 
        Version = "v1",
        Description = "REST API for Blogging System"
    });
    
    // Định nghĩa security scheme cho JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
    
});

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder
            .WithOrigins("http://localhost:5173") // Replace with your frontend URL
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Blogging System API v1");
        c.RoutePrefix = string.Empty; // Để Swagger UI hiển thị ở root URL
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
        c.DefaultModelsExpandDepth(-1); // Ẩn schema section
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigin");

app.UseResponseCompression();

app.UseAuditLogging();

app.UsePostViewTracking();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();