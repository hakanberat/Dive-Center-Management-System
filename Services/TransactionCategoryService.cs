using Dapper;
using DiveCenterManager.Data;
using DiveCenterManager.Models;

namespace DiveCenterManager.Services;

public class TransactionCategoryService
{
    private readonly SqlConnectionFactory _connectionFactory;

    public TransactionCategoryService(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<TransactionCategory>> GetAllAsync(
        string? searchTerm = null)
    {
        const string sql = """
            SELECT
                Id,
                Name,
                Type,
                Description,
                IsActive
            FROM TransactionCategories
            WHERE
                @SearchTerm IS NULL
                OR Name LIKE '%' + @SearchTerm + '%'
                OR Type LIKE '%' + @SearchTerm + '%'
                OR Description LIKE '%' + @SearchTerm + '%'
            ORDER BY
                Type,
                Name;
            """;

        using var connection = _connectionFactory.CreateConnection();

        return await connection.QueryAsync<TransactionCategory>(
            sql,
            new
            {
                SearchTerm = string.IsNullOrWhiteSpace(searchTerm)
                    ? null
                    : searchTerm.Trim()
            });
    }
    public async Task<IEnumerable<TransactionCategory>> GetActiveAsync()
{
    const string sql = """
        SELECT
            Id,
            Name,
            Type,
            Description,
            IsActive
        FROM TransactionCategories
        WHERE IsActive = 1
        ORDER BY
            Type,
            Name;
        """;

    using var connection = _connectionFactory.CreateConnection();

    return await connection.QueryAsync<TransactionCategory>(sql);
}

    public async Task<TransactionCategory?> GetByIdAsync(int id)
    {
        const string sql = """
            SELECT
                Id,
                Name,
                Type,
                Description,
                IsActive
            FROM TransactionCategories
            WHERE Id = @Id;
            """;

        using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<TransactionCategory>(
            sql,
            new { Id = id });
    }

    public async Task<int> CreateAsync(TransactionCategory category)
    {
        const string sql = """
            INSERT INTO TransactionCategories
            (
                Name,
                Type,
                Description,
                IsActive
            )
            VALUES
            (
                @Name,
                @Type,
                @Description,
                @IsActive
            );

            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;

        using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleAsync<int>(
            sql,
            category);
    }

    public async Task UpdateAsync(TransactionCategory category)
    {
        const string sql = """
            UPDATE TransactionCategories
            SET
                Name = @Name,
                Type = @Type,
                Description = @Description,
                IsActive = @IsActive
            WHERE Id = @Id;
            """;

        using var connection = _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(
            sql,
            category);
    }

    public async Task ToggleActiveStatusAsync(int id)
    {
        const string sql = """
            UPDATE TransactionCategories
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