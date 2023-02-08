using Microsoft.Extensions.Options;

namespace CraftHouse.Web.Options;

public class DatabaseOptionSetup : IConfigureOptions<DatabaseOptions>
{
    private const string ConfigurationSectionName = "DatabaseOptions";
    private readonly IConfiguration _configuration;

    public DatabaseOptionSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(DatabaseOptions options)
    {
        var connectionString = _configuration.GetConnectionString("dev")!;
        options.ConnectionString = connectionString;
        
        _configuration.GetSection(ConfigurationSectionName).Bind(options);
    }

}