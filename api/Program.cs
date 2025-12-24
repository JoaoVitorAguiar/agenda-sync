using AgendaSync.Data;
using AgendaSync.Routes;
using AgendaSync.Services;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddCors();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IJwtProvider>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();

    return new JwtProvider(
        secretKey: config["Jwt:SecretKey"]!,
        issuer: config["Jwt:Issuer"]!,
        audience: config["Jwt:Audience"]!,
        expiryInMinutes: int.Parse(config["Jwt:ExpiryInMinutes"]!)
    );
});

builder.Services.AddScoped<IAuthService, GoogleAuthService>();
builder.Services.AddDbContext<AgendaSyncDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);
builder.Services.AddScoped<IUserRepository, UserRepository>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/api-docs");

}

app.UseHttpsRedirection();
app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

// Map routes
app.MapAuthRoutes();

app.Run();
