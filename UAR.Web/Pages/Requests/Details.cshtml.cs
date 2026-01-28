using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UAR.Web.Models;
using UAR.Web.Services;

namespace UAR.Web.Pages.Requests;

public class DetailsModel : PageModel
{
    private readonly RequestService _requestService;

    public DetailsModel(RequestService requestService)
    {
        _requestService = requestService;
    }

    public UarRequest? RequestDetails { get; private set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        RequestDetails = await _requestService.GetByIdAsync(id);
        if (RequestDetails is null)
        {
            return NotFound();
        }

        return Page();
    }
}
