using Dapper;
using DiveCenterManager.Data;
using DiveCenterManager.Models;

namespace DiveCenterManager.Services;

public class EquipmentService
{
    private readonly SqlConnectionFactory _connectionFactory;

    public EquipmentService(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<Equipment>> GetAllAsync(
        string? searchTerm = null)
    {
        const string sql = """
            SELECT
                Id,
                Name,
                Category,
                SerialNumber,
                Brand,
                Model,
                Quantity,
                Notes,
                IsActive
            FROM Equipment
            WHERE
                @SearchTerm IS NULL
                OR Name LIKE '%' + @SearchTerm + '%'
                OR Category LIKE '%' + @SearchTerm + '%'
                OR SerialNumber LIKE '%' + @SearchTerm + '%'
                OR Brand LIKE '%' + @SearchTerm + '%'
                OR Model LIKE '%' + @SearchTerm + '%'
            ORDER BY Name;
            """;

        using var connection = _connectionFactory.CreateConnection();

        return await connection.QueryAsync<Equipment>(
            sql,
            new
            {
                SearchTerm = string.IsNullOrWhiteSpace(searchTerm)
                    ? null
                    : searchTerm.Trim()
            });
    }

    public async Task<Equipment?> GetByIdAsync(int id)
    {
        const string sql = """
            SELECT
                Id,
                Name,
                Category,
                SerialNumber,
                Brand,
                Model,
                Quantity,
                Notes,
                IsActive
            FROM Equipment
            WHERE Id = @Id;
            """;

        using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<Equipment>(
            sql,
            new { Id = id });
    }

    public async Task<int> CreateAsync(Equipment equipment)
    {
        const string sql = """
            INSERT INTO Equipment
            (
                Name,
                Category,
                SerialNumber,
                Brand,
                Model,
                Quantity,
                Notes,
                IsActive
            )
            VALUES
            (
                @Name,
                @Category,
                @SerialNumber,
                @Brand,
                @Model,
                @Quantity,
                @Notes,
                @IsActive
            );

            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;

        using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleAsync<int>(
            sql,
            equipment);
    }

    public async Task UpdateAsync(Equipment equipment)
    {
        const string sql = """
            UPDATE Equipment
            SET
                Name = @Name,
                Category = @Category,
                SerialNumber = @SerialNumber,
                Brand = @Brand,
                Model = @Model,
                Quantity = @Quantity,
                Notes = @Notes,
                IsActive = @IsActive
            WHERE Id = @Id;
            """;

        using var connection = _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(
            sql,
            equipment);
    }

    public async Task ToggleActiveStatusAsync(int id)
    {
        const string sql = """
            UPDATE Equipment
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