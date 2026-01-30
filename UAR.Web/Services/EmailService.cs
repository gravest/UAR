using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using UAR.Web.Models;

namespace UAR.Web.Services;

public class EmailService
{
    private readonly EmailOptions _options;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailOptions> options, ILogger<EmailService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<EmailPreview?> SendApproverEmailAsync(UarRequest request)
    {
        var recipient = request.AuthorizedApprover;
        if (string.IsNullOrWhiteSpace(recipient))
        {
            _logger.LogWarning("Approver email missing for request {RequestNumber}.", request.RequestNumber);
            return null;
        }

        if (!request.ApprovalToken.HasValue || request.ApprovalToken == Guid.Empty)
        {
            _logger.LogWarning("Approval token missing for request {RequestNumber}.", request.RequestNumber);
            return null;
        }

        if (!request.RejectionToken.HasValue || request.RejectionToken == Guid.Empty)
        {
            _logger.LogWarning("Rejection token missing for request {RequestNumber}.", request.RequestNumber);
            return null;
        }

        var approvalLink = BuildLink(_options.Urls.ApprovalBaseUrl, request.ApprovalToken.Value.ToString());
        var rejectionLink = BuildLink(_options.Urls.RejectionBaseUrl, request.RejectionToken.Value.ToString());
        var subject = $"UAR Request {request.RequestNumber} Awaiting Approval";
        var body = $"A user access request has been submitted for approval.\n\n" +
                   $"Employee: {request.EmployeeFirstName} {request.EmployeeLastName}\n" +
                   $"Requested by: {request.ProgramAdministrator}\n" +
                   $"Status: {request.Status}\n\n" +
                   $"Approve: {approvalLink}\n" +
                   (string.IsNullOrWhiteSpace(rejectionLink) ? string.Empty : $"Reject: {rejectionLink}\n");

        if (_options.DebugEmailPopup)
        {
            return new EmailPreview(subject, body, approvalLink, rejectionLink);
        }

        await SendAsync(recipient, subject, body);
        return null;
    }

    public Task SendRdoApprovalRequestAsync(UarRequest request, string baseUrl)
    {
        var recipient = request.RdoApprover;
        if (string.IsNullOrWhiteSpace(recipient))
        {
            _logger.LogWarning("RDO approver email missing for request {RequestNumber}.", request.RequestNumber);
            return Task.CompletedTask;
        }

        var approveUrl = $"{baseUrl}/approvals/rdo/{request.Id}/approve";
        var rejectUrl = $"{baseUrl}/approvals/rdo/{request.Id}/reject";
        var subject = $"UAR Request {request.RequestNumber} Awaiting RDO Approval";
        var body = $"A user access request requires RDO approval.\n\n" +
                   $"Employee: {request.EmployeeFirstName} {request.EmployeeLastName}\n" +
                   $"Requested by: {request.ProgramAdministrator}\n" +
                   $"Status: {request.Status}\n\n" +
                   $"Approve: {approveUrl}\n" +
                   $"Reject: {rejectUrl}";

        return SendAsync(recipient, subject, body);
    }

    public Task SendSubmitterDecisionEmailAsync(UarRequest request, bool approved)
    {
        var recipient = request.ProgramAdministrator;
        if (string.IsNullOrWhiteSpace(recipient))
        {
            _logger.LogWarning("Submitter email missing for request {RequestNumber}.", request.RequestNumber);
            return Task.CompletedTask;
        }

        var detailsLink = BuildLink(_options.Urls.RequestDetailsBaseUrl, request.Id.ToString());
        var subject = approved
            ? $"UAR Request {request.RequestNumber} Approved"
            : $"UAR Request {request.RequestNumber} Rejected";
        var decisionText = approved ? "approved" : "rejected";
        var body = $"Your user access request has been {decisionText}.\n\n" +
                   $"Employee: {request.EmployeeFirstName} {request.EmployeeLastName}\n" +
                   $"Status: {request.Status}\n" +
                   (approved ? string.Empty : $"Rejection reason: {request.RejectionReason}\n") +
                   $"\nView details: {detailsLink}";

        return SendAsync(recipient, subject, body);
    }

    public Task SendFulfillmentEmailAsync(UarRequest request)
    {
        var recipient = _options.FulfillmentRecipient;
        if (string.IsNullOrWhiteSpace(recipient))
        {
            _logger.LogWarning("Fulfillment recipient missing for request {RequestNumber}.", request.RequestNumber);
            return Task.CompletedTask;
        }

        var subject = $"UAR Request {request.RequestNumber} Approved - Fulfillment Needed";
        var requestedItems = BuildRequestedItems(request);
        var body = $"The following user access request was approved and is ready for fulfillment.\n\n" +
                   $"Employee: {request.EmployeeFirstName} {request.EmployeeLastName}\n" +
                   $"Program(s): {request.Program1} {request.Program2} {request.Program3}\n" +
                   $"\nRequested items:\n{requestedItems}";

        return SendAsync(recipient, subject, body);
    }

    private async Task SendAsync(string recipient, string subject, string body)
    {
        if (!IsConfigured())
        {
            _logger.LogWarning("Email settings are not configured. Skipping email '{Subject}'.", subject);
            return;
        }

        using var message = new MailMessage
        {
            From = new MailAddress(_options.From.Address, _options.From.Name),
            Subject = subject,
            Body = body
        };
        message.To.Add(recipient);

        using var client = new SmtpClient(_options.Smtp.Host, _options.Smtp.Port)
        {
            EnableSsl = _options.Smtp.EnableSsl
        };

        if (!string.IsNullOrWhiteSpace(_options.Smtp.UserName))
        {
            client.Credentials = new NetworkCredential(_options.Smtp.UserName, _options.Smtp.Password);
        }

        await client.SendMailAsync(message);
    }

    private bool IsConfigured()
    {
        return !string.IsNullOrWhiteSpace(_options.Smtp.Host)
            && !string.IsNullOrWhiteSpace(_options.From.Address);
    }

    private static string BuildRequestedItems(UarRequest request)
    {
        var items = new List<string>();
        AddItemIfPresent(items, "Email Account", request.EmailAccount);
        AddItemIfPresent(items, "Account Enable/Disable", request.AccountEnableDisable, "No change");
        AddItemIfPresent(items, "Forwarding Email Address", request.ForwardingEmailAddress);
        AddItemIfPresent(items, "Office 365 License", request.Office365License);
        AddItemIfPresent(items, "Additional Microsoft Products", request.AdditionalMicrosoftProducts);
        AddItemIfPresent(items, "Create Email Groups", request.CreateEmailGroups);
        AddItemIfPresent(items, "Existing Email Groups", request.ExistingEmailGroups);
        AddItemIfPresent(items, "SharePoint Site URL", request.SharePointSiteUrl);
        AddItemIfPresent(items, "SharePoint Site Description", request.SharePointSiteDescription);
        AddItemIfPresent(items, "Additional SharePoint Access", request.AdditionalSharepointAccess);
        AddItemIfPresent(items, "Network Shared Drives", request.NetworkSharedDrives);
        AddItemIfPresent(items, "TOP Access", request.TopAccess);
        AddItemIfPresent(items, "Kronos Access Types", request.KronosAccessTypes);
        AddItemIfPresent(items, "Kronos Access Details", request.KronosAccessDetails);
        AddItemIfPresent(items, "Lawson Access", request.LawsonAccess);
        AddItemIfPresent(items, "Additional Lawson Access", request.AdditionalLawsonAccess);
        AddItemIfPresent(items, "Mileage Access", request.MileageAccess);
        AddItemIfPresent(items, "Comerica Banking Access", request.ComericaBankingAccess);
        AddItemIfPresent(items, "Trust Rep Payee Access", request.TrustRepPayeeAccess);
        AddItemIfPresent(items, "Quickbooks Access", request.QuickbooksAccess);
        AddItemIfPresent(items, "CoWorker Access", request.CoWorkerAccess);
        AddItemIfPresent(items, "Additional HR/Finance Access", request.AdditionalHrFinanceAccess);
        AddItemIfPresent(items, "OrderConnect Access", request.OrderConnectAccess);
        AddItemIfPresent(items, "Caminar Access", request.CaminarAccess);
        AddItemIfPresent(items, "Avatar Access", request.AvatarAccess);
        AddItemIfPresent(items, "Adverse Event Supervisor Access", request.AdverseEventSupervisorAccess);
        AddItemIfPresent(items, "Adverse Event Additional Program Access", request.AdverseEventAdditionalProgramAccess);
        AddItemIfPresent(items, "Business Intelligence Role", request.BusinessIntelligenceRole);
        AddItemIfPresent(items, "Adverse Event Non-Employee Access", request.AdverseEventNonEmployeeAccess);
        AddItemIfPresent(items, "Telehealth Access", request.TelehealthAccess);
        AddItemIfPresent(items, "Next Telehealth Session", request.NextTelehealthSession);
        AddItemIfPresent(items, "Pharmerica Access", request.PharmericaAccess);
        AddItemIfPresent(items, "Pharmerica User Role", request.PharmericaUserRole);
        AddItemIfPresent(items, "Pharmerica Username", request.PharmericaUsername);
        AddItemIfPresent(items, "Call Center Access", request.CallCenterAccess);
        AddItemIfPresent(items, "Adobe Sign Account", request.AdobeSignAccount);
        AddItemIfPresent(items, "Copilot License", request.CopilotLicense);
        AddItemIfPresent(items, "Smartsheet License", request.SmartsheetLicense);
        AddItemIfPresent(items, "eFax", request.EFax);

        if (items.Count == 0)
        {
            return "(No requested items listed.)";
        }

        return string.Join(Environment.NewLine, items.Select(item => $"- {item}"));
    }

    private static void AddItemIfPresent(List<string> items, string label, string? value, string? excludedValue = null)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        if (string.Equals(value, "No", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (!string.IsNullOrWhiteSpace(excludedValue)
            && string.Equals(value, excludedValue, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        items.Add($"{label}: {value}");
    }

    private static string BuildLink(string? baseUrl, string? suffix)
    {
        if (string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(suffix))
        {
            return string.Empty;
        }

        if (baseUrl.EndsWith("=", StringComparison.Ordinal))
        {
            return baseUrl + suffix;
        }

        return baseUrl.EndsWith("/", StringComparison.Ordinal)
            ? $"{baseUrl}{suffix}"
            : $"{baseUrl}/{suffix}";
    }
}

public class EmailOptions
{
    public EmailSmtpOptions Smtp { get; set; } = new();
    public EmailFromOptions From { get; set; } = new();
    public EmailUrlOptions Urls { get; set; } = new();
    public string FulfillmentRecipient { get; set; } = string.Empty;
    public bool DebugEmailPopup { get; set; }
}

public class EmailSmtpOptions
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 587;
    public bool EnableSsl { get; set; } = true;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class EmailFromOptions
{
    public string Address { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class EmailUrlOptions
{
    public string ApprovalBaseUrl { get; set; } = string.Empty;
    public string RejectionBaseUrl { get; set; } = string.Empty;
    public string RequestDetailsBaseUrl { get; set; } = string.Empty;
}

public record EmailPreview(string Subject, string Body, string? ApprovalLink, string? RejectionLink);
