using Microsoft.AspNetCore.Mvc.RazorPages;
using UAR.Web.Models;
using UAR.Web.Services;

namespace UAR.Web.Pages.Requests;

public class IndexModel : PageModel
{
    private readonly RequestService _requestService;

    public IndexModel(RequestService requestService)
    {
        _requestService = requestService;
    }

    public IReadOnlyList<UarRequest> Requests { get; private set; } = Array.Empty<UarRequest>();

    public async Task OnGetAsync()
    {
        Requests = await _requestService.GetAllAsync();
    }
}
