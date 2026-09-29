using Dapper;
using DiveCenterManager.Data;
using DiveCenterManager.ViewModels;

namespace DiveCenterManager.Services;

public class ReservationService
{
    private readonly SqlConnectionFactory _connectionFactory;

    public ReservationService(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<ReservationListItemViewModel>> GetAllAsync()
    {
        const string sql = """
            SELECT
                R.Id,
                R.ReservationDate,
                R.StartTime,

                A.Name AS ActivityName,

                DS.Name AS DiveSiteName,

                B.Name AS BoatName,

                CASE
                    WHEN S.Id IS NULL THEN NULL
                    ELSE S.FirstName + ' ' + S.LastName
                END AS StaffName,

                R.ParticipantCount,
                R.Status,
                R.UnitPrice,
                R.TotalAmount,

                C.Code AS CurrencyCode,
                C.Symbol AS CurrencySymbol,

                R.Notes

            FROM Reservations R

            INNER JOIN Activities A
                ON R.ActivityId = A.Id

            INNER JOIN Currencies C
                ON R.CurrencyId = C.Id

            LEFT JOIN DiveSites DS
                ON R.DiveSiteId = DS.Id

            LEFT JOIN Boats B
                ON R.BoatId = B.Id

            LEFT JOIN Staff S
                ON R.StaffId = S.Id

            ORDER BY
                R.ReservationDate DESC,
                R.StartTime DESC,
                R.Id DESC;
            """;

        using var connection = _connectionFactory.CreateConnection();

        return await connection.QueryAsync<ReservationListItemViewModel>(sql);
    }

    public async Task<int> CreateAsync(
        ReservationCreateViewModel model)
    {
        using var connection = _connectionFactory.CreateConnection();

        await connection.OpenAsync();

        // Activity kontrolü ve güncel fiyat bilgisi
        const string activitySql = """
            SELECT
                DefaultPrice,
                CurrencyId
            FROM Activities
            WHERE
                Id = @ActivityId
                AND IsActive = 1;
            """;

        var activity = await connection.QuerySingleOrDefaultAsync<(
            decimal DefaultPrice,
            int CurrencyId)>(
            activitySql,
            new
            {
                model.ActivityId
            });

        if (activity == default)
        {
            throw new InvalidOperationException(
                "The selected activity does not exist or is inactive.");
        }

        // Boat seçilmişse kontrolleri yap
        if (model.BoatId.HasValue)
        {
            // Tekne aktif mi ve kapasitesi nedir?
            const string boatSql = """
                SELECT Capacity
                FROM Boats
                WHERE
                    Id = @BoatId
                    AND IsActive = 1;
                """;

            var boatCapacity =
                await connection.QuerySingleOrDefaultAsync<int?>(
                    boatSql,
                    new
                    {
                        BoatId = model.BoatId.Value
                    });

            if (!boatCapacity.HasValue)
            {
                throw new InvalidOperationException(
                    "The selected boat does not exist or is inactive.");
            }

            // Tekne kapasitesi yeterli mi?
            if (boatCapacity.Value < model.ParticipantCount)
            {
                throw new InvalidOperationException(
                    "The selected boat does not have enough capacity.");
            }

            // Tekne aynı tarihte başka aktif rezervasyonda mı?
            const string boatReservationSql = """
                SELECT COUNT(1)
                FROM Reservations
                WHERE
                    BoatId = @BoatId
                    AND ReservationDate = @ReservationDate
                    AND Status NOT IN
                    (
                        N'Cancelled',
                        N'NoShow'
                    );
                """;

            var boatReservationCount =
                await connection.QuerySingleAsync<int>(
                    boatReservationSql,
                    new
                    {
                        BoatId = model.BoatId.Value,
                        model.ReservationDate
                    });

            if (boatReservationCount > 0)
            {
                throw new InvalidOperationException(
                    "The selected boat is already assigned to another reservation on this date.");
            }
        }
        // Staff seçilmişse kontrolleri yap
        if (model.StaffId.HasValue)
        {
         // Personel aktif mi?
         const string staffSql = """
        SELECT COUNT(1)
        FROM Staff
        WHERE
            Id = @StaffId
            AND IsActive = 1;
        """;

    var staffExists =
        await connection.QuerySingleAsync<int>(
            staffSql,
            new
            {
                StaffId = model.StaffId.Value
            });

    if (staffExists == 0)
    {
        throw new InvalidOperationException(
            "The selected staff member does not exist or is inactive.");
    }

    // Personel aynı tarihte başka aktif rezervasyonda mı?
    const string staffReservationSql = """
        SELECT COUNT(1)
        FROM Reservations
        WHERE
            StaffId = @StaffId
            AND ReservationDate = @ReservationDate
            AND Status NOT IN
            (
                N'Cancelled',
                N'NoShow'
            );
        """;

    var staffReservationCount =
        await connection.QuerySingleAsync<int>(
            staffReservationSql,
            new
            {
                StaffId = model.StaffId.Value,
                model.ReservationDate
            });

    if (staffReservationCount > 0)
    {
        throw new InvalidOperationException(
            "The selected staff member is already assigned to another reservation on this date.");
    }
}
        // Activity fiyatını reservation'a snapshot olarak kopyala
        var unitPrice = activity.DefaultPrice;

        var totalAmount =
            unitPrice * model.ParticipantCount;

        // Reservation kaydı
        const string reservationSql = """
            INSERT INTO Reservations
            (
                ReservationDate,
                StartTime,
                ActivityId,
                DiveSiteId,
                BoatId,
                StaffId,
                ParticipantCount,
                Status,
                UnitPrice,
                CurrencyId,
                TotalAmount,
                Notes
            )
            VALUES
            (
                @ReservationDate,
                @StartTime,
                @ActivityId,
                @DiveSiteId,
                @BoatId,
                @StaffId,
                @ParticipantCount,
                N'Pending',
                @UnitPrice,
                @CurrencyId,
                @TotalAmount,
                @Notes
            );

            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;

        return await connection.QuerySingleAsync<int>(
            reservationSql,
            new
            {
                model.ReservationDate,
                model.StartTime,
                model.ActivityId,
                model.DiveSiteId,
                model.BoatId,
                model.StaffId,
                model.ParticipantCount,

                UnitPrice = unitPrice,
                CurrencyId = activity.CurrencyId,
                TotalAmount = totalAmount,

                model.Notes
            });
    }
public async Task SetStatusAsync(
    int id,
    string status)
{
    string[] allowedStatuses =
    [
        "Pending",
        "Confirmed",
        "Completed",
        "Cancelled",
        "NoShow"
    ];

    if (!allowedStatuses.Contains(status))
    {
        throw new ArgumentException(
            "Invalid reservation status.",
            nameof(status));
    }

    const string sql = """
        UPDATE Reservations
        SET Status = @Status
        WHERE Id = @Id;
        """;

    using var connection = _connectionFactory.CreateConnection();

    var affectedRows = await connection.ExecuteAsync(
        sql,
        new
        {
            Id = id,
            Status = status
        });

    if (affectedRows == 0)
    {
        throw new InvalidOperationException(
            "Reservation not found.");
    }
}
public async Task<ReservationEditViewModel?> GetByIdAsync(int id)
{
    const string sql = """
        SELECT
            Id,
            ReservationDate,
            StartTime,
            ActivityId,
            DiveSiteId,
            BoatId,
            StaffId,
            ParticipantCount,
            Notes
        FROM Reservations
        WHERE Id = @Id;
        """;

    using var connection = _connectionFactory.CreateConnection();

    return await connection.QuerySingleOrDefaultAsync<ReservationEditViewModel>(
        sql,
        new { Id = id });
}
public async Task UpdateAsync(
    ReservationEditViewModel model)
{
    using var connection = _connectionFactory.CreateConnection();

    await connection.OpenAsync();

    // Mevcut reservation bilgilerini al
    const string existingReservationSql = """
        SELECT
            ActivityId,
            UnitPrice,
            CurrencyId
        FROM Reservations
        WHERE Id = @Id;
        """;

    var existingReservation =
        await connection.QuerySingleOrDefaultAsync<(
            int ActivityId,
            decimal UnitPrice,
            int CurrencyId)>(
            existingReservationSql,
            new { model.Id });

    if (existingReservation == default)
    {
        throw new InvalidOperationException(
            "Reservation not found.");
    }

    decimal unitPrice;
    int currencyId;

    // Activity değiştiyse yeni Activity'nin güncel fiyatını al
    if (existingReservation.ActivityId != model.ActivityId)
    {
        const string activitySql = """
            SELECT
                DefaultPrice,
                CurrencyId
            FROM Activities
            WHERE
                Id = @ActivityId
                AND IsActive = 1;
            """;

        var activity =
            await connection.QuerySingleOrDefaultAsync<(
                decimal DefaultPrice,
                int CurrencyId)>(
                activitySql,
                new { model.ActivityId });

        if (activity == default)
        {
            throw new InvalidOperationException(
                "The selected activity does not exist or is inactive.");
        }

        unitPrice = activity.DefaultPrice;
        currencyId = activity.CurrencyId;
    }
    else
    {
        // Activity değişmediyse tarihi fiyat snapshot'ını koru
        unitPrice = existingReservation.UnitPrice;
        currencyId = existingReservation.CurrencyId;
    }

    // Boat seçilmişse kontrol et
    if (model.BoatId.HasValue)
    {
        const string boatSql = """
            SELECT Capacity
            FROM Boats
            WHERE
                Id = @BoatId
                AND IsActive = 1;
            """;

        var boatCapacity =
            await connection.QuerySingleOrDefaultAsync<int?>(
                boatSql,
                new
                {
                    BoatId = model.BoatId.Value
                });

        if (!boatCapacity.HasValue)
        {
            throw new InvalidOperationException(
                "The selected boat does not exist or is inactive.");
        }

        if (boatCapacity.Value < model.ParticipantCount)
        {
            throw new InvalidOperationException(
                "The selected boat does not have enough capacity.");
        }

        const string boatReservationSql = """
            SELECT COUNT(1)
            FROM Reservations
            WHERE
                BoatId = @BoatId
                AND ReservationDate = @ReservationDate
                AND Id <> @ReservationId
                AND Status NOT IN
                (
                    N'Cancelled',
                    N'NoShow'
                );
            """;

        var boatReservationCount =
            await connection.QuerySingleAsync<int>(
                boatReservationSql,
                new
                {
                    BoatId = model.BoatId.Value,
                    model.ReservationDate,
                    ReservationId = model.Id
                });

        if (boatReservationCount > 0)
        {
            throw new InvalidOperationException(
                "The selected boat is already assigned to another reservation on this date.");
        }
    }

    // Staff seçilmişse kontrol et
    if (model.StaffId.HasValue)
    {
        const string staffSql = """
            SELECT COUNT(1)
            FROM Staff
            WHERE
                Id = @StaffId
                AND IsActive = 1;
            """;

        var staffExists =
            await connection.QuerySingleAsync<int>(
                staffSql,
                new
                {
                    StaffId = model.StaffId.Value
                });

        if (staffExists == 0)
        {
            throw new InvalidOperationException(
                "The selected staff member does not exist or is inactive.");
        }

        const string staffReservationSql = """
            SELECT COUNT(1)
            FROM Reservations
            WHERE
                StaffId = @StaffId
                AND ReservationDate = @ReservationDate
                AND Id <> @ReservationId
                AND Status NOT IN
                (
                    N'Cancelled',
                    N'NoShow'
                );
            """;

        var staffReservationCount =
            await connection.QuerySingleAsync<int>(
                staffReservationSql,
                new
                {
                    StaffId = model.StaffId.Value,
                    model.ReservationDate,
                    ReservationId = model.Id
                });

        if (staffReservationCount > 0)
        {
            throw new InvalidOperationException(
                "The selected staff member is already assigned to another reservation on this date.");
        }
    }

    var totalAmount =
        unitPrice * model.ParticipantCount;

    const string updateSql = """
        UPDATE Reservations
        SET
            ReservationDate = @ReservationDate,
            StartTime = @StartTime,
            ActivityId = @ActivityId,
            DiveSiteId = @DiveSiteId,
            BoatId = @BoatId,
            StaffId = @StaffId,
            ParticipantCount = @ParticipantCount,
            UnitPrice = @UnitPrice,
            CurrencyId = @CurrencyId,
            TotalAmount = @TotalAmount,
            Notes = @Notes
        WHERE Id = @Id;
        """;

    await connection.ExecuteAsync(
        updateSql,
        new
        {
            model.Id,
            model.ReservationDate,
            model.StartTime,
            model.ActivityId,
            model.DiveSiteId,
            model.BoatId,
            model.StaffId,
            model.ParticipantCount,

            UnitPrice = unitPrice,
            CurrencyId = currencyId,
            TotalAmount = totalAmount,

            model.Notes
        });
}
public async Task<IEnumerable<CalendarReservationViewModel>> GetCalendarAsync(
    int year,
    int month)
{
    var startDate = new DateTime(year, month, 1);
    var endDate = startDate.AddMonths(1);

    const string sql = """
        SELECT
            R.Id,
            R.ReservationDate,
            R.StartTime,

            A.Name AS ActivityName,

            DS.Name AS DiveSiteName,

            B.Name AS BoatName,

            CASE
                WHEN S.Id IS NULL THEN NULL
                ELSE S.FirstName + ' ' + S.LastName
            END AS StaffName,

            R.ParticipantCount,
            R.Status

        FROM Reservations R

        INNER JOIN Activities A
            ON R.ActivityId = A.Id

        LEFT JOIN DiveSites DS
            ON R.DiveSiteId = DS.Id

        LEFT JOIN Boats B
            ON R.BoatId = B.Id

        LEFT JOIN Staff S
            ON R.StaffId = S.Id

        WHERE
            R.ReservationDate >= @StartDate
            AND R.ReservationDate < @EndDate

        ORDER BY
            R.ReservationDate,
            R.StartTime;
        """;

    using var connection = _connectionFactory.CreateConnection();

    return await connection.QueryAsync<CalendarReservationViewModel>(
        sql,
        new
        {
            StartDate = startDate,
            EndDate = endDate
        });
}

}