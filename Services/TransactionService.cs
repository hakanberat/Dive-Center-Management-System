using Dapper;
using DiveCenterManager.Data;
using DiveCenterManager.ViewModels;

namespace DiveCenterManager.Services;

public class TransactionService
{
    private readonly SqlConnectionFactory _connectionFactory;

    public TransactionService(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<TransactionListItemViewModel>> GetAllAsync()
    {
        const string sql = """
            SELECT
                T.Id,
                T.TransactionDate,
                TC.Name AS CategoryName,
                TC.Type,
                T.Amount,
                C.Code AS CurrencyCode,
                C.Symbol AS CurrencySymbol,
                T.Description
            FROM Transactions T

            INNER JOIN TransactionCategories TC
                ON T.CategoryId = TC.Id

            INNER JOIN Currencies C
                ON T.CurrencyId = C.Id

            ORDER BY
                T.TransactionDate DESC,
                T.Id DESC;
            """;

        using var connection = _connectionFactory.CreateConnection();

        return await connection.QueryAsync<TransactionListItemViewModel>(sql);
    }
    public async Task<int> CreateAsync(TransactionCreateViewModel model)
{
    const string sql = """
        INSERT INTO Transactions
        (
            TransactionDate,
            CategoryId,
            Amount,
            CurrencyId,
            Description
        )
        VALUES
        (
            @TransactionDate,
            @CategoryId,
            @Amount,
            @CurrencyId,
            @Description
        );

        SELECT CAST(SCOPE_IDENTITY() AS INT);
        """;

    using var connection = _connectionFactory.CreateConnection();

    return await connection.QuerySingleAsync<int>(
        sql,
        new
        {
            model.TransactionDate,
            model.CategoryId,
            model.Amount,
            model.CurrencyId,
            model.Description
        });
}
public async Task<TransactionCreateViewModel?> GetByIdAsync(int id)
{
    const string sql = """
        SELECT
            TransactionDate,
            CategoryId,
            Amount,
            CurrencyId,
            Description
        FROM Transactions
        WHERE Id = @Id;
        """;

    using var connection = _connectionFactory.CreateConnection();

    return await connection.QuerySingleOrDefaultAsync<TransactionCreateViewModel>(
        sql,
        new { Id = id });
}
public async Task UpdateAsync(
    int id,
    TransactionCreateViewModel model)
{
    const string sql = """
        UPDATE Transactions
        SET
            TransactionDate = @TransactionDate,
            CategoryId = @CategoryId,
            Amount = @Amount,
            CurrencyId = @CurrencyId,
            Description = @Description
        WHERE Id = @Id;
        """;

    using var connection = _connectionFactory.CreateConnection();

    await connection.ExecuteAsync(
        sql,
        new
        {
            Id = id,
            model.TransactionDate,
            model.CategoryId,
            model.Amount,
            model.CurrencyId,
            model.Description
        });
}
public async Task DeleteAsync(int id)
{
    const string sql = """
        DELETE FROM Transactions
        WHERE Id = @Id;
        """;

    using var connection = _connectionFactory.CreateConnection();

    await connection.ExecuteAsync(
        sql,
        new { Id = id });
}
}