-- Seed dropdown options used by the request form.
-- Safe to re-run: skips values that already exist.

DECLARE @Options TABLE (
    Category NVARCHAR(100),
    Value NVARCHAR(200),
    SortOrder INT
);

INSERT INTO @Options (Category, Value, SortOrder)
VALUES
    (N'Company', N'Telecare Corporation', 10),
    (N'Company', N'TLC', 20),

    (N'EmployeeStatus', N'New', 10),
    (N'EmployeeStatus', N'Existing', 20),
    (N'EmployeeStatus', N'Terminated', 30),
    (N'EmployeeStatus', N'Transfer', 40),
    (N'EmployeeStatus', N'Rehire', 50),

    (N'EmployeeType', N'Employee', 10),
    (N'EmployeeType', N'Contractor', 20),
    (N'EmployeeType', N'Intern', 30),

    (N'EmployeeNameChange', N'Yes', 10),
    (N'EmployeeNameChange', N'No', 20),

    (N'TransferFromProgram', N'Yes, REMOVE existing program data access', 10),
    (N'TransferFromProgram', N'Yes, RETAIN existing program data access', 20),
    (N'TransferFromProgram', N'No', 30),

    (N'LeaveOfAbsence', N'Yes', 10),
    (N'LeaveOfAbsence', N'No', 20),

    (N'DeviceType', N'Ipad', 10),
    (N'DeviceType', N'Phone', 20),
    (N'DeviceType', N'Mifi', 30),

    (N'Status', N'Saved As Draft', 10),
    (N'Status', N'Submit for Approval', 20),
    (N'Status', N'Pending Manager Approval', 30),
    (N'Status', N'Approved', 40),
    (N'Status', N'Pending RDO Approval', 50),
    (N'Status', N'Rejected', 60),
    (N'Status', N'Completed', 70),

    (N'SubmitForApproval', N'Yes', 10),
    (N'SubmitForApproval', N'No', 20),

    (N'EmailAccount', N'Yes', 10),
    (N'EmailAccount', N'No', 20),

    (N'AccountEnableDisable', N'No change', 10),
    (N'AccountEnableDisable', N'Enable', 20),
    (N'AccountEnableDisable', N'Disable', 30),

    (N'Office365License', N'O365 F3 License', 10),
    (N'Office365License', N'O365 E3 License', 20),

    (N'AdditionalMicrosoftProduct', N'Project', 10),
    (N'AdditionalMicrosoftProduct', N'Visio', 20),
    (N'AdditionalMicrosoftProduct', N'Power BI Pro', 30),
    (N'AdditionalMicrosoftProduct', N'Teams Phone', 40),

    (N'CreateEmailGroups', N'Yes', 10),
    (N'CreateEmailGroups', N'No', 20),

    (N'TopAccess', N'Yes', 10),
    (N'TopAccess', N'No', 20),

    (N'LawsonAccess', N'Yes', 10),
    (N'LawsonAccess', N'No', 20),

    (N'MileageAccess', N'Yes', 10),
    (N'MileageAccess', N'No', 20),

    (N'ComericaBankingAccess', N'Yes', 10),
    (N'ComericaBankingAccess', N'No', 20),

    (N'TrustRepPayeeAccess', N'Yes', 10),
    (N'TrustRepPayeeAccess', N'No', 20),

    (N'QuickbooksAccess', N'Yes', 10),
    (N'QuickbooksAccess', N'No', 20),

    (N'CoWorkerAccess', N'Yes', 10),
    (N'CoWorkerAccess', N'No', 20),

    (N'OrderConnectAccess', N'Yes', 10),
    (N'OrderConnectAccess', N'No', 20),

    (N'CaminarAccess', N'Yes', 10),
    (N'CaminarAccess', N'No', 20),

    (N'AvatarAccess', N'Yes', 10),
    (N'AvatarAccess', N'No', 20),

    (N'AdverseEventSupervisorAccess', N'Yes', 10),
    (N'AdverseEventSupervisorAccess', N'No', 20),

    (N'AdverseEventAdditionalProgramAccess', N'Yes', 10),
    (N'AdverseEventAdditionalProgramAccess', N'No', 20),

    (N'BusinessIntelligenceRole', N'Telepsychiatry Provider', 10),
    (N'BusinessIntelligenceRole', N'Program Staff', 20),

    (N'AdverseEventNonEmployeeAccess', N'Yes', 10),
    (N'AdverseEventNonEmployeeAccess', N'No', 20),

    (N'TelehealthAccess', N'Yes', 10),
    (N'TelehealthAccess', N'No', 20),

    (N'PharmericaAccess', N'Yes', 10),
    (N'PharmericaAccess', N'No', 20),

    (N'PharmericaUserRole', N'RN', 10),
    (N'PharmericaUserRole', N'LVN', 20),

    (N'CallCenterAccess', N'Yes', 10),
    (N'CallCenterAccess', N'No', 20),

    (N'AdobeSignAccount', N'Yes', 10),
    (N'AdobeSignAccount', N'No', 20),

    (N'CopilotLicense', N'Yes', 10),
    (N'CopilotLicense', N'No', 20),

    (N'SmartsheetLicense', N'Yes', 10),
    (N'SmartsheetLicense', N'No', 20),

    (N'EFax', N'Yes', 10),
    (N'EFax', N'No', 20);

INSERT INTO dbo.DropdownOptions (Category, Value, SortOrder, IsActive)
SELECT o.Category,
       o.Value,
       o.SortOrder,
       1
FROM @Options o
WHERE NOT EXISTS (
    SELECT 1
    FROM dbo.DropdownOptions existing
    WHERE existing.Category = o.Category
      AND existing.Value = o.Value
);

DECLARE @Programs TABLE (
    ProgramCode NVARCHAR(50),
    ProgramName NVARCHAR(200),
    Description NVARCHAR(500)
);

INSERT INTO @Programs (ProgramCode, ProgramName, Description)
VALUES
    (N'001', N'Program 1', N'Sample program entry for initial setup.'),
    (N'002', N'Program 2', N'Sample program entry for initial setup.'),
    (N'003', N'Program 3', N'Sample program entry for initial setup.');

INSERT INTO dbo.Programs (ProgramCode, ProgramName, Description, IsActive)
SELECT p.ProgramCode,
       p.ProgramName,
       p.Description,
       1
FROM @Programs p
WHERE NOT EXISTS (
    SELECT 1
    FROM dbo.Programs existing
    WHERE existing.ProgramCode = p.ProgramCode
       OR existing.ProgramName = p.ProgramName
);
