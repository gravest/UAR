using Microsoft.Data.SqlClient;
using UAR.Web.Data;
using UAR.Web.Models;

namespace UAR.Web.Services;

public class UserLookupService
{
    private readonly DbConnectionFactory _connectionFactory;

    public UserLookupService(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<UserLookup>> GetAllAsync()
    {
        var results = new List<UserLookup>();
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = new SqlCommand("dbo.Users_GetAll", connection)
        {
            CommandType = System.Data.CommandType.StoredProcedure
        };

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            results.Add(new UserLookup
            {
                Id = reader.GetInt32(0),
                EmployeeId = reader.GetString(1),
                FullName = reader.GetString(2),
                Email = reader.GetString(3),
                Department = reader.GetString(4),
                Title = reader.GetString(5),
                Location = reader.GetString(6),
                IsActive = reader.GetBoolean(7)
            });
        }

        return results;
    }

    public async Task UpsertAsync(UserLookup user)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = new SqlCommand("dbo.Users_Upsert", connection)
        {
            CommandType = System.Data.CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@Id", user.Id);
        command.Parameters.AddWithValue("@EmployeeId", user.EmployeeId);
        command.Parameters.AddWithValue("@FullName", user.FullName);
        command.Parameters.AddWithValue("@Email", user.Email);
        command.Parameters.AddWithValue("@Department", user.Department);
        command.Parameters.AddWithValue("@Title", user.Title);
        command.Parameters.AddWithValue("@Location", user.Location);
        command.Parameters.AddWithValue("@IsActive", user.IsActive);
        await command.ExecuteNonQueryAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = new SqlCommand("dbo.Users_Delete", connection)
        {
            CommandType = System.Data.CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@Id", id);
        await command.ExecuteNonQueryAsync();
    }
}
