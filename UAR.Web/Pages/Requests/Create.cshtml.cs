using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UAR.Web.Models;
using UAR.Web.Services;

namespace UAR.Web.Pages.Requests;

public class CreateModel : PageModel
{
    private readonly DropdownService _dropdownService;
    private readonly ProgramLookupService _programService;
    private readonly RequestService _requestService;
    private readonly EmailService _emailService;

    public CreateModel(
        DropdownService dropdownService,
        ProgramLookupService programService,
        RequestService requestService,
        EmailService emailService)
    {
        _dropdownService = dropdownService;
        _programService = programService;
        _requestService = requestService;
        _emailService = emailService;
    }

    [BindProperty]
    public UarRequest RequestForm { get; set; } = new();

    [BindProperty]
    public string[] SelectedEmployeeDeviceTypes { get; set; } = Array.Empty<string>();

    [BindProperty]
    public string[] SelectedAdditionalMicrosoftProducts { get; set; } = Array.Empty<string>();

    [BindProperty]
    public string[] SelectedKronosAccessTypes { get; set; } = Array.Empty<string>();

    [BindProperty]
    public string[] SelectedAdditionalLawsonAccess { get; set; } = Array.Empty<string>();

    public IReadOnlyList<DropdownOption> Companies { get; private set; } = Array.Empty<DropdownOption>();
    public IReadOnlyList<DropdownOption> EmployeeStatuses { get; private set; } = Array.Empty<DropdownOption>();
    public IReadOnlyList<DropdownOption> EmployeeTypes { get; private set; } = Array.Empty<DropdownOption>();
    public IReadOnlyList<DropdownOption> DeviceTypes { get; private set; } = Array.Empty<DropdownOption>();
    public IReadOnlyList<DropdownOption> Statuses { get; private set; } = Array.Empty<DropdownOption>();
    public IReadOnlyList<DropdownOption> Office365Licenses { get; private set; } = Array.Empty<DropdownOption>();
    public IReadOnlyList<DropdownOption> AdditionalMicrosoftProductOptions { get; private set; } = Array.Empty<DropdownOption>();
    public IReadOnlyList<DropdownOption> BusinessIntelligenceRoles { get; private set; } = Array.Empty<DropdownOption>();
    public IReadOnlyList<DropdownOption> PharmericaUserRoles { get; private set; } = Array.Empty<DropdownOption>();
    public IReadOnlyList<ProgramLookup> Programs { get; private set; } = Array.Empty<ProgramLookup>();

    public async Task OnGetAsync()
    {
        await LoadOptionsAsync();
        SetDefaultDesiredEffectiveDate();
        RequestForm.Status = ApprovalWorkflow.DraftStatus;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await LoadOptionsAsync();
        ApplyWorkflowStatus();
        ValidateStatusRequirements();
        SetDefaultDesiredEffectiveDate();

        RequestForm.EmployeeDeviceTypes = string.Join(", ", SelectedEmployeeDeviceTypes);
        RequestForm.AdditionalMicrosoftProducts = string.Join(", ", SelectedAdditionalMicrosoftProducts);
        RequestForm.KronosAccessTypes = string.Join(", ", SelectedKronosAccessTypes);
        RequestForm.AdditionalLawsonAccess = string.Join(", ", SelectedAdditionalLawsonAccess);

        if (ApprovalWorkflow.IsSubmittingForApproval(RequestForm)
            && RdoApprovalEvaluator.RequiresRdoApproval(RequestForm)
            && string.IsNullOrWhiteSpace(RequestForm.RdoApprover))
        {
            ModelState.AddModelError("RequestForm.RdoApprover", "RDO Approver is required when RDO approval is needed.");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        RequestForm.RequestNumber = $"UAR-{DateTime.UtcNow:yyyyMMddHHmmss}";
        RequestForm.SubmittedOn = DateTime.UtcNow;
        RequestForm.ApprovalToken = Guid.NewGuid();
        RequestForm.RejectionToken = Guid.NewGuid();
        RequestForm.ApprovalDecision = "Pending";
        RequestForm.EmployeeDeviceTypes = string.Join(", ", SelectedEmployeeDeviceTypes);
        RequestForm.AdditionalMicrosoftProducts = string.Join(", ", SelectedAdditionalMicrosoftProducts);
        RequestForm.KronosAccessTypes = string.Join(", ", SelectedKronosAccessTypes);
        RequestForm.AdditionalLawsonAccess = string.Join(", ", SelectedAdditionalLawsonAccess);

        var id = await _requestService.CreateAsync(RequestForm);
        RequestForm.Id = id;

        if (ApprovalWorkflow.IsSubmittingForApproval(RequestForm))
        {
            await _emailService.SendApproverEmailAsync(RequestForm);
        }

        return RedirectToPage("/Requests/Details", new { id });
    }

    private void ApplyWorkflowStatus()
    {
        var submitRequested = IsSubmitRequested(RequestForm.SubmitForApproval);
        var targetStatus = submitRequested
            ? ApprovalWorkflow.PendingManagerApprovalStatus
            : ApprovalWorkflow.DraftStatus;

        ModelState.Remove(nameof(RequestForm.Status));
        ModelState.Remove(nameof(RequestForm.SubmitForApproval));

        if (!string.IsNullOrWhiteSpace(RequestForm.Status)
            && !string.Equals(RequestForm.Status, targetStatus, StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(nameof(RequestForm.Status),
                "Draft requests can only move to pending approval when submitted.");
        }

        RequestForm.Status = targetStatus;
        RequestForm.SubmitForApproval = null;
    }

    private void ValidateStatusRequirements()
    {
        if (ApprovalWorkflow.IsRejectedStatus(RequestForm.Status)
            && string.IsNullOrWhiteSpace(RequestForm.RejectionReason))
        {
            ModelState.AddModelError(nameof(RequestForm.RejectionReason),
                "Rejection reason is required when a request is rejected.");
        }
    }

    private static bool IsSubmitRequested(string? submitValue)
    {
        return string.Equals(submitValue, "Yes", StringComparison.OrdinalIgnoreCase);
    }

    private void SetDefaultDesiredEffectiveDate()
    {
        if (string.IsNullOrWhiteSpace(RequestForm.DesiredEffectiveDate))
        {
            RequestForm.DesiredEffectiveDate = DateTime.Today.AddDays(5).ToString("yyyy-MM-dd");
        }
    }

    private async Task LoadOptionsAsync()
    {
        Companies = await _dropdownService.GetOptionsAsync("Company");
        EmployeeStatuses = await _dropdownService.GetOptionsAsync("EmployeeStatus");
        EmployeeTypes = await _dropdownService.GetOptionsAsync("EmployeeType");
        DeviceTypes = await _dropdownService.GetOptionsAsync("DeviceType");
        Statuses = await _dropdownService.GetOptionsAsync("Status");
        Office365Licenses = await _dropdownService.GetOptionsAsync("Office365License");
        AdditionalMicrosoftProductOptions = await _dropdownService.GetOptionsAsync("AdditionalMicrosoftProduct");
        BusinessIntelligenceRoles = await _dropdownService.GetOptionsAsync("BusinessIntelligenceRole");
        PharmericaUserRoles = await _dropdownService.GetOptionsAsync("PharmericaUserRole");
        Programs = await _programService.GetAllAsync();
    }

}
