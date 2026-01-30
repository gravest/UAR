using Microsoft.AspNetCore.Mvc.RazorPages;
using UAR.Web.Models;
using UAR.Web.Services;

namespace UAR.Web.Pages.Requests;

public class ApproveModel : PageModel
{
    private readonly RequestService _requestService;

    public ApproveModel(RequestService requestService)
    {
        _requestService = requestService;
    }

    public UarRequest? RequestDetails { get; private set; }

    public string? ErrorMessage { get; private set; }

    public bool Updated { get; private set; }

    public async Task OnGetAsync(Guid token)
    {
        if (token == Guid.Empty)
        {
            ErrorMessage = "Invalid approval token.";
            return;
        }

        RequestDetails = await _requestService.GetByApprovalTokenAsync(token);
        if (RequestDetails == null)
        {
            ErrorMessage = "Request not found for this approval link.";
            return;
        }

        if (string.Equals(RequestDetails.ApprovalDecision, "Approved", StringComparison.OrdinalIgnoreCase))
        {
            Updated = false;
            return;
        }

        if (string.Equals(RequestDetails.ApprovalDecision, "Rejected", StringComparison.OrdinalIgnoreCase))
        {
            ErrorMessage = "This request has already been rejected.";
            return;
        }

        var approver = string.IsNullOrWhiteSpace(RequestDetails.AuthorizedApprover)
            ? "Authorized Approver"
            : RequestDetails.AuthorizedApprover;

        Updated = await _requestService.ApproveByTokenAsync(token, approver);
        if (Updated)
        {
            RequestDetails = await _requestService.GetByApprovalTokenAsync(token);
        }
    }
}
