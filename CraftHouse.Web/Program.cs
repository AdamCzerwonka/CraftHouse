using System.Globalization;
using CraftHouse.Web.Data;
using CraftHouse.Web.Infrastructure;
using CraftHouse.Web.Options;
using CraftHouse.Web.Repositories;
using CraftHouse.Web.Services;
using CraftHouse.Web.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Localization;
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

    services.AddHttpContextAccessor();

    services.AddTransient<IAuthService, AuthService>();
    services.AddTransient<IUserRepository, UserRepository>();
    services.AddTransient<ICategoryRepository, CategoryRepository>();
    services.AddTransient<IProductRepository, ProductRepository>();
    services.AddTransient<IOptionRepository, OptionRepository>();
    services.AddTransient<IOptionValueRepository, OptionValueRepository>();
    services.AddTransient<ICartService, CartService>();
    services.AddTransient<IOrderRepository, OrderRepository>();
    services.AddTransient<IOrderItemRepository, OrderItemRepository>();
    services.AddTransient<IOrderItemOptionRepository, OrderItemOptionRepository>();
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

    app.UseRouting();

    app.UseSession();
    app.UseAuthorization();

    app.UseRequestLocalization(options =>
    {
        var cultureInfo = new CultureInfo("en-US");
        options.DefaultRequestCulture = new RequestCulture(cultureInfo);
        options.SupportedCultures = new List<CultureInfo>
        {
            cultureInfo
        };
    });

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