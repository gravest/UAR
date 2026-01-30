using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UAR.Web.Models;
using UAR.Web.Services;

namespace UAR.Web.Pages.Requests;

public class EditModel : RequestFormPageModel
{
    private readonly DropdownService _dropdownService;
    private readonly ProgramLookupService _programService;
    private readonly RequestService _requestService;
    private readonly EmailService _emailService;

    [BindProperty]
    public string? WorkflowAction { get; set; }

    public EditModel(
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


    public async Task<IActionResult> OnGetAsync(int id)
    {
        await LoadOptionsAsync();
        var request = await _requestService.GetByIdAsync(id);
        if (request is null)
        {
            return NotFound();
        }

        RequestForm = request;
        SelectedEmployeeDeviceTypes = SplitMultiSelect(RequestForm.EmployeeDeviceTypes);
        SelectedAdditionalMicrosoftProducts = SplitMultiSelect(RequestForm.AdditionalMicrosoftProducts);
        SelectedKronosAccessTypes = SplitMultiSelect(RequestForm.KronosAccessTypes);
        SelectedAdditionalLawsonAccess = SplitMultiSelect(RequestForm.AdditionalLawsonAccess);

        if (ApprovalWorkflow.IsFinalStatus(RequestForm.Status))
        {
            return RedirectToPage("/Requests/Details", new { id });
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await LoadOptionsAsync();

        var existingRequest = await _requestService.GetByIdAsync(RequestForm.Id);
        if (existingRequest is null)
        {
            return NotFound();
        }

        ApplyWorkflowStatus(existingRequest.Status);
        ValidateStatusRequirements();

        RequestForm.EmployeeDeviceTypes = string.Join(", ", SelectedEmployeeDeviceTypes);
        RequestForm.AdditionalMicrosoftProducts = string.Join(", ", SelectedAdditionalMicrosoftProducts);
        RequestForm.KronosAccessTypes = string.Join(", ", SelectedKronosAccessTypes);
        RequestForm.AdditionalLawsonAccess = string.Join(", ", SelectedAdditionalLawsonAccess);

        if (!ModelState.IsValid)
        {
            return Page();
        }

        await _requestService.UpdateAsync(RequestForm);

        var preview = await SendWorkflowEmailsAsync(existingRequest, RequestForm);
        if (preview is not null)
        {
            StoreEmailPreview(preview);
        }
        return RedirectToPage("/Requests/Details", new { id = RequestForm.Id });
    }

    private void ApplyWorkflowStatus(string? currentStatus)
    {
        var action = WorkflowAction?.Trim();
        var normalizedCurrentStatus = string.IsNullOrWhiteSpace(currentStatus)
            ? ApprovalWorkflow.DraftStatus
            : currentStatus;

        ModelState.Remove(nameof(RequestForm.Status));
        ModelState.Remove(nameof(RequestForm.SubmitForApproval));

        if (ApprovalWorkflow.IsDraftStatus(normalizedCurrentStatus))
        {
            var targetStatus = string.Equals(action, "SubmitForApproval", StringComparison.OrdinalIgnoreCase)
                ? ApprovalWorkflow.PendingManagerApprovalStatus
                : ApprovalWorkflow.DraftStatus;

            if (!string.IsNullOrWhiteSpace(RequestForm.Status)
                && !string.Equals(RequestForm.Status, targetStatus, StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(nameof(RequestForm.Status),
                    "Draft requests can only move to pending approval when submitted.");
            }

            RequestForm.Status = targetStatus;
            RequestForm.SubmitForApproval = string.Equals(action, "SubmitForApproval", StringComparison.OrdinalIgnoreCase)
                ? "Yes"
                : null;
            return;
        }

        if (string.Equals(action, "SubmitForApproval", StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(nameof(RequestForm.SubmitForApproval),
                "Only draft requests can be submitted for approval.");
        }

        var requestedStatus = ResolveWorkflowStatus(action, normalizedCurrentStatus);

        if (ApprovalWorkflow.IsDraftStatus(requestedStatus))
        {
            ModelState.AddModelError(nameof(RequestForm.Status),
                "Requests cannot be moved back to Draft.");
        }

        if ((ApprovalWorkflow.IsApprovedStatus(requestedStatus)
                || ApprovalWorkflow.IsRejectedStatus(requestedStatus))
            && !ApprovalWorkflow.IsPendingApprovalStatus(normalizedCurrentStatus))
        {
            ModelState.AddModelError(nameof(RequestForm.Status),
                "Only pending requests can be approved or rejected.");
        }

        RequestForm.Status = requestedStatus;
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

    private static string ResolveWorkflowStatus(string? action, string currentStatus)
    {
        if (string.Equals(action, "Approve", StringComparison.OrdinalIgnoreCase))
        {
            return ApprovalWorkflow.ApprovedStatus;
        }

        if (string.Equals(action, "Reject", StringComparison.OrdinalIgnoreCase))
        {
            return ApprovalWorkflow.RejectedStatus;
        }

        if (string.Equals(action, "SavePending", StringComparison.OrdinalIgnoreCase))
        {
            return currentStatus;
        }

        return currentStatus;
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

    private static string[] SplitMultiSelect(string values)
    {
        return values
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    private async Task<EmailPreview?> SendWorkflowEmailsAsync(UarRequest previousRequest, UarRequest updatedRequest)
    {
        EmailPreview? preview = null;
        var wasSubmitted = ApprovalWorkflow.IsSubmittingForApproval(previousRequest);
        var isSubmitted = ApprovalWorkflow.IsSubmittingForApproval(updatedRequest);
        if (!wasSubmitted && isSubmitted)
        {
            preview = await _emailService.SendApproverEmailAsync(updatedRequest);
        }

        var wasFinal = ApprovalWorkflow.IsFinalStatus(previousRequest.Status);
        var isFinal = ApprovalWorkflow.IsFinalStatus(updatedRequest.Status);
        if (!wasFinal && isFinal)
        {
            var approved = ApprovalWorkflow.IsApprovedStatus(updatedRequest.Status);
            await _emailService.SendSubmitterDecisionEmailAsync(updatedRequest, approved);

            if (approved)
            {
                await _emailService.SendFulfillmentEmailAsync(updatedRequest);
            }
        }

        return preview;
    }

    public bool IsTerminalStatus(string? status)
    {
        return ApprovalWorkflow.IsFinalStatus(status);
    }

    private void StoreEmailPreview(EmailPreview preview)
    {
        TempData["DebugEmailSubject"] = preview.Subject;
        TempData["DebugEmailBody"] = preview.Body;
        TempData["DebugEmailApproveUrl"] = preview.ApprovalLink;
        TempData["DebugEmailRejectUrl"] = preview.RejectionLink;
    }
}
