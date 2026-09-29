using Dapper;
using DiveCenterManager.Data;
using DiveCenterManager.Models;
using DiveCenterManager.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace DiveCenterManager.Services;

public class AppUserService
{
    private readonly SqlConnectionFactory _connectionFactory;
    private readonly IPasswordHasher<AppUser> _passwordHasher;

    public AppUserService(
        SqlConnectionFactory connectionFactory,
        IPasswordHasher<AppUser> passwordHasher)
    {
        _connectionFactory = connectionFactory;
        _passwordHasher = passwordHasher;
    }

    public async Task<IEnumerable<AppUserListItemViewModel>> GetAllAsync()
    {
        const string sql = """
            SELECT
                Id,
                Username,
                DisplayName,
                SystemRole,
                IsActive,
                CreatedAt
            FROM AppUsers
            ORDER BY Username;
            """;

        using var connection =
            _connectionFactory.CreateConnection();

        return await connection.QueryAsync<AppUserListItemViewModel>(
            sql);
    }

    public async Task<AppUserEditViewModel?> GetByIdAsync(int id)
    {
        const string sql = """
            SELECT
                Id,
                Username,
                DisplayName,
                SystemRole
            FROM AppUsers
            WHERE Id = @Id;
            """;

        using var connection =
            _connectionFactory.CreateConnection();

        return await connection
            .QuerySingleOrDefaultAsync<AppUserEditViewModel>(
                sql,
                new
                {
                    Id = id
                });
    }

    public async Task UpdateAsync(
    AppUserEditViewModel model)
{
    if (model.SystemRole != "Admin" &&
        model.SystemRole != "User")
    {
        throw new ArgumentException(
            "System role must be Admin or User.",
            nameof(model.SystemRole));
    }

    const string sql = """
        UPDATE AppUsers
        SET
            Username = @Username,
            DisplayName = @DisplayName,
            SystemRole = @SystemRole,
            SecurityVersion = SecurityVersion + 1
        WHERE Id = @Id;
        """;

    using var connection =
        _connectionFactory.CreateConnection();

    await connection.ExecuteAsync(
        sql,
        new
        {
            model.Id,
            Username = model.Username.Trim(),
            DisplayName = model.DisplayName.Trim(),
            model.SystemRole
        });
}

    public async Task<AppUserResetPasswordViewModel?>
        GetResetPasswordAsync(int id)
    {
        const string sql = """
            SELECT
                Id,
                Username
            FROM AppUsers
            WHERE Id = @Id;
            """;

        using var connection =
            _connectionFactory.CreateConnection();

        return await connection
            .QuerySingleOrDefaultAsync<AppUserResetPasswordViewModel>(
                sql,
                new
                {
                    Id = id
                });
    }

    public async Task<bool> ResetPasswordAsync(
        int id,
        string newPassword)
    {
        const string selectSql = """
            SELECT
                Id,
                Username,
                PasswordHash,
                DisplayName,
                SystemRole,
                IsActive,
                SecurityVersion,
                CreatedAt
            FROM AppUsers
            WHERE Id = @Id;
            """;

        using var connection =
            _connectionFactory.CreateConnection();

        var user =
            await connection.QuerySingleOrDefaultAsync<AppUser>(
                selectSql,
                new
                {
                    Id = id
                });

        if (user is null)
        {
            return false;
        }

        var passwordHash =
            _passwordHasher.HashPassword(
                user,
                newPassword);

        const string updateSql = """
            UPDATE AppUsers
            SET
                PasswordHash = @PasswordHash,
                SecurityVersion = SecurityVersion + 1
            WHERE Id = @Id;
            """;

        await connection.ExecuteAsync(
            updateSql,
            new
            {
                Id = id,
                PasswordHash = passwordHash
            });

        return true;
    }

    public async Task<AppUser?> AuthenticateAsync(
        string username,
        string password)
    {
        const string sql = """
            SELECT
                Id,
                Username,
                PasswordHash,
                DisplayName,
                SystemRole,
                IsActive,
                SecurityVersion,
                CreatedAt
            FROM AppUsers
            WHERE
                Username = @Username
                AND IsActive = 1;
            """;

        using var connection =
            _connectionFactory.CreateConnection();

        var user =
            await connection.QuerySingleOrDefaultAsync<AppUser>(
                sql,
                new
                {
                    Username = username.Trim()
                });

        if (user is null)
        {
            return null;
        }

        var result =
            _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                password);

        if (result == PasswordVerificationResult.Failed)
        {
            return null;
        }

        return user;
    }

    public async Task<bool> AnyUsersAsync()
    {
        const string sql = """
            SELECT
                CASE
                    WHEN EXISTS
                    (
                        SELECT 1
                        FROM AppUsers
                    )
                    THEN CAST(1 AS BIT)
                    ELSE CAST(0 AS BIT)
                END;
            """;

        using var connection =
            _connectionFactory.CreateConnection();

        return await connection.QuerySingleAsync<bool>(sql);
    }

    public async Task<int> CreateAsync(
        string username,
        string password,
        string displayName,
        string systemRole)
    {
        if (systemRole != "Admin" &&
            systemRole != "User")
        {
            throw new ArgumentException(
                "System role must be Admin or User.",
                nameof(systemRole));
        }

        var user = new AppUser
        {
            Username = username.Trim(),
            DisplayName = displayName.Trim(),
            SystemRole = systemRole,
            IsActive = true
        };

        user.PasswordHash =
            _passwordHasher.HashPassword(
                user,
                password);

        const string sql = """
            INSERT INTO AppUsers
            (
                Username,
                PasswordHash,
                DisplayName,
                SystemRole,
                IsActive
            )
            VALUES
            (
                @Username,
                @PasswordHash,
                @DisplayName,
                @SystemRole,
                @IsActive
            );

            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;

        using var connection =
            _connectionFactory.CreateConnection();

        return await connection.QuerySingleAsync<int>(
            sql,
            user);
    }

    public async Task ToggleActiveStatusAsync(int id)
    {
        const string sql = """
            UPDATE AppUsers
            SET IsActive =
                CASE
                    WHEN IsActive = 1 THEN 0
                    ELSE 1
                END
            WHERE Id = @Id;
            """;

        using var connection =
            _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(
            sql,
            new
            {
                Id = id
            });
    }

    public async Task<AppUser?> GetAuthenticationUserByIdAsync(int id)
    {
        const string sql = """
            SELECT
                Id,
                Username,
                PasswordHash,
                DisplayName,
                SystemRole,
                IsActive,
                SecurityVersion,
                CreatedAt
            FROM AppUsers
            WHERE Id = @Id;
            """;

        using var connection =
            _connectionFactory.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<AppUser>(
            sql,
            new
            {
                Id = id
            });
    }

    public async Task<int> GetActiveAdminCountAsync()
    {
        const string sql = """
            SELECT COUNT(*)
            FROM AppUsers
            WHERE
                SystemRole = N'Admin'
                AND IsActive = 1;
            """;

        using var connection =
            _connectionFactory.CreateConnection();

        return await connection.QuerySingleAsync<int>(sql);
    }

    public async Task<bool> UsernameExistsAsync(
        string username,
        int? excludeUserId = null)
    {
        const string sql = """
            SELECT
                CASE
                    WHEN EXISTS
                    (
                        SELECT 1
                        FROM AppUsers
                        WHERE
                            Username = @Username
                            AND
                            (
                                @ExcludeUserId IS NULL
                                OR Id <> @ExcludeUserId
                            )
                    )
                    THEN CAST(1 AS BIT)
                    ELSE CAST(0 AS BIT)
                END;
            """;

        using var connection =
            _connectionFactory.CreateConnection();

        return await connection.QuerySingleAsync<bool>(
            sql,
            new
            {
                Username = username.Trim(),
                ExcludeUserId = excludeUserId
            });
    }
}