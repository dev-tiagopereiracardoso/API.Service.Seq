using Serilog;
using Serilog.Events;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;

var seqUri = builder.Configuration["SEQ_URI"]!;
if (string.IsNullOrEmpty(seqUri))
    throw new ArgumentException($"Falha ao iniciar {assemblyName}. Parâmetro SEQ_URI não encontrado.");

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyOrigin",
        policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

Log.Logger = new LoggerConfiguration()
    .MinimumLevel
    .Override("Microsoft", LogEventLevel.Warning)
    .Enrich
    .FromLogContext()
    .WriteTo
    .Seq(seqUri)
    .WriteTo
    .Console()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseExceptionHandler("/Home/Error");

app.UseHsts();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

Log.Information("Start Service {assemblyName}", assemblyName);
app.Run();
Log.Information("Stop Service {assemblyName}", assemblyName);
