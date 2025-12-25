using System.Text;
using AgendaSync.Api.OpenApi;
using AgendaSync.Data;
using AgendaSync.Routes;
using AgendaSync.Security;
using AgendaSync.Services;
using AgendaSync.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});
builder.Services.AddCors();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

var jwt = builder.Configuration.GetSection("Jwt");
var secretKey = jwt["SecretKey"]!;
var issuer = jwt["Issuer"]!;
var audience = jwt["Audience"]!;
var expiry = int.Parse(jwt["ExpiryInMinutes"]!);

builder.Services.AddSingleton<IJwtProvider>(
    _ => new JwtProvider(secretKey, issuer, audience, expiry)
);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,

            ValidateAudience = true,
            ValidAudience = audience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secretKey)
            ),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddScoped<AuthUser>(sp =>
{
    var httpContext = sp.GetRequiredService<IHttpContextAccessor>().HttpContext;
    if (httpContext == null)
        throw new UnauthorizedAccessException("HttpContext is not available.");
    return new AuthUser(httpContext.User);
});
builder.Services.AddAuthorization();
builder.Services.AddDbContext<AgendaSyncDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);
builder.Services.AddScoped<IAuthService, GoogleAuthService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/api-docs", options => options
    .AddPreferredSecuritySchemes("BearerAuth")
    .AddHttpAuthentication("BearerAuth", auth =>
    {
        auth.Token = "";
    }));
}


app.UseHttpsRedirection();
app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseAuthentication();
app.UseAuthorization();

// Map routes
app.MapAuthRoutes();
app.MapEventRoutes();

app.Run();
