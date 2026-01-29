CREATE OR ALTER PROCEDURE dbo.DropdownOptions_GetByCategory
    @Category NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Category, Value, SortOrder, IsActive
    FROM dbo.DropdownOptions
    WHERE Category = @Category AND IsActive = 1
    ORDER BY SortOrder, Value;
END
GO

CREATE OR ALTER PROCEDURE dbo.DropdownOptions_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Category, Value, SortOrder, IsActive
    FROM dbo.DropdownOptions
    ORDER BY Category, SortOrder, Value;
END
GO

CREATE OR ALTER PROCEDURE dbo.DropdownOptions_Upsert
    @Id INT,
    @Category NVARCHAR(100),
    @Value NVARCHAR(200),
    @SortOrder INT,
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;
    IF @Id = 0
    BEGIN
        INSERT INTO dbo.DropdownOptions (Category, Value, SortOrder, IsActive)
        VALUES (@Category, @Value, @SortOrder, @IsActive);
    END
    ELSE
    BEGIN
        UPDATE dbo.DropdownOptions
        SET Category = @Category,
            Value = @Value,
            SortOrder = @SortOrder,
            IsActive = @IsActive
        WHERE Id = @Id;
    END
END
GO

CREATE OR ALTER PROCEDURE dbo.DropdownOptions_Delete
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.DropdownOptions WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE dbo.Users_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, EmployeeId, FullName, Email, Department, Title, Location, IsActive
    FROM dbo.Users
    ORDER BY FullName;
END
GO

CREATE OR ALTER PROCEDURE dbo.Users_Upsert
    @Id INT,
    @EmployeeId NVARCHAR(50),
    @FullName NVARCHAR(200),
    @Email NVARCHAR(200),
    @Department NVARCHAR(200),
    @Title NVARCHAR(200),
    @Location NVARCHAR(200),
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;
    IF @Id = 0
    BEGIN
        INSERT INTO dbo.Users (EmployeeId, FullName, Email, Department, Title, Location, IsActive)
        VALUES (@EmployeeId, @FullName, @Email, @Department, @Title, @Location, @IsActive);
    END
    ELSE
    BEGIN
        UPDATE dbo.Users
        SET EmployeeId = @EmployeeId,
            FullName = @FullName,
            Email = @Email,
            Department = @Department,
            Title = @Title,
            Location = @Location,
            IsActive = @IsActive
        WHERE Id = @Id;
    END
END
GO

CREATE OR ALTER PROCEDURE dbo.Users_Delete
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.Users WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE dbo.Programs_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, ProgramCode, ProgramName, Description, IsActive
    FROM dbo.Programs
    ORDER BY ProgramName;
END
GO

CREATE OR ALTER PROCEDURE dbo.Programs_Upsert
    @Id INT,
    @ProgramCode NVARCHAR(50),
    @ProgramName NVARCHAR(200),
    @Description NVARCHAR(500),
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;
    IF @Id = 0
    BEGIN
        INSERT INTO dbo.Programs (ProgramCode, ProgramName, Description, IsActive)
        VALUES (@ProgramCode, @ProgramName, @Description, @IsActive);
    END
    ELSE
    BEGIN
        UPDATE dbo.Programs
        SET ProgramCode = @ProgramCode,
            ProgramName = @ProgramName,
            Description = @Description,
            IsActive = @IsActive
        WHERE Id = @Id;
    END
END
GO

CREATE OR ALTER PROCEDURE dbo.Programs_Delete
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.Programs WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE dbo.UarRequest_Create
    @RequestNumber NVARCHAR(50),
    @SubmittedOn DATETIME2,
    @EmployeeNameChange NVARCHAR(20),
    @Company NVARCHAR(200),
    @EmployeeFirstName NVARCHAR(100),
    @EmployeeMiddleName NVARCHAR(50),
    @EmployeeLastName NVARCHAR(100),
    @JobTitle NVARCHAR(200),
    @DesiredEffectiveDate NVARCHAR(50),
    @RequestorPhoneNumber NVARCHAR(50),
    @EmployeeStatus NVARCHAR(100),
    @EmployeeType NVARCHAR(100),
    @TransferFromProgram NVARCHAR(20),
    @LeaveOfAbsence NVARCHAR(20),
    @RegionName NVARCHAR(200),
    @Program1 NVARCHAR(200),
    @Program2 NVARCHAR(200),
    @Program3 NVARCHAR(200),
    @OtherPrograms NVARCHAR(MAX),
    @EmployeeDeviceTypes NVARCHAR(MAX),
    @OtherDeviceType NVARCHAR(200),
    @HrRepresentativeName NVARCHAR(200),
    @SubmitForApproval NVARCHAR(20),
    @Status NVARCHAR(100),
    @AuthorizedApprover NVARCHAR(200),
    @ProgramAdministrator NVARCHAR(200),
    @RdoApprover NVARCHAR(200),
    @RejectionReason NVARCHAR(MAX),
    @EmailAccount NVARCHAR(20),
    @AccountEnableDisable NVARCHAR(50),
    @ForwardingEmailAddress NVARCHAR(200),
    @AssignedAccountUsername NVARCHAR(200),
    @Office365License NVARCHAR(200),
    @AdditionalMicrosoftProducts NVARCHAR(MAX),
    @CreateEmailGroups NVARCHAR(20),
    @SharePointSiteUrl NVARCHAR(500),
    @SharePointSiteDescription NVARCHAR(200),
    @ExistingEmailGroups NVARCHAR(MAX),
    @AdditionalSharepointAccess NVARCHAR(MAX),
    @NetworkSharedDrives NVARCHAR(MAX),
    @TopAccess NVARCHAR(20),
    @KronosAccessTypes NVARCHAR(MAX),
    @KronosAccessDetails NVARCHAR(MAX),
    @LawsonAccess NVARCHAR(20),
    @AdditionalLawsonAccess NVARCHAR(MAX),
    @MileageAccess NVARCHAR(20),
    @ComericaBankingAccess NVARCHAR(20),
    @TrustRepPayeeAccess NVARCHAR(20),
    @QuickbooksAccess NVARCHAR(20),
    @CoWorkerAccess NVARCHAR(20),
    @AdditionalHrFinanceAccess NVARCHAR(MAX),
    @OrderConnectAccess NVARCHAR(20),
    @CaminarAccess NVARCHAR(20),
    @AvatarAccess NVARCHAR(20),
    @AdverseEventSupervisorAccess NVARCHAR(20),
    @AdverseEventAdditionalProgramAccess NVARCHAR(20),
    @BusinessIntelligenceRole NVARCHAR(200),
    @AdverseEventNonEmployeeAccess NVARCHAR(20),
    @TelehealthAccess NVARCHAR(20),
    @NextTelehealthSession NVARCHAR(100),
    @AdditionalComments NVARCHAR(MAX),
    @PharmericaAccess NVARCHAR(20),
    @PharmericaUserRole NVARCHAR(200),
    @PharmericaUsername NVARCHAR(200),
    @CallCenterAccess NVARCHAR(20),
    @AdobeSignAccount NVARCHAR(20),
    @CopilotLicense NVARCHAR(20),
    @SmartsheetLicense NVARCHAR(20),
    @EFax NVARCHAR(20),
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.UarRequests (
        RequestNumber, SubmittedOn, EmployeeNameChange, Company, EmployeeFirstName, EmployeeMiddleName,
        EmployeeLastName, JobTitle, DesiredEffectiveDate, RequestorPhoneNumber, EmployeeStatus,
        EmployeeType, TransferFromProgram, LeaveOfAbsence, RegionName, Program1, Program2, Program3,
        OtherPrograms, EmployeeDeviceTypes, OtherDeviceType, HrRepresentativeName, SubmitForApproval,
        Status, AuthorizedApprover, ProgramAdministrator, RdoApprover, RejectionReason, EmailAccount,
        AccountEnableDisable, ForwardingEmailAddress, AssignedAccountUsername, Office365License,
        AdditionalMicrosoftProducts, CreateEmailGroups, SharePointSiteUrl, SharePointSiteDescription,
        ExistingEmailGroups, AdditionalSharepointAccess, NetworkSharedDrives, TopAccess,
        KronosAccessTypes, KronosAccessDetails, LawsonAccess, AdditionalLawsonAccess, MileageAccess,
        ComericaBankingAccess, TrustRepPayeeAccess, QuickbooksAccess, CoWorkerAccess,
        AdditionalHrFinanceAccess, OrderConnectAccess, CaminarAccess, AvatarAccess,
        AdverseEventSupervisorAccess, AdverseEventAdditionalProgramAccess, BusinessIntelligenceRole,
        AdverseEventNonEmployeeAccess, TelehealthAccess, NextTelehealthSession, AdditionalComments,
        PharmericaAccess, PharmericaUserRole, PharmericaUsername, CallCenterAccess, AdobeSignAccount,
        CopilotLicense, SmartsheetLicense, EFax
    )
    VALUES (
        @RequestNumber, @SubmittedOn, @EmployeeNameChange, @Company, @EmployeeFirstName, @EmployeeMiddleName,
        @EmployeeLastName, @JobTitle, @DesiredEffectiveDate, @RequestorPhoneNumber, @EmployeeStatus,
        @EmployeeType, @TransferFromProgram, @LeaveOfAbsence, @RegionName, @Program1, @Program2, @Program3,
        @OtherPrograms, @EmployeeDeviceTypes, @OtherDeviceType, @HrRepresentativeName, @SubmitForApproval,
        @Status, @AuthorizedApprover, @ProgramAdministrator, @RdoApprover, @RejectionReason, @EmailAccount,
        @AccountEnableDisable, @ForwardingEmailAddress, @AssignedAccountUsername, @Office365License,
        @AdditionalMicrosoftProducts, @CreateEmailGroups, @SharePointSiteUrl, @SharePointSiteDescription,
        @ExistingEmailGroups, @AdditionalSharepointAccess, @NetworkSharedDrives, @TopAccess,
        @KronosAccessTypes, @KronosAccessDetails, @LawsonAccess, @AdditionalLawsonAccess, @MileageAccess,
        @ComericaBankingAccess, @TrustRepPayeeAccess, @QuickbooksAccess, @CoWorkerAccess,
        @AdditionalHrFinanceAccess, @OrderConnectAccess, @CaminarAccess, @AvatarAccess,
        @AdverseEventSupervisorAccess, @AdverseEventAdditionalProgramAccess, @BusinessIntelligenceRole,
        @AdverseEventNonEmployeeAccess, @TelehealthAccess, @NextTelehealthSession, @AdditionalComments,
        @PharmericaAccess, @PharmericaUserRole, @PharmericaUsername, @CallCenterAccess, @AdobeSignAccount,
        @CopilotLicense, @SmartsheetLicense, @EFax
    );
    SET @Id = SCOPE_IDENTITY();
END
GO

CREATE OR ALTER PROCEDURE dbo.UarRequest_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, RequestNumber, SubmittedOn, EmployeeNameChange, Company, EmployeeFirstName, EmployeeMiddleName,
           EmployeeLastName, JobTitle, DesiredEffectiveDate, RequestorPhoneNumber, EmployeeStatus,
           EmployeeType, TransferFromProgram, LeaveOfAbsence, RegionName, Program1, Program2, Program3,
           OtherPrograms, EmployeeDeviceTypes, OtherDeviceType, HrRepresentativeName, SubmitForApproval,
           Status, AuthorizedApprover, ProgramAdministrator, RdoApprover, RejectionReason, EmailAccount,
           AccountEnableDisable, ForwardingEmailAddress, AssignedAccountUsername, Office365License,
           AdditionalMicrosoftProducts, CreateEmailGroups, SharePointSiteUrl, SharePointSiteDescription,
           ExistingEmailGroups, AdditionalSharepointAccess, NetworkSharedDrives, TopAccess,
           KronosAccessTypes, KronosAccessDetails, LawsonAccess, AdditionalLawsonAccess, MileageAccess,
           ComericaBankingAccess, TrustRepPayeeAccess, QuickbooksAccess, CoWorkerAccess,
           AdditionalHrFinanceAccess, OrderConnectAccess, CaminarAccess, AvatarAccess,
           AdverseEventSupervisorAccess, AdverseEventAdditionalProgramAccess, BusinessIntelligenceRole,
           AdverseEventNonEmployeeAccess, TelehealthAccess, NextTelehealthSession, AdditionalComments,
           PharmericaAccess, PharmericaUserRole, PharmericaUsername, CallCenterAccess, AdobeSignAccount,
           CopilotLicense, SmartsheetLicense, EFax
    FROM dbo.UarRequests
    ORDER BY SubmittedOn DESC;
END
GO

CREATE OR ALTER PROCEDURE dbo.UarRequest_GetById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, RequestNumber, SubmittedOn, EmployeeNameChange, Company, EmployeeFirstName, EmployeeMiddleName,
           EmployeeLastName, JobTitle, DesiredEffectiveDate, RequestorPhoneNumber, EmployeeStatus,
           EmployeeType, TransferFromProgram, LeaveOfAbsence, RegionName, Program1, Program2, Program3,
           OtherPrograms, EmployeeDeviceTypes, OtherDeviceType, HrRepresentativeName, SubmitForApproval,
           Status, AuthorizedApprover, ProgramAdministrator, RdoApprover, RejectionReason, EmailAccount,
           AccountEnableDisable, ForwardingEmailAddress, AssignedAccountUsername, Office365License,
           AdditionalMicrosoftProducts, CreateEmailGroups, SharePointSiteUrl, SharePointSiteDescription,
           ExistingEmailGroups, AdditionalSharepointAccess, NetworkSharedDrives, TopAccess,
           KronosAccessTypes, KronosAccessDetails, LawsonAccess, AdditionalLawsonAccess, MileageAccess,
           ComericaBankingAccess, TrustRepPayeeAccess, QuickbooksAccess, CoWorkerAccess,
           AdditionalHrFinanceAccess, OrderConnectAccess, CaminarAccess, AvatarAccess,
           AdverseEventSupervisorAccess, AdverseEventAdditionalProgramAccess, BusinessIntelligenceRole,
           AdverseEventNonEmployeeAccess, TelehealthAccess, NextTelehealthSession, AdditionalComments,
           PharmericaAccess, PharmericaUserRole, PharmericaUsername, CallCenterAccess, AdobeSignAccount,
           CopilotLicense, SmartsheetLicense, EFax
    FROM dbo.UarRequests
    WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE dbo.UarRequest_Update
    @Id INT,
    @RequestNumber NVARCHAR(50),
    @SubmittedOn DATETIME2,
    @EmployeeNameChange NVARCHAR(20),
    @Company NVARCHAR(200),
    @EmployeeFirstName NVARCHAR(100),
    @EmployeeMiddleName NVARCHAR(50),
    @EmployeeLastName NVARCHAR(100),
    @JobTitle NVARCHAR(200),
    @DesiredEffectiveDate NVARCHAR(50),
    @RequestorPhoneNumber NVARCHAR(50),
    @EmployeeStatus NVARCHAR(100),
    @EmployeeType NVARCHAR(100),
    @TransferFromProgram NVARCHAR(20),
    @LeaveOfAbsence NVARCHAR(20),
    @RegionName NVARCHAR(200),
    @Program1 NVARCHAR(200),
    @Program2 NVARCHAR(200),
    @Program3 NVARCHAR(200),
    @OtherPrograms NVARCHAR(MAX),
    @EmployeeDeviceTypes NVARCHAR(MAX),
    @OtherDeviceType NVARCHAR(200),
    @HrRepresentativeName NVARCHAR(200),
    @SubmitForApproval NVARCHAR(20),
    @Status NVARCHAR(100),
    @AuthorizedApprover NVARCHAR(200),
    @ProgramAdministrator NVARCHAR(200),
    @RdoApprover NVARCHAR(200),
    @RejectionReason NVARCHAR(MAX),
    @EmailAccount NVARCHAR(20),
    @AccountEnableDisable NVARCHAR(50),
    @ForwardingEmailAddress NVARCHAR(200),
    @AssignedAccountUsername NVARCHAR(200),
    @Office365License NVARCHAR(200),
    @AdditionalMicrosoftProducts NVARCHAR(MAX),
    @CreateEmailGroups NVARCHAR(20),
    @SharePointSiteUrl NVARCHAR(500),
    @SharePointSiteDescription NVARCHAR(200),
    @ExistingEmailGroups NVARCHAR(MAX),
    @AdditionalSharepointAccess NVARCHAR(MAX),
    @NetworkSharedDrives NVARCHAR(MAX),
    @TopAccess NVARCHAR(20),
    @KronosAccessTypes NVARCHAR(MAX),
    @KronosAccessDetails NVARCHAR(MAX),
    @LawsonAccess NVARCHAR(20),
    @AdditionalLawsonAccess NVARCHAR(MAX),
    @MileageAccess NVARCHAR(20),
    @ComericaBankingAccess NVARCHAR(20),
    @TrustRepPayeeAccess NVARCHAR(20),
    @QuickbooksAccess NVARCHAR(20),
    @CoWorkerAccess NVARCHAR(20),
    @AdditionalHrFinanceAccess NVARCHAR(MAX),
    @OrderConnectAccess NVARCHAR(20),
    @CaminarAccess NVARCHAR(20),
    @AvatarAccess NVARCHAR(20),
    @AdverseEventSupervisorAccess NVARCHAR(20),
    @AdverseEventAdditionalProgramAccess NVARCHAR(20),
    @BusinessIntelligenceRole NVARCHAR(200),
    @AdverseEventNonEmployeeAccess NVARCHAR(20),
    @TelehealthAccess NVARCHAR(20),
    @NextTelehealthSession NVARCHAR(100),
    @AdditionalComments NVARCHAR(MAX),
    @PharmericaAccess NVARCHAR(20),
    @PharmericaUserRole NVARCHAR(200),
    @PharmericaUsername NVARCHAR(200),
    @CallCenterAccess NVARCHAR(20),
    @AdobeSignAccount NVARCHAR(20),
    @CopilotLicense NVARCHAR(20),
    @SmartsheetLicense NVARCHAR(20),
    @EFax NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.UarRequests
    SET RequestNumber = @RequestNumber,
        SubmittedOn = @SubmittedOn,
        EmployeeNameChange = @EmployeeNameChange,
        Company = @Company,
        EmployeeFirstName = @EmployeeFirstName,
        EmployeeMiddleName = @EmployeeMiddleName,
        EmployeeLastName = @EmployeeLastName,
        JobTitle = @JobTitle,
        DesiredEffectiveDate = @DesiredEffectiveDate,
        RequestorPhoneNumber = @RequestorPhoneNumber,
        EmployeeStatus = @EmployeeStatus,
        EmployeeType = @EmployeeType,
        TransferFromProgram = @TransferFromProgram,
        LeaveOfAbsence = @LeaveOfAbsence,
        RegionName = @RegionName,
        Program1 = @Program1,
        Program2 = @Program2,
        Program3 = @Program3,
        OtherPrograms = @OtherPrograms,
        EmployeeDeviceTypes = @EmployeeDeviceTypes,
        OtherDeviceType = @OtherDeviceType,
        HrRepresentativeName = @HrRepresentativeName,
        SubmitForApproval = @SubmitForApproval,
        Status = @Status,
        AuthorizedApprover = @AuthorizedApprover,
        ProgramAdministrator = @ProgramAdministrator,
        RdoApprover = @RdoApprover,
        RejectionReason = @RejectionReason,
        EmailAccount = @EmailAccount,
        AccountEnableDisable = @AccountEnableDisable,
        ForwardingEmailAddress = @ForwardingEmailAddress,
        AssignedAccountUsername = @AssignedAccountUsername,
        Office365License = @Office365License,
        AdditionalMicrosoftProducts = @AdditionalMicrosoftProducts,
        CreateEmailGroups = @CreateEmailGroups,
        SharePointSiteUrl = @SharePointSiteUrl,
        SharePointSiteDescription = @SharePointSiteDescription,
        ExistingEmailGroups = @ExistingEmailGroups,
        AdditionalSharepointAccess = @AdditionalSharepointAccess,
        NetworkSharedDrives = @NetworkSharedDrives,
        TopAccess = @TopAccess,
        KronosAccessTypes = @KronosAccessTypes,
        KronosAccessDetails = @KronosAccessDetails,
        LawsonAccess = @LawsonAccess,
        AdditionalLawsonAccess = @AdditionalLawsonAccess,
        MileageAccess = @MileageAccess,
        ComericaBankingAccess = @ComericaBankingAccess,
        TrustRepPayeeAccess = @TrustRepPayeeAccess,
        QuickbooksAccess = @QuickbooksAccess,
        CoWorkerAccess = @CoWorkerAccess,
        AdditionalHrFinanceAccess = @AdditionalHrFinanceAccess,
        OrderConnectAccess = @OrderConnectAccess,
        CaminarAccess = @CaminarAccess,
        AvatarAccess = @AvatarAccess,
        AdverseEventSupervisorAccess = @AdverseEventSupervisorAccess,
        AdverseEventAdditionalProgramAccess = @AdverseEventAdditionalProgramAccess,
        BusinessIntelligenceRole = @BusinessIntelligenceRole,
        AdverseEventNonEmployeeAccess = @AdverseEventNonEmployeeAccess,
        TelehealthAccess = @TelehealthAccess,
        NextTelehealthSession = @NextTelehealthSession,
        AdditionalComments = @AdditionalComments,
        PharmericaAccess = @PharmericaAccess,
        PharmericaUserRole = @PharmericaUserRole,
        PharmericaUsername = @PharmericaUsername,
        CallCenterAccess = @CallCenterAccess,
        AdobeSignAccount = @AdobeSignAccount,
        CopilotLicense = @CopilotLicense,
        SmartsheetLicense = @SmartsheetLicense,
        EFax = @EFax
    WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE dbo.UarRequest_Delete
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.UarRequests WHERE Id = @Id;
END
GO
