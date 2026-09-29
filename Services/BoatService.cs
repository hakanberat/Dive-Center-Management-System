using Dapper;
using DiveCenterManager.Data;
using DiveCenterManager.Models;

namespace DiveCenterManager.Services;

public class BoatService
{
    private readonly SqlConnectionFactory _connectionFactory;

    public BoatService(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<Boat>> GetAllAsync(string? searchTerm = null)
{
    const string sql = """
        SELECT
            Id,
            Name,
            RegistrationNumber,
            Capacity,
            Notes,
            IsActive
        FROM Boats
        WHERE
            @SearchTerm IS NULL
            OR Name LIKE '%' + @SearchTerm + '%'
            OR RegistrationNumber LIKE '%' + @SearchTerm + '%'
        ORDER BY Name;
        """;

    using var connection = _connectionFactory.CreateConnection();

    return await connection.QueryAsync<Boat>(
        sql,
        new
        {
            SearchTerm = string.IsNullOrWhiteSpace(searchTerm)
                ? null
                : searchTerm.Trim()
        });
}
    public async Task<int> CreateAsync(Boat boat)
{
    const string sql = """
        INSERT INTO Boats
        (
            Name,
            RegistrationNumber,
            Capacity,
            Notes,
            IsActive
        )
        VALUES
        (
            @Name,
            @RegistrationNumber,
            @Capacity,
            @Notes,
            @IsActive
        );

        SELECT CAST(SCOPE_IDENTITY() AS INT);
        """;

    using var connection = _connectionFactory.CreateConnection();

    return await connection.QuerySingleAsync<int>(sql, boat);
}
public async Task<Boat?> GetByIdAsync(int id)
{
    const string sql = """
        SELECT
            Id,
            Name,
            RegistrationNumber,
            Capacity,
            Notes,
            IsActive
        FROM Boats
        WHERE Id = @Id;
        """;

    using var connection = _connectionFactory.CreateConnection();

    return await connection.QuerySingleOrDefaultAsync<Boat>(
        sql,
        new { Id = id });
}
public async Task<bool> UpdateAsync(Boat boat)
{
    const string sql = """
        UPDATE Boats
        SET
            Name = @Name,
            RegistrationNumber = @RegistrationNumber,
            Capacity = @Capacity,
            Notes = @Notes,
            IsActive = @IsActive
        WHERE Id = @Id;
        """;

    using var connection = _connectionFactory.CreateConnection();

    var affectedRows = await connection.ExecuteAsync(sql, boat);

    return affectedRows > 0;
}
public async Task<bool> SetActiveStatusAsync(int id, bool isActive)
{
    const string sql = """
        UPDATE Boats
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
public async Task<IEnumerable<Boat>> GetActiveAsync()
{
    const string sql = """
        SELECT
            Id,
            Name,
            RegistrationNumber,
            Capacity,
            Notes,
            IsActive
        FROM Boats
        WHERE IsActive = 1
        ORDER BY Name;
        """;

    using var connection = _connectionFactory.CreateConnection();

    return await connection.QueryAsync<Boat>(sql);
}
public async Task<IEnumerable<Boat>> GetAvailableAsync(
    DateTime reservationDate,
    int participantCount)
{
    const string sql = """
        SELECT
            B.Id,
            B.Name,
            B.RegistrationNumber,
            B.Capacity,
            B.Notes,
            B.IsActive
        FROM Boats B
        WHERE
            B.IsActive = 1
            AND B.Capacity >= @ParticipantCount

            AND NOT EXISTS
            (
                SELECT 1
                FROM Reservations R
                WHERE
                    R.BoatId = B.Id
                    AND R.ReservationDate = @ReservationDate
                    AND R.Status NOT IN
                    (
                        N'Cancelled',
                        N'NoShow'
                    )
            )

        ORDER BY B.Name;
        """;

    using var connection = _connectionFactory.CreateConnection();

    return await connection.QueryAsync<Boat>(
        sql,
        new
        {
            ReservationDate = reservationDate.Date,
            ParticipantCount = participantCount
        });
}
public async Task<IEnumerable<Boat>> GetAvailableForEditAsync(
    DateTime reservationDate,
    int participantCount,
    int reservationId)
{
    const string sql = """
        SELECT
            B.Id,
            B.Name,
            B.RegistrationNumber,
            B.Capacity,
            B.Notes,
            B.IsActive
        FROM Boats B
        WHERE
            B.IsActive = 1
            AND B.Capacity >= @ParticipantCount

            AND NOT EXISTS
            (
                SELECT 1
                FROM Reservations R
                WHERE
                    R.BoatId = B.Id
                    AND R.ReservationDate = @ReservationDate
                    AND R.Id <> @ReservationId
                    AND R.Status NOT IN
                    (
                        N'Cancelled',
                        N'NoShow'
                    )
            )

        ORDER BY B.Name;
        """;

    using var connection = _connectionFactory.CreateConnection();

    return await connection.QueryAsync<Boat>(
        sql,
        new
        {
            ReservationDate = reservationDate.Date,
            ParticipantCount = participantCount,
            ReservationId = reservationId
        });
}

}