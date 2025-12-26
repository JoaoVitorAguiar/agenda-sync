using AgendaSync.DependencyInjection;
using AgendaSync.Middleware;
using AgendaSync.Routes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

builder.Services.AddOpenApiWithBearer();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddTransient<ExceptionHandlingMiddleware>();


var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseScalarDocs();

app.MapAuthRoutes();
app.MapEventRoutes();
app.Run();
