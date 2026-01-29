using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UAR.Web.Models;
using UAR.Web.Services;

namespace UAR.Web.Pages.Admin;

public class DropdownsModel : PageModel
{
    private readonly DropdownService _dropdownService;

    public DropdownsModel(DropdownService dropdownService)
    {
        _dropdownService = dropdownService;
    }

    public IReadOnlyList<DropdownOption> Options { get; private set; } = Array.Empty<DropdownOption>();

    [BindProperty]
    public DropdownOption NewOption { get; set; } = new() { IsActive = true };

    public async Task OnGetAsync()
    {
        Options = await _dropdownService.GetAllAsync();
    }

    public async Task<IActionResult> OnPostAddAsync()
    {
        if (!ModelState.IsValid)
        {
            Options = await _dropdownService.GetAllAsync();
            return Page();
        }

        await _dropdownService.UpsertAsync(NewOption);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        await _dropdownService.DeleteAsync(id);
        return RedirectToPage();
    }
}
