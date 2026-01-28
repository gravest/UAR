using Microsoft.Data.SqlClient;
using UAR.Web.Data;
using UAR.Web.Models;

namespace UAR.Web.Services;

public class DropdownService
{
    private readonly DbConnectionFactory _connectionFactory;

    public DropdownService(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<DropdownOption>> GetOptionsAsync(string category)
    {
        var results = new List<DropdownOption>();
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = new SqlCommand("dbo.DropdownOptions_GetByCategory", connection)
        {
            CommandType = System.Data.CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@Category", category);

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            results.Add(new DropdownOption
            {
                Id = reader.GetInt32(0),
                Category = reader.GetString(1),
                Value = reader.GetString(2),
                SortOrder = reader.GetInt32(3),
                IsActive = reader.GetBoolean(4)
            });
        }

        return results;
    }

    public async Task<IReadOnlyList<DropdownOption>> GetAllAsync()
    {
        var results = new List<DropdownOption>();
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = new SqlCommand("dbo.DropdownOptions_GetAll", connection)
        {
            CommandType = System.Data.CommandType.StoredProcedure
        };

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            results.Add(new DropdownOption
            {
                Id = reader.GetInt32(0),
                Category = reader.GetString(1),
                Value = reader.GetString(2),
                SortOrder = reader.GetInt32(3),
                IsActive = reader.GetBoolean(4)
            });
        }

        return results;
    }

    public async Task UpsertAsync(DropdownOption option)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = new SqlCommand("dbo.DropdownOptions_Upsert", connection)
        {
            CommandType = System.Data.CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@Id", option.Id);
        command.Parameters.AddWithValue("@Category", option.Category);
        command.Parameters.AddWithValue("@Value", option.Value);
        command.Parameters.AddWithValue("@SortOrder", option.SortOrder);
        command.Parameters.AddWithValue("@IsActive", option.IsActive);
        await command.ExecuteNonQueryAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = new SqlCommand("dbo.DropdownOptions_Delete", connection)
        {
            CommandType = System.Data.CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@Id", id);
        await command.ExecuteNonQueryAsync();
    }
}
