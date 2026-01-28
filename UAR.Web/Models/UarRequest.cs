using System.ComponentModel.DataAnnotations;

namespace UAR.Web.Models;

public class UarRequest
{
    public int Id { get; set; }

    public string RequestNumber { get; set; } = string.Empty;

    public DateTime SubmittedOn { get; set; }

    public string EmployeeNameChange { get; set; } = string.Empty;

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

    public string DesiredEffectiveDate { get; set; } = string.Empty;

    [Required]
    public string RequestorPhoneNumber { get; set; } = string.Empty;

    [Required]
    public string EmployeeStatus { get; set; } = string.Empty;

    public string EmployeeType { get; set; } = string.Empty;

    public string TransferFromProgram { get; set; } = string.Empty;

    public string LeaveOfAbsence { get; set; } = string.Empty;

    public string RegionName { get; set; } = string.Empty;

    [Required]
    public string Program1 { get; set; } = string.Empty;

    public string Program2 { get; set; } = string.Empty;

    public string Program3 { get; set; } = string.Empty;

    public string OtherPrograms { get; set; } = string.Empty;

    public string EmployeeDeviceTypes { get; set; } = string.Empty;

    public string OtherDeviceType { get; set; } = string.Empty;

    public string HrRepresentativeName { get; set; } = string.Empty;

    public string SubmitForApproval { get; set; } = string.Empty;

    [Required]
    public string Status { get; set; } = string.Empty;

    [Required]
    public string AuthorizedApprover { get; set; } = string.Empty;

    [Required]
    public string ProgramAdministrator { get; set; } = string.Empty;

    public string RdoApprover { get; set; } = string.Empty;

    public string RejectionReason { get; set; } = string.Empty;

    public string EmailAccount { get; set; } = string.Empty;

    public string AccountEnableDisable { get; set; } = string.Empty;

    public string ForwardingEmailAddress { get; set; } = string.Empty;

    public string AssignedAccountUsername { get; set; } = string.Empty;

    public string Office365License { get; set; } = string.Empty;

    public string AdditionalMicrosoftProducts { get; set; } = string.Empty;

    public string CreateEmailGroups { get; set; } = string.Empty;

    public string SharePointSiteUrl { get; set; } = string.Empty;

    public string SharePointSiteDescription { get; set; } = string.Empty;

    public string ExistingEmailGroups { get; set; } = string.Empty;

    public string AdditionalSharepointAccess { get; set; } = string.Empty;

    public string NetworkSharedDrives { get; set; } = string.Empty;

    public string TopAccess { get; set; } = string.Empty;

    public string KronosAccessTypes { get; set; } = string.Empty;

    public string KronosAccessDetails { get; set; } = string.Empty;

    public string LawsonAccess { get; set; } = string.Empty;

    public string AdditionalLawsonAccess { get; set; } = string.Empty;

    public string MileageAccess { get; set; } = string.Empty;

    public string ComericaBankingAccess { get; set; } = string.Empty;

    public string TrustRepPayeeAccess { get; set; } = string.Empty;

    public string QuickbooksAccess { get; set; } = string.Empty;

    public string CoWorkerAccess { get; set; } = string.Empty;

    public string AdditionalHrFinanceAccess { get; set; } = string.Empty;

    public string OrderConnectAccess { get; set; } = string.Empty;

    public string CaminarAccess { get; set; } = string.Empty;

    public string AvatarAccess { get; set; } = string.Empty;

    public string AdverseEventSupervisorAccess { get; set; } = string.Empty;

    public string AdverseEventAdditionalProgramAccess { get; set; } = string.Empty;

    public string BusinessIntelligenceRole { get; set; } = string.Empty;

    public string AdverseEventNonEmployeeAccess { get; set; } = string.Empty;

    public string TelehealthAccess { get; set; } = string.Empty;

    public string NextTelehealthSession { get; set; } = string.Empty;

    public string AdditionalComments { get; set; } = string.Empty;

    public string PharmericaAccess { get; set; } = string.Empty;

    public string PharmericaUserRole { get; set; } = string.Empty;

    public string PharmericaUsername { get; set; } = string.Empty;

    public string CallCenterAccess { get; set; } = string.Empty;

    public string AdobeSignAccount { get; set; } = string.Empty;

    public string CopilotLicense { get; set; } = string.Empty;

    public string SmartsheetLicense { get; set; } = string.Empty;

    public string EFax { get; set; } = string.Empty;
}
