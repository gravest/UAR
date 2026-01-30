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

        await SendWorkflowEmailsAsync(existingRequest, RequestForm);
        return RedirectToPage("/Requests/Details", new { id = RequestForm.Id });
    }

    private void ApplyWorkflowStatus(string? currentStatus)
    {
        var submitRequested = IsSubmitRequested(RequestForm.SubmitForApproval);
        var normalizedCurrentStatus = string.IsNullOrWhiteSpace(currentStatus)
            ? ApprovalWorkflow.DraftStatus
            : currentStatus;

        ModelState.Remove(nameof(RequestForm.Status));
        ModelState.Remove(nameof(RequestForm.SubmitForApproval));

        if (ApprovalWorkflow.IsDraftStatus(normalizedCurrentStatus))
        {
            var targetStatus = submitRequested
                ? ApprovalWorkflow.PendingManagerApprovalStatus
                : ApprovalWorkflow.DraftStatus;

            if (!string.IsNullOrWhiteSpace(RequestForm.Status)
                && !string.Equals(RequestForm.Status, targetStatus, StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(nameof(RequestForm.Status),
                    "Draft requests can only move to pending approval when submitted.");
            }

            RequestForm.Status = targetStatus;
            RequestForm.SubmitForApproval = null;
            return;
        }

        if (submitRequested)
        {
            ModelState.AddModelError(nameof(RequestForm.SubmitForApproval),
                "Only draft requests can be submitted for approval.");
        }

        var requestedStatus = string.IsNullOrWhiteSpace(RequestForm.Status)
            ? normalizedCurrentStatus
            : RequestForm.Status;

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

    private static bool IsSubmitRequested(string? submitValue)
    {
        return string.Equals(submitValue, "Yes", StringComparison.OrdinalIgnoreCase);
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

    private async Task SendWorkflowEmailsAsync(UarRequest previousRequest, UarRequest updatedRequest)
    {
        var wasSubmitted = ApprovalWorkflow.IsSubmittingForApproval(previousRequest);
        var isSubmitted = ApprovalWorkflow.IsSubmittingForApproval(updatedRequest);
        if (!wasSubmitted && isSubmitted)
        {
            await _emailService.SendApproverEmailAsync(updatedRequest);
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
    }
}
