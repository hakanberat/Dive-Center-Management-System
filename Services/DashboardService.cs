using Dapper;
using DiveCenterManager.Data;
using DiveCenterManager.ViewModels;

namespace DiveCenterManager.Services;

public class DashboardService
{
    private readonly SqlConnectionFactory _connectionFactory;

    public DashboardService(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<DashboardViewModel> GetDashboardAsync()
    {
        using var connection = _connectionFactory.CreateConnection();

        const string countsSql = """
            SELECT
                (
                    SELECT COUNT(*)
                    FROM Reservations
                    WHERE ReservationDate = CAST(GETDATE() AS DATE)
                      AND Status NOT IN (N'Cancelled', N'NoShow')
                ) AS TodayReservationCount,

                (
                    SELECT COUNT(*)
                    FROM Reservations
                    WHERE Status = N'Pending'
                ) AS PendingReservationCount,

                (
                    SELECT COUNT(*)
                    FROM Reservations
                    WHERE Status = N'Confirmed'
                ) AS ConfirmedReservationCount,

                (
                    SELECT COUNT(*)
                    FROM Staff
                    WHERE IsActive = 1
                ) AS ActiveStaffCount,

                (
                    SELECT COUNT(*)
                    FROM Boats
                    WHERE IsActive = 1
                ) AS ActiveBoatCount,

                (
                    SELECT COUNT(*)
                    FROM Equipment
                    WHERE IsActive = 1
                ) AS ActiveEquipmentCount;
            """;

        var model =
            await connection.QuerySingleAsync<DashboardViewModel>(
                countsSql);

        const string todayReservationsSql = """
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

            WHERE
                R.ReservationDate = CAST(GETDATE() AS DATE)
                AND R.Status NOT IN
                (
                    N'Cancelled',
                    N'NoShow'
                )

            ORDER BY
                R.StartTime;
            """;

        model.TodayReservations =
            await connection.QueryAsync<ReservationListItemViewModel>(
                todayReservationsSql);

        return model;
    }
}