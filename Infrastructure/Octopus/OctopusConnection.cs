using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace DataAnalysis.Infrastructure.Octopus;

public class OctopusConnection
{
    // TEMPLATE: Name of the main fact/source table the repositories read from.
    // Replace with your own schema-qualified table name (e.g. "dbo.YOUR_FACT_TABLE").
    public const string PolicyTable = "dbo.YOUR_FACT_TABLE";

    private readonly string _connectionString;

    public OctopusConnection(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("OctopusConnection")
            ?? throw new InvalidOperationException("OctopusConnection string is not configured.");
    }

    public IDbConnection Create() => new SqlConnection(_connectionString);
}