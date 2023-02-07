using CraftHouse.Web.Data;
using CraftHouse.Web.Infrastructure;
using CraftHouse.Web.Options;
using CraftHouse.Web.Repositories;
using CraftHouse.Web.Services;
using CraftHouse.Web.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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
    
    services.ConfigureOptions<DatabaseOptionSetup>();

    services.AddDbContext<AppDbContext>((serviceProvider, options)=>
    {
        var databaseOptions = serviceProvider.GetService<IOptions<DatabaseOptions>>()!.Value;
        
        options.UseSqlServer(databaseOptions.ConnectionString, sqlServerAction =>
        {
            sqlServerAction.EnableRetryOnFailure(databaseOptions.MaxRetryCount);
            sqlServerAction.CommandTimeout(databaseOptions.CommandTimeout);
        });
        options.EnableDetailedErrors(databaseOptions.EnableDetailedErrors);
        options.EnableSensitiveDataLogging(databaseOptions.EnableSensitiveDataLogging);
    });

    services.AddSession(options => { options.Cookie.Name = "SessionID"; });
    services.AddMemoryCache();

    services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

    services.AddTransient<IAuthService, AuthService>();
    services.AddTransient<IUserRepository, UserRepository>();
    services.AddTransient<ICategoryRepository, CategoryRepository>();
    services.AddValidatorsFromAssemblyContaining<UserValidator>();

    services.Configure<RouteOptions>(options =>
    {
        options.LowercaseUrls = true;
        options.LowercaseQueryStrings = true;
    });

    services.AddRazorPages().AddMvcOptions(options =>
    {
        options.Filters.Add<AuthFilter>();
    });

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
catch (HostAbortedException)
{
   // It handles exception thrown while doing `dotnet ef` related actions 
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated!");
}
finally
{
    Log.CloseAndFlush();
}