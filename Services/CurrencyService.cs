using Dapper;
using DiveCenterManager.Data;
using DiveCenterManager.Models;

namespace DiveCenterManager.Services;

public class CurrencyService
{
    private readonly SqlConnectionFactory _connectionFactory;

    public CurrencyService(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<Currency>> GetAllAsync(
        string? searchTerm = null)
    {
        const string sql = """
            SELECT
                Id,
                Code,
                Name,
                Symbol,
                IsActive
            FROM Currencies
            WHERE
                @SearchTerm IS NULL
                OR Code LIKE '%' + @SearchTerm + '%'
                OR Name LIKE '%' + @SearchTerm + '%'
                OR Symbol LIKE '%' + @SearchTerm + '%'
            ORDER BY Code;
            """;

        using var connection = _connectionFactory.CreateConnection();

        return await connection.QueryAsync<Currency>(
            sql,
            new
            {
                SearchTerm = string.IsNullOrWhiteSpace(searchTerm)
                    ? null
                    : searchTerm.Trim()
            });
    }

    public async Task<IEnumerable<Currency>> GetActiveAsync()
    {
        const string sql = """
            SELECT
                Id,
                Code,
                Name,
                Symbol,
                IsActive
            FROM Currencies
            WHERE IsActive = 1
            ORDER BY Code;
            """;

        using var connection = _connectionFactory.CreateConnection();

        return await connection.QueryAsync<Currency>(sql);
    }

    public async Task<Currency?> GetByIdAsync(int id)
    {
        const string sql = """
            SELECT
                Id,
                Code,
                Name,
                Symbol,
                IsActive
            FROM Currencies
            WHERE Id = @Id;
            """;

        using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<Currency>(
            sql,
            new { Id = id });
    }

    public async Task<int> CreateAsync(Currency currency)
    {
        const string sql = """
            INSERT INTO Currencies
            (
                Code,
                Name,
                Symbol,
                IsActive
            )
            VALUES
            (
                @Code,
                @Name,
                @Symbol,
                @IsActive
            );

            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;

        using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleAsync<int>(
            sql,
            currency);
    }

    public async Task UpdateAsync(Currency currency)
    {
        const string sql = """
            UPDATE Currencies
            SET
                Code = @Code,
                Name = @Name,
                Symbol = @Symbol,
                IsActive = @IsActive
            WHERE Id = @Id;
            """;

        using var connection = _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(
            sql,
            currency);
    }

    public async Task ToggleActiveStatusAsync(int id)
    {
        const string sql = """
            UPDATE Currencies
            SET IsActive =
                CASE
                    WHEN IsActive = 1 THEN 0
                    ELSE 1
                END
            WHERE Id = @Id;
            """;

        using var connection = _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(
            sql,
            new { Id = id });
    }
}