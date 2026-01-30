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

    public async Task<UarRequest?> GetByApprovalTokenAsync(Guid token)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = new SqlCommand("dbo.UarRequest_GetByApprovalToken", connection)
        {
            CommandType = System.Data.CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@ApprovalToken", token);

        await using var reader = await command.ExecuteReaderAsync();
        return await reader.ReadAsync() ? MapRequest(reader) : null;
    }

    public async Task<UarRequest?> GetByRejectionTokenAsync(Guid token)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = new SqlCommand("dbo.UarRequest_GetByRejectionToken", connection)
        {
            CommandType = System.Data.CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@RejectionToken", token);

        await using var reader = await command.ExecuteReaderAsync();
        return await reader.ReadAsync() ? MapRequest(reader) : null;
    }

    public async Task<bool> ApproveByTokenAsync(Guid token, string approvedBy)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = new SqlCommand("dbo.UarRequest_ApproveByToken", connection)
        {
            CommandType = System.Data.CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@ApprovalToken", token);
        command.Parameters.AddWithValue("@ApprovedBy", approvedBy);
        command.Parameters.AddWithValue("@ApprovedOn", DateTime.UtcNow);
        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> RejectByTokenAsync(Guid token, string rejectedBy, string? rejectionReason)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var command = new SqlCommand("dbo.UarRequest_RejectByToken", connection)
        {
            CommandType = System.Data.CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@RejectionToken", token);
        command.Parameters.AddWithValue("@RejectedBy", rejectedBy);
        command.Parameters.AddWithValue("@RejectedOn", DateTime.UtcNow);
        command.Parameters.AddWithValue("@RejectionReason", ToDbValue(rejectionReason));
        return await command.ExecuteNonQueryAsync() > 0;
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
        command.Parameters.AddWithValue("@RequestNumber", ToDbValue(request.RequestNumber));
        command.Parameters.AddWithValue("@SubmittedOn", request.SubmittedOn);
        command.Parameters.AddWithValue("@EmployeeNameChange", ToDbValue(request.EmployeeNameChange));
        command.Parameters.AddWithValue("@Company", request.Company);
        command.Parameters.AddWithValue("@EmployeeFirstName", request.EmployeeFirstName);
        command.Parameters.AddWithValue("@EmployeeMiddleName", request.EmployeeMiddleName);
        command.Parameters.AddWithValue("@EmployeeLastName", request.EmployeeLastName);
        command.Parameters.AddWithValue("@JobTitle", request.JobTitle);
        command.Parameters.AddWithValue("@DesiredEffectiveDate", ToDbValue(request.DesiredEffectiveDate));
        command.Parameters.AddWithValue("@RequestorPhoneNumber", request.RequestorPhoneNumber);
        command.Parameters.AddWithValue("@EmployeeStatus", request.EmployeeStatus);
        command.Parameters.AddWithValue("@EmployeeType", ToDbValue(request.EmployeeType));
        command.Parameters.AddWithValue("@TransferFromProgram", ToDbValue(request.TransferFromProgram));
        command.Parameters.AddWithValue("@LeaveOfAbsence", ToDbValue(request.LeaveOfAbsence));
        command.Parameters.AddWithValue("@RegionName", ToDbValue(request.RegionName));
        command.Parameters.AddWithValue("@Program1", request.Program1);
        command.Parameters.AddWithValue("@Program2", ToDbValue(request.Program2));
        command.Parameters.AddWithValue("@Program3", ToDbValue(request.Program3));
        command.Parameters.AddWithValue("@OtherPrograms", ToDbValue(request.OtherPrograms));
        command.Parameters.AddWithValue("@EmployeeDeviceTypes", ToDbValue(request.EmployeeDeviceTypes));
        command.Parameters.AddWithValue("@OtherDeviceType", ToDbValue(request.OtherDeviceType));
        command.Parameters.AddWithValue("@HrRepresentativeName", ToDbValue(request.HrRepresentativeName));
        command.Parameters.AddWithValue("@SubmitForApproval", ToDbValue(request.SubmitForApproval));
        command.Parameters.AddWithValue("@Status", request.Status);
        command.Parameters.AddWithValue("@AuthorizedApproverName", request.AuthorizedApproverName);
        command.Parameters.AddWithValue("@AuthorizedApproverEmail", request.AuthorizedApproverEmail);
        command.Parameters.AddWithValue("@ProgramAdministrator", request.ProgramAdministrator);
        command.Parameters.AddWithValue("@RdoApproverName", ToDbValue(request.RdoApproverName));
        command.Parameters.AddWithValue("@RdoApproverEmail", ToDbValue(request.RdoApproverEmail));
        command.Parameters.AddWithValue("@RejectionReason", ToDbValue(request.RejectionReason));
        command.Parameters.AddWithValue("@EmailAccount", ToDbValue(request.EmailAccount));
        command.Parameters.AddWithValue("@AccountEnableDisable", ToDbValue(request.AccountEnableDisable));
        command.Parameters.AddWithValue("@ForwardingEmailAddress", ToDbValue(request.ForwardingEmailAddress));
        command.Parameters.AddWithValue("@AssignedAccountUsername", ToDbValue(request.AssignedAccountUsername));
        command.Parameters.AddWithValue("@Office365License", ToDbValue(request.Office365License));
        command.Parameters.AddWithValue("@AdditionalMicrosoftProducts", ToDbValue(request.AdditionalMicrosoftProducts));
        command.Parameters.AddWithValue("@CreateEmailGroups", ToDbValue(request.CreateEmailGroups));
        command.Parameters.AddWithValue("@SharePointSiteUrl", ToDbValue(request.SharePointSiteUrl));
        command.Parameters.AddWithValue("@SharePointSiteDescription", ToDbValue(request.SharePointSiteDescription));
        command.Parameters.AddWithValue("@ExistingEmailGroups", ToDbValue(request.ExistingEmailGroups));
        command.Parameters.AddWithValue("@AdditionalSharepointAccess", ToDbValue(request.AdditionalSharepointAccess));
        command.Parameters.AddWithValue("@NetworkSharedDrives", ToDbValue(request.NetworkSharedDrives));
        command.Parameters.AddWithValue("@TopAccess", ToDbValue(request.TopAccess));
        command.Parameters.AddWithValue("@KronosAccessTypes", ToDbValue(request.KronosAccessTypes));
        command.Parameters.AddWithValue("@KronosAccessDetails", ToDbValue(request.KronosAccessDetails));
        command.Parameters.AddWithValue("@LawsonAccess", ToDbValue(request.LawsonAccess));
        command.Parameters.AddWithValue("@AdditionalLawsonAccess", ToDbValue(request.AdditionalLawsonAccess));
        command.Parameters.AddWithValue("@MileageAccess", ToDbValue(request.MileageAccess));
        command.Parameters.AddWithValue("@ComericaBankingAccess", ToDbValue(request.ComericaBankingAccess));
        command.Parameters.AddWithValue("@TrustRepPayeeAccess", ToDbValue(request.TrustRepPayeeAccess));
        command.Parameters.AddWithValue("@QuickbooksAccess", ToDbValue(request.QuickbooksAccess));
        command.Parameters.AddWithValue("@CoWorkerAccess", ToDbValue(request.CoWorkerAccess));
        command.Parameters.AddWithValue("@AdditionalHrFinanceAccess", ToDbValue(request.AdditionalHrFinanceAccess));
        command.Parameters.AddWithValue("@OrderConnectAccess", ToDbValue(request.OrderConnectAccess));
        command.Parameters.AddWithValue("@CaminarAccess", ToDbValue(request.CaminarAccess));
        command.Parameters.AddWithValue("@AvatarAccess", ToDbValue(request.AvatarAccess));
        command.Parameters.AddWithValue("@AdverseEventSupervisorAccess", ToDbValue(request.AdverseEventSupervisorAccess));
        command.Parameters.AddWithValue("@AdverseEventAdditionalProgramAccess", ToDbValue(request.AdverseEventAdditionalProgramAccess));
        command.Parameters.AddWithValue("@BusinessIntelligenceRole", ToDbValue(request.BusinessIntelligenceRole));
        command.Parameters.AddWithValue("@AdverseEventNonEmployeeAccess", ToDbValue(request.AdverseEventNonEmployeeAccess));
        command.Parameters.AddWithValue("@TelehealthAccess", ToDbValue(request.TelehealthAccess));
        command.Parameters.AddWithValue("@NextTelehealthSession", ToDbValue(request.NextTelehealthSession));
        command.Parameters.AddWithValue("@AdditionalComments", ToDbValue(request.AdditionalComments));
        command.Parameters.AddWithValue("@PharmericaAccess", ToDbValue(request.PharmericaAccess));
        command.Parameters.AddWithValue("@PharmericaUserRole", ToDbValue(request.PharmericaUserRole));
        command.Parameters.AddWithValue("@PharmericaUsername", ToDbValue(request.PharmericaUsername));
        command.Parameters.AddWithValue("@CallCenterAccess", ToDbValue(request.CallCenterAccess));
        command.Parameters.AddWithValue("@AdobeSignAccount", ToDbValue(request.AdobeSignAccount));
        command.Parameters.AddWithValue("@CopilotLicense", ToDbValue(request.CopilotLicense));
        command.Parameters.AddWithValue("@SmartsheetLicense", ToDbValue(request.SmartsheetLicense));
        command.Parameters.AddWithValue("@EFax", ToDbValue(request.EFax));
        command.Parameters.AddWithValue("@ApprovalToken", ToDbValue(request.ApprovalToken));
        command.Parameters.AddWithValue("@RejectionToken", ToDbValue(request.RejectionToken));
        command.Parameters.AddWithValue("@ApprovalDecision", ToDbValue(request.ApprovalDecision));
        command.Parameters.AddWithValue("@ApprovedOn", ToDbValue(request.ApprovedOn));
        command.Parameters.AddWithValue("@ApprovedBy", ToDbValue(request.ApprovedBy));
        command.Parameters.AddWithValue("@RejectedOn", ToDbValue(request.RejectedOn));
        command.Parameters.AddWithValue("@RejectedBy", ToDbValue(request.RejectedBy));
    }

    private static object ToDbValue(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? DBNull.Value : value;
    }

    private static object ToDbValue(Guid? value)
    {
        return value.HasValue && value.Value != Guid.Empty ? value.Value : DBNull.Value;
    }

    private static object ToDbValue(DateTime? value)
    {
        return value.HasValue ? value.Value : DBNull.Value;
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
            AuthorizedApproverName = GetStringSafe(reader, 25),
            AuthorizedApproverEmail = GetStringSafe(reader, 26),
            ProgramAdministrator = GetStringSafe(reader, 27),
            RdoApproverName = GetStringSafe(reader, 28),
            RdoApproverEmail = GetStringSafe(reader, 29),
            RejectionReason = GetStringSafe(reader, 30),
            EmailAccount = GetStringSafe(reader, 31),
            AccountEnableDisable = GetStringSafe(reader, 32),
            ForwardingEmailAddress = GetStringSafe(reader, 33),
            AssignedAccountUsername = GetStringSafe(reader, 34),
            Office365License = GetStringSafe(reader, 35),
            AdditionalMicrosoftProducts = GetStringSafe(reader, 36),
            CreateEmailGroups = GetStringSafe(reader, 37),
            SharePointSiteUrl = GetStringSafe(reader, 38),
            SharePointSiteDescription = GetStringSafe(reader, 39),
            ExistingEmailGroups = GetStringSafe(reader, 40),
            AdditionalSharepointAccess = GetStringSafe(reader, 41),
            NetworkSharedDrives = GetStringSafe(reader, 42),
            TopAccess = GetStringSafe(reader, 43),
            KronosAccessTypes = GetStringSafe(reader, 44),
            KronosAccessDetails = GetStringSafe(reader, 45),
            LawsonAccess = GetStringSafe(reader, 46),
            AdditionalLawsonAccess = GetStringSafe(reader, 47),
            MileageAccess = GetStringSafe(reader, 48),
            ComericaBankingAccess = GetStringSafe(reader, 49),
            TrustRepPayeeAccess = GetStringSafe(reader, 50),
            QuickbooksAccess = GetStringSafe(reader, 51),
            CoWorkerAccess = GetStringSafe(reader, 52),
            AdditionalHrFinanceAccess = GetStringSafe(reader, 53),
            OrderConnectAccess = GetStringSafe(reader, 54),
            CaminarAccess = GetStringSafe(reader, 55),
            AvatarAccess = GetStringSafe(reader, 56),
            AdverseEventSupervisorAccess = GetStringSafe(reader, 57),
            AdverseEventAdditionalProgramAccess = GetStringSafe(reader, 58),
            BusinessIntelligenceRole = GetStringSafe(reader, 59),
            AdverseEventNonEmployeeAccess = GetStringSafe(reader, 60),
            TelehealthAccess = GetStringSafe(reader, 61),
            NextTelehealthSession = GetStringSafe(reader, 62),
            AdditionalComments = GetStringSafe(reader, 63),
            PharmericaAccess = GetStringSafe(reader, 64),
            PharmericaUserRole = GetStringSafe(reader, 65),
            PharmericaUsername = GetStringSafe(reader, 66),
            CallCenterAccess = GetStringSafe(reader, 67),
            AdobeSignAccount = GetStringSafe(reader, 68),
            CopilotLicense = GetStringSafe(reader, 69),
            SmartsheetLicense = GetStringSafe(reader, 70),
            EFax = GetStringSafe(reader, 71),
            ApprovalToken = GetGuidSafe(reader, 72),
            RejectionToken = GetGuidSafe(reader, 73),
            ApprovalDecision = GetStringSafe(reader, 74),
            ApprovedOn = GetDateTimeSafe(reader, 75),
            ApprovedBy = GetStringSafe(reader, 76),
            RejectedOn = GetDateTimeSafe(reader, 77),
            RejectedBy = GetStringSafe(reader, 78)
        };
    }

    private static string GetStringSafe(SqlDataReader reader, int ordinal)
    {
        return reader.IsDBNull(ordinal) ? string.Empty : reader.GetString(ordinal);
    }

    private static Guid? GetGuidSafe(SqlDataReader reader, int ordinal)
    {
        return reader.IsDBNull(ordinal) ? null : reader.GetGuid(ordinal);
    }

    private static DateTime? GetDateTimeSafe(SqlDataReader reader, int ordinal)
    {
        return reader.IsDBNull(ordinal) ? null : reader.GetDateTime(ordinal);
    }
}
