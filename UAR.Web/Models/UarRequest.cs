using System.ComponentModel.DataAnnotations;

namespace UAR.Web.Models;

public class UarRequest
{
    public int Id { get; set; }

    public string? RequestNumber { get; set; }

    public DateTime SubmittedOn { get; set; }

    public string? EmployeeNameChange { get; set; }

    [Required]
    public string Company { get; set; } = string.Empty;

    [Required]
    public string EmployeeFirstName { get; set; } = string.Empty;

    [Required]
    public string EmployeeMiddleName { get; set; } = string.Empty;

    [Required]
    public string EmployeeLastName { get; set; } = string.Empty;

    [Required]
    public string JobTitle { get; set; } = string.Empty;

    public string? DesiredEffectiveDate { get; set; }

    [Required]
    public string RequestorPhoneNumber { get; set; } = string.Empty;

    [Required]
    public string EmployeeStatus { get; set; } = string.Empty;

    public string? EmployeeType { get; set; }

    public string? TransferFromProgram { get; set; }

    public string? LeaveOfAbsence { get; set; }

    public string? RegionName { get; set; }

    [Required]
    public string Program1 { get; set; } = string.Empty;

    public string? Program2 { get; set; }

    public string? Program3 { get; set; }

    public string? OtherPrograms { get; set; }

    public string? EmployeeDeviceTypes { get; set; }

    public string? OtherDeviceType { get; set; }

    public string? HrRepresentativeName { get; set; }

    public string? SubmitForApproval { get; set; }

    [Required]
    public string Status { get; set; } = string.Empty;

    [Required]
    public string AuthorizedApproverName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string AuthorizedApproverEmail { get; set; } = string.Empty;

    [Required]
    public string ProgramAdministrator { get; set; } = string.Empty;

    public string? RdoApproverName { get; set; }

    [EmailAddress]
    public string? RdoApproverEmail { get; set; }

    public string? RejectionReason { get; set; }

    public string? EmailAccount { get; set; }

    public string? AccountEnableDisable { get; set; }

    public string? ForwardingEmailAddress { get; set; }

    public string? AssignedAccountUsername { get; set; }

    public string? Office365License { get; set; }

    public string? AdditionalMicrosoftProducts { get; set; }

    public string? CreateEmailGroups { get; set; }

    public string? SharePointSiteUrl { get; set; }

    public string? SharePointSiteDescription { get; set; }

    public string? ExistingEmailGroups { get; set; }

    public string? AdditionalSharepointAccess { get; set; }

    public string? NetworkSharedDrives { get; set; }

    public string? TopAccess { get; set; }

    public string? KronosAccessTypes { get; set; }

    public string? KronosAccessDetails { get; set; }

    public string? LawsonAccess { get; set; }

    public string? AdditionalLawsonAccess { get; set; }

    public string? MileageAccess { get; set; }

    public string? ComericaBankingAccess { get; set; }

    public string? TrustRepPayeeAccess { get; set; }

    public string? QuickbooksAccess { get; set; }

    public string? CoWorkerAccess { get; set; }

    public string? AdditionalHrFinanceAccess { get; set; }

    public string? OrderConnectAccess { get; set; }

    public string? CaminarAccess { get; set; }

    public string? AvatarAccess { get; set; }

    public string? AdverseEventSupervisorAccess { get; set; }

    public string? AdverseEventAdditionalProgramAccess { get; set; }

    public string? BusinessIntelligenceRole { get; set; }

    public string? AdverseEventNonEmployeeAccess { get; set; }

    public string? TelehealthAccess { get; set; }

    public string? NextTelehealthSession { get; set; }

    public string? AdditionalComments { get; set; }

    public string? PharmericaAccess { get; set; }

    public string? PharmericaUserRole { get; set; }

    public string? PharmericaUsername { get; set; }

    public string? CallCenterAccess { get; set; }

    public string? AdobeSignAccount { get; set; }

    public string? CopilotLicense { get; set; }

    public string? SmartsheetLicense { get; set; }

    public string? EFax { get; set; }

    public Guid? ApprovalToken { get; set; }

    public Guid? RejectionToken { get; set; }

    public string? ApprovalDecision { get; set; }

    public DateTime? ApprovedOn { get; set; }

    public string? ApprovedBy { get; set; }

    public DateTime? RejectedOn { get; set; }

    public string? RejectedBy { get; set; }
}
