using UAR.Web.Models;

namespace UAR.Web.Services;

public static class ApprovalWorkflow
{
    public const string ApprovedStatus = "Approved";
    public const string PendingManagerApprovalStatus = "Pending Manager Approval";
    public const string PendingRdoApprovalStatus = "Pending RDO Approval";
    public const string RejectedStatus = "Rejected";
    public const string SubmitForApprovalStatus = "Submit for Approval";

    public static bool IsSubmittingForApproval(UarRequest request)
    {
        return string.Equals(request.SubmitForApproval, "Yes", StringComparison.OrdinalIgnoreCase)
            || string.Equals(request.Status, SubmitForApprovalStatus, StringComparison.OrdinalIgnoreCase)
            || string.Equals(request.Status, PendingManagerApprovalStatus, StringComparison.OrdinalIgnoreCase);
    }
}
