using Microsoft.AspNetCore.Mvc.RazorPages;
using UAR.Web.Models;
using UAR.Web.Services;

namespace UAR.Web.Pages.Requests;

public class RejectModel : PageModel
{
    private readonly RequestService _requestService;

    public RejectModel(RequestService requestService)
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
            ErrorMessage = "Invalid rejection token.";
            return;
        }

        RequestDetails = await _requestService.GetByRejectionTokenAsync(token);
        if (RequestDetails == null)
        {
            ErrorMessage = "Request not found for this rejection link.";
            return;
        }

        if (string.Equals(RequestDetails.ApprovalDecision, "Rejected", StringComparison.OrdinalIgnoreCase))
        {
            Updated = false;
            return;
        }

        if (string.Equals(RequestDetails.ApprovalDecision, "Approved", StringComparison.OrdinalIgnoreCase))
        {
            ErrorMessage = "This request has already been approved.";
            return;
        }

        var approver = string.IsNullOrWhiteSpace(RequestDetails.AuthorizedApprover)
            ? "Authorized Approver"
            : RequestDetails.AuthorizedApprover;
        var rejectionReason = string.IsNullOrWhiteSpace(RequestDetails.RejectionReason)
            ? "Rejected via email link."
            : RequestDetails.RejectionReason;

        Updated = await _requestService.RejectByTokenAsync(token, approver, rejectionReason);
        if (Updated)
        {
            RequestDetails = await _requestService.GetByRejectionTokenAsync(token);
        }
    }
}
