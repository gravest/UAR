using Microsoft.Data.SqlClient;
using UAR.Web.Data;
using UAR.Web.Models;

namespace UAR.Web.Services;

public class ProgramLookupService
{
    private readonly DbConnectionFactory _connectionFactory;

    public ProgramLookupService(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<ProgramLookup>> GetAllAsync()
    {
        var results = new List<ProgramLookup>();
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = new SqlCommand("dbo.Programs_GetAll", connection)
        {
            CommandType = System.Data.CommandType.StoredProcedure
        };

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            results.Add(new ProgramLookup
            {
                Id = reader.GetInt32(0),
                ProgramCode = reader.GetString(1),
                ProgramName = reader.GetString(2),
                Description = reader.GetString(3),
                IsActive = reader.GetBoolean(4)
            });
        }

        return results;
    }

    public async Task UpsertAsync(ProgramLookup program)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = new SqlCommand("dbo.Programs_Upsert", connection)
        {
            CommandType = System.Data.CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@Id", program.Id);
        command.Parameters.AddWithValue("@ProgramCode", program.ProgramCode);
        command.Parameters.AddWithValue("@ProgramName", program.ProgramName);
        command.Parameters.AddWithValue("@Description", program.Description);
        command.Parameters.AddWithValue("@IsActive", program.IsActive);
        await command.ExecuteNonQueryAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = new SqlCommand("dbo.Programs_Delete", connection)
        {
            CommandType = System.Data.CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@Id", id);
        await command.ExecuteNonQueryAsync();
    }
}
