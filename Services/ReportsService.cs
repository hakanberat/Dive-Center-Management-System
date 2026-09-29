using Dapper;
using DiveCenterManager.Data;
using DiveCenterManager.ViewModels;

namespace DiveCenterManager.Services;

public class ReportsService
{
    private readonly SqlConnectionFactory _connectionFactory;

    public ReportsService(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<ReportsViewModel> GetReportAsync(
        DateTime startDate,
        DateTime endDate)
    {
        using var connection = _connectionFactory.CreateConnection();

        var model = new ReportsViewModel
        {
            StartDate = startDate.Date,
            EndDate = endDate.Date
        };

        const string reservationSummarySql = """
            SELECT
                COUNT(*) AS ReservationCount,
                ISNULL(SUM(ParticipantCount), 0) AS ParticipantCount
            FROM Reservations
            WHERE
                ReservationDate >= @StartDate
                AND ReservationDate < @EndDateExclusive;
            """;

        var summary =
            await connection.QuerySingleAsync<ReportsViewModel>(
                reservationSummarySql,
                new
                {
                    StartDate = startDate.Date,
                    EndDateExclusive = endDate.Date.AddDays(1)
                });

        model.ReservationCount =
            summary.ReservationCount;

        model.ParticipantCount =
            summary.ParticipantCount;

        const string incomeSql = """
            SELECT
                C.Code AS CurrencyCode,
                C.Symbol AS CurrencySymbol,
                SUM(T.Amount) AS TotalAmount
            FROM Transactions T

            INNER JOIN TransactionCategories TC
                ON T.CategoryId = TC.Id

            INNER JOIN Currencies C
                ON T.CurrencyId = C.Id

            WHERE
                TC.Type = N'Income'
                AND T.TransactionDate >= @StartDate
                AND T.TransactionDate < @EndDateExclusive

            GROUP BY
                C.Code,
                C.Symbol

            ORDER BY C.Code;
            """;

        model.IncomeTotals =
            await connection.QueryAsync<ReportCurrencyTotalViewModel>(
                incomeSql,
                new
                {
                    StartDate = startDate.Date,
                    EndDateExclusive = endDate.Date.AddDays(1)
                });

        const string expenseSql = """
            SELECT
                C.Code AS CurrencyCode,
                C.Symbol AS CurrencySymbol,
                SUM(T.Amount) AS TotalAmount
            FROM Transactions T

            INNER JOIN TransactionCategories TC
                ON T.CategoryId = TC.Id

            INNER JOIN Currencies C
                ON T.CurrencyId = C.Id

            WHERE
                TC.Type = N'Expense'
                AND T.TransactionDate >= @StartDate
                AND T.TransactionDate < @EndDateExclusive

            GROUP BY
                C.Code,
                C.Symbol

            ORDER BY C.Code;
            """;

        model.ExpenseTotals =
            await connection.QueryAsync<ReportCurrencyTotalViewModel>(
                expenseSql,
                new
                {
                    StartDate = startDate.Date,
                    EndDateExclusive = endDate.Date.AddDays(1)
                });

        const string statusSql = """
            SELECT
                Status,
                COUNT(*) AS Count
            FROM Reservations
            WHERE
                ReservationDate >= @StartDate
                AND ReservationDate < @EndDateExclusive
            GROUP BY Status
            ORDER BY Status;
            """;

        model.ReservationStatuses =
            await connection.QueryAsync<ReportStatusViewModel>(
                statusSql,
                new
                {
                    StartDate = startDate.Date,
                    EndDateExclusive = endDate.Date.AddDays(1)
                });

        const string activitySql = """
            SELECT
                A.Name AS ActivityName,
                COUNT(*) AS ReservationCount,
                SUM(R.ParticipantCount) AS ParticipantCount
            FROM Reservations R

            INNER JOIN Activities A
                ON R.ActivityId = A.Id

            WHERE
                R.ReservationDate >= @StartDate
                AND R.ReservationDate < @EndDateExclusive

            GROUP BY A.Name

            ORDER BY
                ReservationCount DESC,
                A.Name;
            """;

        model.Activities =
            await connection.QueryAsync<ReportActivityViewModel>(
                activitySql,
                new
                {
                    StartDate = startDate.Date,
                    EndDateExclusive = endDate.Date.AddDays(1)
                });

        return model;
    }
}