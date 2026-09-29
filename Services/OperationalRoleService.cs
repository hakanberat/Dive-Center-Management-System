using Dapper;
using DiveCenterManager.Data;
using DiveCenterManager.Models;

namespace DiveCenterManager.Services;

public class OperationalRoleService
{
    private readonly SqlConnectionFactory _connectionFactory;

    public OperationalRoleService(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<OperationalRole>> GetAllAsync(
        string? searchTerm = null)
    {
        const string sql = """
            SELECT
                Id,
                Name,
                Description,
                IsActive
            FROM Roles
            WHERE
                @SearchTerm IS NULL
                OR Name LIKE '%' + @SearchTerm + '%'
                OR Description LIKE '%' + @SearchTerm + '%'
            ORDER BY Name;
            """;

        using var connection = _connectionFactory.CreateConnection();

        return await connection.QueryAsync<OperationalRole>(
            sql,
            new
            {
                SearchTerm = string.IsNullOrWhiteSpace(searchTerm)
                    ? null
                    : searchTerm.Trim()
            });
    }

    public async Task<IEnumerable<OperationalRole>> GetActiveAsync()
    {
        const string sql = """
            SELECT
                Id,
                Name,
                Description,
                IsActive
            FROM Roles
            WHERE IsActive = 1
            ORDER BY Name;
            """;

        using var connection = _connectionFactory.CreateConnection();

        return await connection.QueryAsync<OperationalRole>(sql);
    }

    public async Task<OperationalRole?> GetByIdAsync(int id)
    {
        const string sql = """
            SELECT
                Id,
                Name,
                Description,
                IsActive
            FROM Roles
            WHERE Id = @Id;
            """;

        using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<OperationalRole>(
            sql,
            new { Id = id });
    }

    public async Task<int> CreateAsync(OperationalRole role)
    {
        const string sql = """
            INSERT INTO Roles
            (
                Name,
                Description,
                IsActive
            )
            VALUES
            (
                @Name,
                @Description,
                @IsActive
            );

            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;

        using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleAsync<int>(sql, role);
    }

    public async Task UpdateAsync(OperationalRole role)
    {
        const string sql = """
            UPDATE Roles
            SET
                Name = @Name,
                Description = @Description,
                IsActive = @IsActive
            WHERE Id = @Id;
            """;

        using var connection = _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(sql, role);
    }

   public async Task ToggleActiveStatusAsync(int id)
{
    const string sql = """
        UPDATE Roles
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