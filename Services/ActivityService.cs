using Dapper;
using DiveCenterManager.Data;
using DiveCenterManager.Models;

namespace DiveCenterManager.Services;

public class ActivityService
{
    private readonly SqlConnectionFactory _connectionFactory;

    public ActivityService(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<DiveActivity>> GetAllAsync(string? searchTerm = null)
{
    const string sql = """
        SELECT
            A.Id,
            A.Name,
            A.DefaultPrice,
            A.CurrencyId,
            A.Description,
            A.IsActive,
            C.Code AS CurrencyCode,
            C.Symbol AS CurrencySymbol
        FROM Activities A
        INNER JOIN Currencies C
            ON A.CurrencyId = C.Id
        WHERE
            @SearchTerm IS NULL
            OR A.Name LIKE '%' + @SearchTerm + '%'
            OR A.Description LIKE '%' + @SearchTerm + '%'
        ORDER BY A.Name;
        """;

    using var connection = _connectionFactory.CreateConnection();

    return await connection.QueryAsync<DiveActivity>(
        sql,
        new
        {
            SearchTerm = string.IsNullOrWhiteSpace(searchTerm)
                ? null
                : searchTerm.Trim()
        });
}
    public async Task<int> CreateAsync(DiveActivity activity)
{
    const string sql = """
        INSERT INTO Activities
        (
            Name,
            DefaultPrice,
            CurrencyId,
            Description,
            IsActive
        )
        VALUES
        (
            @Name,
            @DefaultPrice,
            @CurrencyId,
            @Description,
            @IsActive
        );

        SELECT CAST(SCOPE_IDENTITY() AS INT);
        """;

    using var connection = _connectionFactory.CreateConnection();

    return await connection.QuerySingleAsync<int>(sql, activity);
}
public async Task<DiveActivity?> GetByIdAsync(int id)
{
    const string sql = """
        SELECT
            A.Id,
            A.Name,
            A.DefaultPrice,
            A.CurrencyId,
            A.Description,
            A.IsActive,
            C.Code AS CurrencyCode,
            C.Symbol AS CurrencySymbol
        FROM Activities A
        INNER JOIN Currencies C
            ON A.CurrencyId = C.Id
        WHERE A.Id = @Id;
        """;

    using var connection = _connectionFactory.CreateConnection();

    return await connection.QuerySingleOrDefaultAsync<DiveActivity>(
        sql,
        new { Id = id });
}
public async Task<bool> UpdateAsync(DiveActivity activity)
{
    const string sql = """
        UPDATE Activities
        SET
            Name = @Name,
            DefaultPrice = @DefaultPrice,
            CurrencyId = @CurrencyId,
            Description = @Description,
            IsActive = @IsActive
        WHERE Id = @Id;
        """;

    using var connection = _connectionFactory.CreateConnection();

    var affectedRows = await connection.ExecuteAsync(sql, activity);

    return affectedRows > 0;
}
public async Task<bool> SetActiveStatusAsync(int id, bool isActive)
{
    const string sql = """
        UPDATE Activities
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
public async Task<IEnumerable<DiveActivity>> GetActiveAsync()
{
    const string sql = """
        SELECT
            A.Id,
            A.Name,
            A.DefaultPrice,
            A.CurrencyId,
            A.Description,
            A.IsActive,
            C.Code AS CurrencyCode,
            C.Symbol AS CurrencySymbol
        FROM Activities A

        INNER JOIN Currencies C
            ON A.CurrencyId = C.Id

        WHERE
            A.IsActive = 1
            AND C.IsActive = 1

        ORDER BY A.Name;
        """;

    using var connection = _connectionFactory.CreateConnection();

    return await connection.QueryAsync<DiveActivity>(sql);
}

}