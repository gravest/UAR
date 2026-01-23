using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UAR.Web.Models;
using UAR.Web.Services;

namespace UAR.Web.Pages.Admin;

public class UsersModel : PageModel
{
    private readonly UserLookupService _userService;

    public UsersModel(UserLookupService userService)
    {
        _userService = userService;
    }

    public IReadOnlyList<UserLookup> Users { get; private set; } = Array.Empty<UserLookup>();

    [BindProperty]
    public UserLookup NewUser { get; set; } = new() { IsActive = true };

    public async Task OnGetAsync()
    {
        Users = await _userService.GetAllAsync();
    }

    public async Task<IActionResult> OnPostAddAsync()
    {
        if (!ModelState.IsValid)
        {
            Users = await _userService.GetAllAsync();
            return Page();
        }

        await _userService.UpsertAsync(NewUser);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        await _userService.DeleteAsync(id);
        return RedirectToPage();
    }
}
