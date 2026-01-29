using Microsoft.AspNetCore.Mvc.RazorPages;
using UAR.Web.Models;

namespace UAR.Web.Pages.Requests;

public abstract class RequestFormPageModel : PageModel
{
    [Microsoft.AspNetCore.Mvc.BindProperty]
    public UarRequest RequestForm { get; set; } = new();

    [Microsoft.AspNetCore.Mvc.BindProperty]
    public string[] SelectedEmployeeDeviceTypes { get; set; } = Array.Empty<string>();

    [Microsoft.AspNetCore.Mvc.BindProperty]
    public string[] SelectedAdditionalMicrosoftProducts { get; set; } = Array.Empty<string>();

    [Microsoft.AspNetCore.Mvc.BindProperty]
    public string[] SelectedKronosAccessTypes { get; set; } = Array.Empty<string>();

    [Microsoft.AspNetCore.Mvc.BindProperty]
    public string[] SelectedAdditionalLawsonAccess { get; set; } = Array.Empty<string>();

    public IReadOnlyList<DropdownOption> Companies { get; set; } = Array.Empty<DropdownOption>();
    public IReadOnlyList<DropdownOption> EmployeeStatuses { get; set; } = Array.Empty<DropdownOption>();
    public IReadOnlyList<DropdownOption> EmployeeTypes { get; set; } = Array.Empty<DropdownOption>();
    public IReadOnlyList<DropdownOption> DeviceTypes { get; set; } = Array.Empty<DropdownOption>();
    public IReadOnlyList<DropdownOption> Statuses { get; set; } = Array.Empty<DropdownOption>();
    public IReadOnlyList<DropdownOption> Office365Licenses { get; set; } = Array.Empty<DropdownOption>();
    public IReadOnlyList<DropdownOption> AdditionalMicrosoftProductOptions { get; set; } = Array.Empty<DropdownOption>();
    public IReadOnlyList<DropdownOption> BusinessIntelligenceRoles { get; set; } = Array.Empty<DropdownOption>();
    public IReadOnlyList<DropdownOption> PharmericaUserRoles { get; set; } = Array.Empty<DropdownOption>();
    public IReadOnlyList<ProgramLookup> Programs { get; set; } = Array.Empty<ProgramLookup>();
}
