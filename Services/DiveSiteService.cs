using Dapper;
using DiveCenterManager.Data;
using DiveCenterManager.Models;

namespace DiveCenterManager.Services;

public class DiveSiteService
{
    private readonly SqlConnectionFactory _connectionFactory;

    public DiveSiteService(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<DiveSite>> GetAllAsync(string? searchTerm = null)
{
    const string sql = """
        SELECT
            Id,
            Name,
            Location,
            Latitude,
            Longitude,
            MaximumDepthMeters,
            Description,
            IsActive
        FROM DiveSites
        WHERE
            @SearchTerm IS NULL
            OR Name LIKE '%' + @SearchTerm + '%'
            OR Location LIKE '%' + @SearchTerm + '%'
        ORDER BY Name;
        """;

    using var connection = _connectionFactory.CreateConnection();

    return await connection.QueryAsync<DiveSite>(
        sql,
        new
        {
            SearchTerm = string.IsNullOrWhiteSpace(searchTerm)
                ? null
                : searchTerm.Trim()
        });
}
    public async Task<int> CreateAsync(DiveSite diveSite)
{
    const string sql = """
        INSERT INTO DiveSites
        (
            Name,
            Location,
            Latitude,
            Longitude,
            MaximumDepthMeters,
            Description,
            IsActive
        )
        VALUES
        (
            @Name,
            @Location,
            @Latitude,
            @Longitude,
            @MaximumDepthMeters,
            @Description,
            @IsActive
        );

        SELECT CAST(SCOPE_IDENTITY() AS INT);
        """;

    using var connection = _connectionFactory.CreateConnection();

    return await connection.QuerySingleAsync<int>(sql, diveSite);
}

public async Task<DiveSite?> GetByIdAsync(int id)
{
    const string sql = """
        SELECT
            Id,
            Name,
            Location,
            Latitude,
            Longitude,
            MaximumDepthMeters,
            Description,
            IsActive
        FROM DiveSites
        WHERE Id = @Id;
        """;

    using var connection = _connectionFactory.CreateConnection();

    return await connection.QuerySingleOrDefaultAsync<DiveSite>(
        sql,
        new { Id = id });
}

public async Task<bool> UpdateAsync(DiveSite diveSite)
{
    const string sql = """
        UPDATE DiveSites
        SET
            Name = @Name,
            Location = @Location,
            Latitude = @Latitude,
            Longitude = @Longitude,
            MaximumDepthMeters = @MaximumDepthMeters,
            Description = @Description,
            IsActive = @IsActive
        WHERE Id = @Id;
        """;

    using var connection = _connectionFactory.CreateConnection();

    var affectedRows = await connection.ExecuteAsync(sql, diveSite);

    return affectedRows > 0;
}
public async Task<bool> SetActiveStatusAsync(int id, bool isActive)
{
    const string sql = """
        UPDATE DiveSites
        SET IsActive = @IsActive
        WHERE Id = @Id;
        """;

    using var connection = _connectionFactory.CreateConnection();

    var affectedRows = await connection.ExecuteAsync(
        sql,
        new
        {
            Id = id,
            IsActive = isActive
        });

    return affectedRows > 0;
}
public async Task<IEnumerable<DiveSite>> GetActiveAsync()
{
    const string sql = """
        SELECT
            Id,
            Name,
            Location,
            Latitude,
            Longitude,
            MaximumDepthMeters,
            Description,
            IsActive
        FROM DiveSites
        WHERE IsActive = 1
        ORDER BY Name;
        """;

    using var connection = _connectionFactory.CreateConnection();

    return await connection.QueryAsync<DiveSite>(sql);
}

}