using UAR.Web.Models;

namespace UAR.Web.Services;

public static class ApprovalWorkflow
{
    public const string DraftStatus = "Saved As Draft";
    public const string ApprovedStatus = "Approved";
    public const string PendingManagerApprovalStatus = "Pending Manager Approval";
    public const string PendingRdoApprovalStatus = "Pending RDO Approval";
    public const string RejectedStatus = "Rejected";
    public const string SubmitForApprovalStatus = "Submit for Approval";

    public static bool IsSubmittingForApproval(UarRequest request)
    {
        return string.Equals(request.SubmitForApproval, "Yes", StringComparison.OrdinalIgnoreCase)
            || IsPendingApprovalStatus(request.Status);
    }

    public static bool IsDraftStatus(string? status)
    {
        return string.Equals(status, DraftStatus, StringComparison.OrdinalIgnoreCase)
            || string.Equals(status, "Draft", StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsPendingApprovalStatus(string? status)
    {
        return string.Equals(status, PendingManagerApprovalStatus, StringComparison.OrdinalIgnoreCase)
            || string.Equals(status, PendingRdoApprovalStatus, StringComparison.OrdinalIgnoreCase)
            || string.Equals(status, "Pending Approval", StringComparison.OrdinalIgnoreCase)
            || string.Equals(status, SubmitForApprovalStatus, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsApprovedStatus(string? status)
    {
        return string.Equals(status, ApprovedStatus, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsRejectedStatus(string? status)
    {
        return string.Equals(status, RejectedStatus, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsFinalStatus(string? status)
    {
        return IsApprovedStatus(status) || IsRejectedStatus(status);
    }
}
