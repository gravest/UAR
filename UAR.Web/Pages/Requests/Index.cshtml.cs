using Microsoft.AspNetCore.Mvc;
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

    [BindProperty(SupportsGet = true)]
    public string? RequestNumberQuery { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SubmitterQuery { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? ApproverQuery { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? RdoApproverQuery { get; set; }

    public IReadOnlyList<UarRequest> Requests { get; private set; } = Array.Empty<UarRequest>();

    public async Task OnGetAsync()
    {
        var requests = await _requestService.GetAllAsync();
        Requests = requests
            .Where(request =>
                Matches(RequestNumberQuery, request.RequestNumber) &&
                MatchesAny(SubmitterQuery, request.ProgramAdministratorName, request.ProgramAdministratorEmail) &&
                MatchesAny(ApproverQuery, request.AuthorizedApproverName, request.AuthorizedApproverEmail) &&
                MatchesAny(RdoApproverQuery, request.RdoApproverName, request.RdoApproverEmail))
            .ToList();
    }

    private static bool Matches(string? query, string? value)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return true;
        }

        return !string.IsNullOrWhiteSpace(value)
            && value.Contains(query.Trim(), StringComparison.OrdinalIgnoreCase);
    }

    private static bool MatchesAny(string? query, params string?[] values)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return true;
        }

        return values.Any(value => Matches(query, value));
    }
}
