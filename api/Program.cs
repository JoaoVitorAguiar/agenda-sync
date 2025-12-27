using AgendaSync.Data;
using AgendaSync.DependencyInjection;
using AgendaSync.Middleware;
using AgendaSync.Routes;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

builder.Services.AddOpenApiWithBearer();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddTransient<ExceptionHandlingMiddleware>();
builder.Services.AddHttpClient("GoogleOAuth", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Google:TokenEndpoint"]!);
});
builder.Services.AddHttpClient("GoogleCalendar", client =>
{
    client.BaseAddress = new Uri("https://www.googleapis.com/calendar/v3/calendars/");
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("webapp", b =>
        b.WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()!)
         .AllowCredentials()
         .AllowAnyHeader()
         .AllowAnyMethod()
    );
});

var app = builder.Build();

app.UseCors("webapp");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseScalarDocs();

app.MapAuthRoutes();
app.MapEventRoutes();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AgendaSyncDbContext>();
    db.Database.Migrate();
}

app.Run();
