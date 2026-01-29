using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UAR.Web.Models;
using UAR.Web.Services;

namespace UAR.Web.Pages.Requests;

public class EditModel : RequestFormPageModel
{
    private static readonly HashSet<string> TerminalStatuses = new(StringComparer.OrdinalIgnoreCase)
    {
        "Rejected",
        "Approved",
        "Completed"
    };
    private readonly DropdownService _dropdownService;
    private readonly ProgramLookupService _programService;
    private readonly RequestService _requestService;

    public EditModel(DropdownService dropdownService, ProgramLookupService programService, RequestService requestService)
    {
        _dropdownService = dropdownService;
        _programService = programService;
        _requestService = requestService;
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

        if (IsTerminalStatus(existingRequest.Status))
        {
            ModelState.AddModelError(string.Empty, $"This request is {existingRequest.Status} and can no longer be updated.");
            return Page();
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        RequestForm.EmployeeDeviceTypes = string.Join(", ", SelectedEmployeeDeviceTypes);
        RequestForm.AdditionalMicrosoftProducts = string.Join(", ", SelectedAdditionalMicrosoftProducts);
        RequestForm.KronosAccessTypes = string.Join(", ", SelectedKronosAccessTypes);
        RequestForm.AdditionalLawsonAccess = string.Join(", ", SelectedAdditionalLawsonAccess);

        await _requestService.UpdateAsync(RequestForm);
        return RedirectToPage("/Requests/Details", new { id = RequestForm.Id });
    }

    public bool IsTerminalStatus(string? status)
    {
        return !string.IsNullOrWhiteSpace(status) && TerminalStatuses.Contains(status);
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
}
