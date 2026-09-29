using Dapper;
using DiveCenterManager.Data;
using DiveCenterManager.ViewModels;
using DiveCenterManager.Models;

namespace DiveCenterManager.Services;

public class StaffService
{
    private readonly SqlConnectionFactory _connectionFactory;

    public StaffService(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<StaffListItemViewModel>> GetAllAsync(
    string? searchTerm = null)
{
    const string sql = """
        SELECT
            S.Id,
            S.FirstName,
            S.LastName,
            S.Phone,
            S.Email,
            S.CertificationAgency,
            S.InstructorNumber,
            S.IsActive,

            ISNULL(
                STRING_AGG(R.Name, ', '),
                ''
            ) AS Roles

        FROM Staff S

        LEFT JOIN StaffRoles SR
            ON S.Id = SR.StaffId

        LEFT JOIN Roles R
            ON SR.RoleId = R.Id

        WHERE
            @SearchTerm IS NULL
            OR S.FirstName LIKE '%' + @SearchTerm + '%'
            OR S.LastName LIKE '%' + @SearchTerm + '%'
            OR S.Phone LIKE '%' + @SearchTerm + '%'
            OR S.Email LIKE '%' + @SearchTerm + '%'
            OR S.CertificationAgency LIKE '%' + @SearchTerm + '%'
            OR S.InstructorNumber LIKE '%' + @SearchTerm + '%'

        GROUP BY
            S.Id,
            S.FirstName,
            S.LastName,
            S.Phone,
            S.Email,
            S.CertificationAgency,
            S.InstructorNumber,
            S.IsActive

        ORDER BY
            S.FirstName,
            S.LastName;
        """;

    using var connection = _connectionFactory.CreateConnection();

    return await connection.QueryAsync<StaffListItemViewModel>(
        sql,
        new
        {
            SearchTerm = string.IsNullOrWhiteSpace(searchTerm)
                ? null
                : searchTerm.Trim()
        });
}
    public async Task<int> CreateAsync(StaffCreateViewModel model)
{
    using var connection = _connectionFactory.CreateConnection();

    await connection.OpenAsync();

    await using var transaction = await connection.BeginTransactionAsync();

    try
    {
        const string staffSql = """
            INSERT INTO Staff
            (
                FirstName,
                LastName,
                Phone,
                Email,
                CertificationAgency,
                InstructorNumber,
                Notes,
                IsActive
            )
            VALUES
            (
                @FirstName,
                @LastName,
                @Phone,
                @Email,
                @CertificationAgency,
                @InstructorNumber,
                @Notes,
                @IsActive
            );

            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;

        var staffId = await connection.QuerySingleAsync<int>(
            staffSql,
            new
            {
                model.FirstName,
                model.LastName,
                model.Phone,
                model.Email,
                model.CertificationAgency,
                model.InstructorNumber,
                model.Notes,
                model.IsActive
            },
            transaction);

        const string roleSql = """
            INSERT INTO StaffRoles
            (
                StaffId,
                RoleId
            )
            VALUES
            (
                @StaffId,
                @RoleId
            );
            """;

        foreach (var roleId in model.SelectedRoleIds.Distinct())
        {
            await connection.ExecuteAsync(
                roleSql,
                new
                {
                    StaffId = staffId,
                    RoleId = roleId
                },
                transaction);
        }

        await transaction.CommitAsync();

        return staffId;
    }
    catch
    {
        await transaction.RollbackAsync();
        throw;
    }
}
public async Task<StaffEditViewModel?> GetByIdAsync(int id)
{
    using var connection = _connectionFactory.CreateConnection();

    const string staffSql = """
        SELECT
            Id,
            FirstName,
            LastName,
            Phone,
            Email,
            CertificationAgency,
            InstructorNumber,
            Notes,
            IsActive
        FROM Staff
        WHERE Id = @Id;
        """;

    var model = await connection.QuerySingleOrDefaultAsync<StaffEditViewModel>(
        staffSql,
        new { Id = id });

    if (model is null)
    {
        return null;
    }

    const string rolesSql = """
        SELECT RoleId
        FROM StaffRoles
        WHERE StaffId = @StaffId;
        """;

    var roleIds = await connection.QueryAsync<int>(
        rolesSql,
        new { StaffId = id });

    model.SelectedRoleIds = roleIds.ToList();

    return model;
}
public async Task UpdateAsync(StaffEditViewModel model)
{
    using var connection = _connectionFactory.CreateConnection();

    await connection.OpenAsync();

    await using var transaction = await connection.BeginTransactionAsync();

    try
    {
        const string staffSql = """
            UPDATE Staff
            SET
                FirstName = @FirstName,
                LastName = @LastName,
                Phone = @Phone,
                Email = @Email,
                CertificationAgency = @CertificationAgency,
                InstructorNumber = @InstructorNumber,
                Notes = @Notes,
                IsActive = @IsActive
            WHERE Id = @Id;
            """;

        await connection.ExecuteAsync(
            staffSql,
            new
            {
                model.Id,
                model.FirstName,
                model.LastName,
                model.Phone,
                model.Email,
                model.CertificationAgency,
                model.InstructorNumber,
                model.Notes,
                model.IsActive
            },
            transaction);

        const string deleteRolesSql = """
            DELETE FROM StaffRoles
            WHERE StaffId = @StaffId;
            """;

        await connection.ExecuteAsync(
            deleteRolesSql,
            new { StaffId = model.Id },
            transaction);

        const string insertRoleSql = """
            INSERT INTO StaffRoles
            (
                StaffId,
                RoleId
            )
            VALUES
            (
                @StaffId,
                @RoleId
            );
            """;

        foreach (var roleId in model.SelectedRoleIds.Distinct())
        {
            await connection.ExecuteAsync(
                insertRoleSql,
                new
                {
                    StaffId = model.Id,
                    RoleId = roleId
                },
                transaction);
        }

        await transaction.CommitAsync();
    }
    catch
    {
        await transaction.RollbackAsync();
        throw;
    }
}
public async Task<IEnumerable<Staff>> GetActiveAsync()
{
    const string sql = """
        SELECT
            Id,
            FirstName,
            LastName,
            Phone,
            Email,
            CertificationAgency,
            InstructorNumber,
            Notes,
            IsActive
        FROM Staff
        WHERE IsActive = 1
        ORDER BY
            FirstName,
            LastName;
        """;

    using var connection = _connectionFactory.CreateConnection();

    return await connection.QueryAsync<Staff>(sql);
}

public async Task<IEnumerable<Staff>> GetAvailableAsync(
    DateTime reservationDate)
{
    const string sql = """
        SELECT
            S.Id,
            S.FirstName,
            S.LastName,
            S.Phone,
            S.Email,
            S.CertificationAgency,
            S.InstructorNumber,
            S.Notes,
            S.IsActive
        FROM Staff S
        WHERE
            S.IsActive = 1

            AND NOT EXISTS
            (
                SELECT 1
                FROM Reservations R
                WHERE
                    R.StaffId = S.Id
                    AND R.ReservationDate = @ReservationDate
                    AND R.Status NOT IN
                    (
                        N'Cancelled',
                        N'NoShow'
                    )
            )

        ORDER BY
            S.FirstName,
            S.LastName;
        """;

    using var connection = _connectionFactory.CreateConnection();

    return await connection.QueryAsync<Staff>(
        sql,
        new
        {
            ReservationDate = reservationDate.Date
        });
}
public async Task<IEnumerable<Staff>> GetAvailableForEditAsync(
    DateTime reservationDate,
    int reservationId)
{
    const string sql = """
        SELECT
            S.Id,
            S.FirstName,
            S.LastName,
            S.Phone,
            S.Email,
            S.CertificationAgency,
            S.InstructorNumber,
            S.Notes,
            S.IsActive
        FROM Staff S
        WHERE
            S.IsActive = 1

            AND NOT EXISTS
            (
                SELECT 1
                FROM Reservations R
                WHERE
                    R.StaffId = S.Id
                    AND R.ReservationDate = @ReservationDate
                    AND R.Id <> @ReservationId
                    AND R.Status NOT IN
                    (
                        N'Cancelled',
                        N'NoShow'
                    )
            )

        ORDER BY
            S.FirstName,
            S.LastName;
        """;

    using var connection = _connectionFactory.CreateConnection();

    return await connection.QueryAsync<Staff>(
        sql,
        new
        {
            ReservationDate = reservationDate.Date,
            ReservationId = reservationId
        });
}

}