using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UAR.Web.Models;
using UAR.Web.Services;

namespace UAR.Web.Pages.Admin;

public class ProgramsModel : PageModel
{
    private readonly ProgramLookupService _programService;

    public ProgramsModel(ProgramLookupService programService)
    {
        _programService = programService;
    }

    public IReadOnlyList<ProgramLookup> Programs { get; private set; } = Array.Empty<ProgramLookup>();

    [BindProperty]
    public ProgramLookup NewProgram { get; set; } = new() { IsActive = true };

    public async Task OnGetAsync()
    {
        Programs = await _programService.GetAllAsync();
    }

    public async Task<IActionResult> OnPostAddAsync()
    {
        if (!ModelState.IsValid)
        {
            Programs = await _programService.GetAllAsync();
            return Page();
        }

        await _programService.UpsertAsync(NewProgram);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        await _programService.DeleteAsync(id);
        return RedirectToPage();
    }
}
