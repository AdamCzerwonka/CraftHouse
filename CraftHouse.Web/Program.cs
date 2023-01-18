using CraftHouse.Web.Data;
using CraftHouse.Web.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog((context, services, configuration) =>
    {
        configuration.ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services);
    });

    var services = builder.Services;
    var configuration = builder.Configuration;

    services.AddDbContext<AppDbContext>(options =>
    {
        options.UseSqlServer(configuration.GetConnectionString("dev"));
        options.EnableSensitiveDataLogging();
    });

    services.AddSession(options =>
    {
        options.Cookie.Name = "SessionID";
    } );
    services.AddMemoryCache();

    services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

    services.AddTransient<IAuthService, AuthService>();

    services.AddRazorPages();

    var app = builder.Build();
    app.UseSerilogRequestLogging();

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseSession();

    app.UseRouting();

    app.UseAuthorization();

    app.MapRazorPages();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated!");
}
finally
{
    Log.CloseAndFlush();
}