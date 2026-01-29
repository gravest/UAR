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

    public CreateModel(DropdownService dropdownService, ProgramLookupService programService, RequestService requestService)
    {
        _dropdownService = dropdownService;
        _programService = programService;
        _requestService = requestService;
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
        if (string.IsNullOrWhiteSpace(RequestForm.Status))
        {
            RequestForm.Status = "Saved As Draft";
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await LoadOptionsAsync();

        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (string.IsNullOrWhiteSpace(RequestForm.Status))
        {
            RequestForm.Status = "Saved As Draft";
        }

        RequestForm.RequestNumber = $"UAR-{DateTime.UtcNow:yyyyMMddHHmmss}";
        RequestForm.SubmittedOn = DateTime.UtcNow;
        RequestForm.EmployeeDeviceTypes = string.Join(", ", SelectedEmployeeDeviceTypes);
        RequestForm.AdditionalMicrosoftProducts = string.Join(", ", SelectedAdditionalMicrosoftProducts);
        RequestForm.KronosAccessTypes = string.Join(", ", SelectedKronosAccessTypes);
        RequestForm.AdditionalLawsonAccess = string.Join(", ", SelectedAdditionalLawsonAccess);

        var id = await _requestService.CreateAsync(RequestForm);
        return RedirectToPage("/Requests/Details", new { id });
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
