using AgendaSync.DependencyInjection;
using AgendaSync.Middleware;
using AgendaSync.Routes;

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

app.Run();
