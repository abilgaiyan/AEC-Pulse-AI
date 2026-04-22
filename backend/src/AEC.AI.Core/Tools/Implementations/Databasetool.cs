using System.ComponentModel;
using System.Data;
using System.Text;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using AEC.AI.Core.Tools.Core;

namespace AEC.AI.Core.Tools.Implementations;

// ── appsettings.json ──────────────────────────────────────────────────────
// "Tools": {
//   "Database": {
//     "ConnectionString": "Server=...;Database=...;Trusted_Connection=True;",
//     "Provider": "SqlServer",    // SqlServer | SQLite | Mock
//     "MaxRows": 50,
//     "AllowedTables": "products,orders,customers"  // safety: restrict which tables
//   }
// }
//
// NuGet: Microsoft.Data.SqlClient  (for SQL Server)
//        Microsoft.Data.Sqlite     (for SQLite)

public class DatabaseTool(IConfiguration config) : ToolBase
{
    public override string        Name        => "database_query";
    public override string        Description => "Run a read-only SQL SELECT query against the database and return results as JSON";
    public override IList<string> Tags        => ["data", "database", "sql"];

    private readonly string   _connStr       = config["Tools:Database:ConnectionString"] ?? string.Empty;
    private readonly string   _provider      = config["Tools:Database:Provider"]         ?? "Mock";
    private readonly int      _maxRows       = int.TryParse(config["Tools:Database:MaxRows"], out var n) ? n : 50;
    private readonly string[] _allowedTables = (config["Tools:Database:AllowedTables"] ?? string.Empty)
                                                   .Split(',', StringSplitOptions.RemoveEmptyEntries);

    public override AIFunction GetAIFunction() => AIFunctionFactory.Create(
        QueryAsync,
        name:        Name,
        description: Description
    );

    private async Task<string> QueryAsync(
        [Description("A read-only SELECT SQL query")] string sql)
    {
        // Safety: only SELECT allowed
        var trimmed = sql.TrimStart();
        if (!trimmed.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
            return "Error: only SELECT queries are allowed.";

        // Safety: check against allowed tables if configured
        if (_allowedTables.Length > 0)
        {
            var upper = sql.ToUpperInvariant();
            var blocked = _allowedTables.Length > 0 &&
                          !_allowedTables.Any(t => upper.Contains(t.ToUpperInvariant()));
            if (blocked)
                return $"Error: query references tables not in the allowed list: {string.Join(", ", _allowedTables)}";
        }

        if (_provider == "Mock" || string.IsNullOrWhiteSpace(_connStr))
            return MockQuery(sql);

        try
        {
            return await RunSqlServerQueryAsync(sql);
        }
        catch (Exception ex)
        {
            return $"Query failed: {ex.Message}";
        }
    }

    private async Task<string> RunSqlServerQueryAsync(string sql)
    {
        await using var conn = new SqlConnection(_connStr);
        await conn.OpenAsync();

        // Append TOP N if no LIMIT/TOP already present
        var safeSql = sql.Contains("TOP ", StringComparison.OrdinalIgnoreCase)
            ? sql
            : sql.Replace("SELECT ", $"SELECT TOP {_maxRows} ", StringComparison.OrdinalIgnoreCase);

        await using var cmd    = new SqlCommand(safeSql, conn);
        await using var reader = await cmd.ExecuteReaderAsync();

        var rows    = new List<Dictionary<string, object?>>();
        var columns = Enumerable.Range(0, reader.FieldCount)
                                .Select(reader.GetName)
                                .ToList();

        while (await reader.ReadAsync() && rows.Count < _maxRows)
        {
            var row = new Dictionary<string, object?>();
            foreach (var col in columns)
                row[col] = reader[col] is DBNull ? null : reader[col];
            rows.Add(row);
        }

        return JsonSerializer.Serialize(new { columns, rows, rowCount = rows.Count });
    }

    private static string MockQuery(string sql) =>
        JsonSerializer.Serialize(new
        {
            note    = "Mock data — add Tools:Database:ConnectionString for real queries",
            sql,
            columns = new[] { "id", "name", "value" },
            rows    = new[]
            {
                new { id = 1, name = "Sample A", value = 100 },
                new { id = 2, name = "Sample B", value = 200 }
            },
            rowCount = 2
        });
}