using Microsoft.Data.SqlClient;
using UAR.Web.Data;
using UAR.Web.Models;

namespace UAR.Web.Services;

public class RequestService
{
    private readonly DbConnectionFactory _connectionFactory;

    public RequestService(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<int> CreateAsync(UarRequest request)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = new SqlCommand("dbo.UarRequest_Create", connection)
        {
            CommandType = System.Data.CommandType.StoredProcedure
        };
        AddRequestParameters(command, request);
        var idParameter = new SqlParameter("@Id", System.Data.SqlDbType.Int)
        {
            Direction = System.Data.ParameterDirection.Output
        };
        command.Parameters.Add(idParameter);
        await command.ExecuteNonQueryAsync();
        return (int)idParameter.Value;
    }

    public async Task<IReadOnlyList<UarRequest>> GetAllAsync()
    {
        var results = new List<UarRequest>();
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = new SqlCommand("dbo.UarRequest_GetAll", connection)
        {
            CommandType = System.Data.CommandType.StoredProcedure
        };

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            results.Add(MapRequest(reader));
        }

        return results;
    }

    public async Task<UarRequest?> GetByIdAsync(int id)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = new SqlCommand("dbo.UarRequest_GetById", connection)
        {
            CommandType = System.Data.CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@Id", id);

        await using var reader = await command.ExecuteReaderAsync();
        return await reader.ReadAsync() ? MapRequest(reader) : null;
    }

    public async Task UpdateAsync(UarRequest request)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = new SqlCommand("dbo.UarRequest_Update", connection)
        {
            CommandType = System.Data.CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@Id", request.Id);
        AddRequestParameters(command, request);
        await command.ExecuteNonQueryAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = new SqlCommand("dbo.UarRequest_Delete", connection)
        {
            CommandType = System.Data.CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@Id", id);
        await command.ExecuteNonQueryAsync();
    }

    private static void AddRequestParameters(SqlCommand command, UarRequest request)
    {
        command.Parameters.AddWithValue("@RequestNumber", request.RequestNumber);
        command.Parameters.AddWithValue("@SubmittedOn", request.SubmittedOn);
        command.Parameters.AddWithValue("@EmployeeNameChange", request.EmployeeNameChange);
        command.Parameters.AddWithValue("@Company", request.Company);
        command.Parameters.AddWithValue("@EmployeeFirstName", request.EmployeeFirstName);
        command.Parameters.AddWithValue("@EmployeeMiddleName", request.EmployeeMiddleName);
        command.Parameters.AddWithValue("@EmployeeLastName", request.EmployeeLastName);
        command.Parameters.AddWithValue("@JobTitle", request.JobTitle);
        command.Parameters.AddWithValue("@DesiredEffectiveDate", request.DesiredEffectiveDate);
        command.Parameters.AddWithValue("@RequestorPhoneNumber", request.RequestorPhoneNumber);
        command.Parameters.AddWithValue("@EmployeeStatus", request.EmployeeStatus);
        command.Parameters.AddWithValue("@EmployeeType", request.EmployeeType);
        command.Parameters.AddWithValue("@TransferFromProgram", request.TransferFromProgram);
        command.Parameters.AddWithValue("@LeaveOfAbsence", request.LeaveOfAbsence);
        command.Parameters.AddWithValue("@RegionName", request.RegionName);
        command.Parameters.AddWithValue("@Program1", request.Program1);
        command.Parameters.AddWithValue("@Program2", request.Program2);
        command.Parameters.AddWithValue("@Program3", request.Program3);
        command.Parameters.AddWithValue("@OtherPrograms", request.OtherPrograms);
        command.Parameters.AddWithValue("@EmployeeDeviceTypes", request.EmployeeDeviceTypes);
        command.Parameters.AddWithValue("@OtherDeviceType", request.OtherDeviceType);
        command.Parameters.AddWithValue("@HrRepresentativeName", request.HrRepresentativeName);
        command.Parameters.AddWithValue("@SubmitForApproval", request.SubmitForApproval);
        command.Parameters.AddWithValue("@Status", request.Status);
        command.Parameters.AddWithValue("@AuthorizedApprover", request.AuthorizedApprover);
        command.Parameters.AddWithValue("@ProgramAdministrator", request.ProgramAdministrator);
        command.Parameters.AddWithValue("@RdoApprover", request.RdoApprover);
        command.Parameters.AddWithValue("@RejectionReason", request.RejectionReason);
        command.Parameters.AddWithValue("@EmailAccount", request.EmailAccount);
        command.Parameters.AddWithValue("@AccountEnableDisable", request.AccountEnableDisable);
        command.Parameters.AddWithValue("@ForwardingEmailAddress", request.ForwardingEmailAddress);
        command.Parameters.AddWithValue("@AssignedAccountUsername", request.AssignedAccountUsername);
        command.Parameters.AddWithValue("@Office365License", request.Office365License);
        command.Parameters.AddWithValue("@AdditionalMicrosoftProducts", request.AdditionalMicrosoftProducts);
        command.Parameters.AddWithValue("@CreateEmailGroups", request.CreateEmailGroups);
        command.Parameters.AddWithValue("@SharePointSiteUrl", request.SharePointSiteUrl);
        command.Parameters.AddWithValue("@SharePointSiteDescription", request.SharePointSiteDescription);
        command.Parameters.AddWithValue("@ExistingEmailGroups", request.ExistingEmailGroups);
        command.Parameters.AddWithValue("@AdditionalSharepointAccess", request.AdditionalSharepointAccess);
        command.Parameters.AddWithValue("@NetworkSharedDrives", request.NetworkSharedDrives);
        command.Parameters.AddWithValue("@TopAccess", request.TopAccess);
        command.Parameters.AddWithValue("@KronosAccessTypes", request.KronosAccessTypes);
        command.Parameters.AddWithValue("@KronosAccessDetails", request.KronosAccessDetails);
        command.Parameters.AddWithValue("@LawsonAccess", request.LawsonAccess);
        command.Parameters.AddWithValue("@AdditionalLawsonAccess", request.AdditionalLawsonAccess);
        command.Parameters.AddWithValue("@MileageAccess", request.MileageAccess);
        command.Parameters.AddWithValue("@ComericaBankingAccess", request.ComericaBankingAccess);
        command.Parameters.AddWithValue("@TrustRepPayeeAccess", request.TrustRepPayeeAccess);
        command.Parameters.AddWithValue("@QuickbooksAccess", request.QuickbooksAccess);
        command.Parameters.AddWithValue("@CoWorkerAccess", request.CoWorkerAccess);
        command.Parameters.AddWithValue("@AdditionalHrFinanceAccess", request.AdditionalHrFinanceAccess);
        command.Parameters.AddWithValue("@OrderConnectAccess", request.OrderConnectAccess);
        command.Parameters.AddWithValue("@CaminarAccess", request.CaminarAccess);
        command.Parameters.AddWithValue("@AvatarAccess", request.AvatarAccess);
        command.Parameters.AddWithValue("@AdverseEventSupervisorAccess", request.AdverseEventSupervisorAccess);
        command.Parameters.AddWithValue("@AdverseEventAdditionalProgramAccess", request.AdverseEventAdditionalProgramAccess);
        command.Parameters.AddWithValue("@BusinessIntelligenceRole", request.BusinessIntelligenceRole);
        command.Parameters.AddWithValue("@AdverseEventNonEmployeeAccess", request.AdverseEventNonEmployeeAccess);
        command.Parameters.AddWithValue("@TelehealthAccess", request.TelehealthAccess);
        command.Parameters.AddWithValue("@NextTelehealthSession", request.NextTelehealthSession);
        command.Parameters.AddWithValue("@AdditionalComments", request.AdditionalComments);
        command.Parameters.AddWithValue("@PharmericaAccess", request.PharmericaAccess);
        command.Parameters.AddWithValue("@PharmericaUserRole", request.PharmericaUserRole);
        command.Parameters.AddWithValue("@PharmericaUsername", request.PharmericaUsername);
        command.Parameters.AddWithValue("@CallCenterAccess", request.CallCenterAccess);
        command.Parameters.AddWithValue("@AdobeSignAccount", request.AdobeSignAccount);
        command.Parameters.AddWithValue("@CopilotLicense", request.CopilotLicense);
        command.Parameters.AddWithValue("@SmartsheetLicense", request.SmartsheetLicense);
        command.Parameters.AddWithValue("@EFax", request.EFax);
    }

    private static UarRequest MapRequest(SqlDataReader reader)
    {
        return new UarRequest
        {
            Id = reader.GetInt32(0),
            RequestNumber = reader.GetString(1),
            SubmittedOn = reader.GetDateTime(2),
            EmployeeNameChange = GetStringSafe(reader, 3),
            Company = GetStringSafe(reader, 4),
            EmployeeFirstName = GetStringSafe(reader, 5),
            EmployeeMiddleName = GetStringSafe(reader, 6),
            EmployeeLastName = GetStringSafe(reader, 7),
            JobTitle = GetStringSafe(reader, 8),
            DesiredEffectiveDate = GetStringSafe(reader, 9),
            RequestorPhoneNumber = GetStringSafe(reader, 10),
            EmployeeStatus = GetStringSafe(reader, 11),
            EmployeeType = GetStringSafe(reader, 12),
            TransferFromProgram = GetStringSafe(reader, 13),
            LeaveOfAbsence = GetStringSafe(reader, 14),
            RegionName = GetStringSafe(reader, 15),
            Program1 = GetStringSafe(reader, 16),
            Program2 = GetStringSafe(reader, 17),
            Program3 = GetStringSafe(reader, 18),
            OtherPrograms = GetStringSafe(reader, 19),
            EmployeeDeviceTypes = GetStringSafe(reader, 20),
            OtherDeviceType = GetStringSafe(reader, 21),
            HrRepresentativeName = GetStringSafe(reader, 22),
            SubmitForApproval = GetStringSafe(reader, 23),
            Status = GetStringSafe(reader, 24),
            AuthorizedApprover = GetStringSafe(reader, 25),
            ProgramAdministrator = GetStringSafe(reader, 26),
            RdoApprover = GetStringSafe(reader, 27),
            RejectionReason = GetStringSafe(reader, 28),
            EmailAccount = GetStringSafe(reader, 29),
            AccountEnableDisable = GetStringSafe(reader, 30),
            ForwardingEmailAddress = GetStringSafe(reader, 31),
            AssignedAccountUsername = GetStringSafe(reader, 32),
            Office365License = GetStringSafe(reader, 33),
            AdditionalMicrosoftProducts = GetStringSafe(reader, 34),
            CreateEmailGroups = GetStringSafe(reader, 35),
            SharePointSiteUrl = GetStringSafe(reader, 36),
            SharePointSiteDescription = GetStringSafe(reader, 37),
            ExistingEmailGroups = GetStringSafe(reader, 38),
            AdditionalSharepointAccess = GetStringSafe(reader, 39),
            NetworkSharedDrives = GetStringSafe(reader, 40),
            TopAccess = GetStringSafe(reader, 41),
            KronosAccessTypes = GetStringSafe(reader, 42),
            KronosAccessDetails = GetStringSafe(reader, 43),
            LawsonAccess = GetStringSafe(reader, 44),
            AdditionalLawsonAccess = GetStringSafe(reader, 45),
            MileageAccess = GetStringSafe(reader, 46),
            ComericaBankingAccess = GetStringSafe(reader, 47),
            TrustRepPayeeAccess = GetStringSafe(reader, 48),
            QuickbooksAccess = GetStringSafe(reader, 49),
            CoWorkerAccess = GetStringSafe(reader, 50),
            AdditionalHrFinanceAccess = GetStringSafe(reader, 51),
            OrderConnectAccess = GetStringSafe(reader, 52),
            CaminarAccess = GetStringSafe(reader, 53),
            AvatarAccess = GetStringSafe(reader, 54),
            AdverseEventSupervisorAccess = GetStringSafe(reader, 55),
            AdverseEventAdditionalProgramAccess = GetStringSafe(reader, 56),
            BusinessIntelligenceRole = GetStringSafe(reader, 57),
            AdverseEventNonEmployeeAccess = GetStringSafe(reader, 58),
            TelehealthAccess = GetStringSafe(reader, 59),
            NextTelehealthSession = GetStringSafe(reader, 60),
            AdditionalComments = GetStringSafe(reader, 61),
            PharmericaAccess = GetStringSafe(reader, 62),
            PharmericaUserRole = GetStringSafe(reader, 63),
            PharmericaUsername = GetStringSafe(reader, 64),
            CallCenterAccess = GetStringSafe(reader, 65),
            AdobeSignAccount = GetStringSafe(reader, 66),
            CopilotLicense = GetStringSafe(reader, 67),
            SmartsheetLicense = GetStringSafe(reader, 68),
            EFax = GetStringSafe(reader, 69)
        };
    }

    private static string GetStringSafe(SqlDataReader reader, int ordinal)
    {
        return reader.IsDBNull(ordinal) ? string.Empty : reader.GetString(ordinal);
    }
}
