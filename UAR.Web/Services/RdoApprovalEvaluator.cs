using UAR.Web.Models;

namespace UAR.Web.Services;

public static class RdoApprovalEvaluator
{
    public static bool RequiresRdoApproval(UarRequest request)
    {
        if (request is null)
        {
            return false;
        }

        if (string.Equals(request.Office365License, "O365 E3 License", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return GetAccessSelections(request)
            .Any(value => value.Contains("RDO", StringComparison.OrdinalIgnoreCase));
    }

    private static IEnumerable<string> GetAccessSelections(UarRequest request)
    {
        var values = new[]
        {
            request.Office365License,
            request.AdditionalMicrosoftProducts,
            request.CreateEmailGroups,
            request.ExistingEmailGroups,
            request.AdditionalSharepointAccess,
            request.NetworkSharedDrives,
            request.TopAccess,
            request.KronosAccessTypes,
            request.KronosAccessDetails,
            request.LawsonAccess,
            request.AdditionalLawsonAccess,
            request.MileageAccess,
            request.ComericaBankingAccess,
            request.TrustRepPayeeAccess,
            request.QuickbooksAccess,
            request.CoWorkerAccess,
            request.AdditionalHrFinanceAccess,
            request.OrderConnectAccess,
            request.CaminarAccess,
            request.AvatarAccess,
            request.AdverseEventSupervisorAccess,
            request.AdverseEventAdditionalProgramAccess,
            request.BusinessIntelligenceRole,
            request.AdverseEventNonEmployeeAccess,
            request.TelehealthAccess,
            request.PharmericaAccess,
            request.PharmericaUserRole,
            request.CallCenterAccess,
            request.AdobeSignAccount,
            request.CopilotLicense,
            request.SmartsheetLicense,
            request.EFax
        };

        foreach (var value in values)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                continue;
            }

            foreach (var selection in value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                yield return selection;
            }
        }
    }
}
