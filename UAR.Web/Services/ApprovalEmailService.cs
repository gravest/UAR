using Microsoft.Extensions.Logging;
using UAR.Web.Models;

namespace UAR.Web.Services;

public class ApprovalEmailService
{
    private readonly ILogger<ApprovalEmailService> _logger;

    public ApprovalEmailService(ILogger<ApprovalEmailService> logger)
    {
        _logger = logger;
    }

    public Task SendManagerApprovalRequestAsync(UarRequest request, string baseUrl)
    {
        var approveUrl = $"{baseUrl}/approvals/manager/{request.Id}/approve";
        var rejectUrl = $"{baseUrl}/approvals/manager/{request.Id}/reject";

        _logger.LogInformation(
            "Manager approval requested for {RequestNumber}. Approver: {Approver}. Approve: {ApproveUrl}. Reject: {RejectUrl}.",
            request.RequestNumber,
            request.AuthorizedApprover,
            approveUrl,
            rejectUrl);

        return Task.CompletedTask;
    }

    public Task SendRdoApprovalRequestAsync(UarRequest request, string baseUrl)
    {
        var approveUrl = $"{baseUrl}/approvals/rdo/{request.Id}/approve";
        var rejectUrl = $"{baseUrl}/approvals/rdo/{request.Id}/reject";

        _logger.LogInformation(
            "RDO approval requested for {RequestNumber}. Approver: {Approver}. Approve: {ApproveUrl}. Reject: {RejectUrl}.",
            request.RequestNumber,
            request.RdoApprover,
            approveUrl,
            rejectUrl);

        return Task.CompletedTask;
    }
}
