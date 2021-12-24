using System;
using System.Collections.Generic;
using System.Linq;

namespace Rights.Domain
{
    public enum Operation : ushort
    {
        ReadInstances = 1,  // OBSOLETE
        ReadEnvironments = 1000,
        DisplayEnvTab = 1001,
        RestoreInstances = 1002,
        ImpersonationOnTrainingAndPreview = 1003,
        ImpersonationOnProd = 1004,
        EditEnvironmentGroup = 1005,
        RenameEnvironment = 1006,
        Restore = 3,  // OBSOLETE
        CreateInstance = 4,
        UserImpersonationPreviewAndTraining = 5,  // OBSOLETE
        UserImpersonationProd = 5001,  // OBSOLETE
        AccessNonAnonymizedInstances = 5002,
        UserProdImpersonationForRestrictedEnvironments = 5003,  // OBSOLETE
        Desactivate = 6,
        GeneralUpgrade = 7,
        Preview = 8,
        UserActifDeprecated = 9,
        Demo = 10,
        TemplateDemos = 10001,
        ReadContracts = 21,
        CreateContracts = 21001,
        CloseContracts = 21002,
        ReadContractEntries = 21003,
        EditContractsEntities = 21004,
        EditContracts = 21005,
        Counts = 22,
        Accounting = 23,
        Invoices = 24,
        ExportClients = 24001,
        CountsTrack = 26,
        SalesforceSync = 28,
        DeleteInstance = 29,


        // New operations
        ReadCommercialOffers = 25,
        CreateCommercialOffers = 35,
        ReadFtp = 27,
        ReadCounts = 30,
        CreateCounts = 31,
        ReadLuccaInstances = 32,  // OBSOLETE
        RestoreRestrictedEnvironments = 33,  // OBSOLETE
        ReadCMRR = 34,
        ReadCodeSources = 36000,
        EditCodeSources = 36001,
        EditGitHubBranchesAndPR = 37000,
        ReadDistributorsTab = 38000,
        EditDistributors = 38001,
        EnvironmentAccessesAndDistributorRelations = 38002,
        UpdatePreviewDeployStatus = 8001,
        DeployAllPreviews = 8002,
        DeletePreviews = 8003,
        EnvironmentLogTab = 39000,
        ContactRoles = 40000,
        EditClients = 41001,
        HangfireNetcoreRequest = 52000,

        ReadDistributorRelations = 38003,

        ReadTenantDataRequest = 50000,
        WriteTenantDataRequest = 50001,
        ReadTenantDataRequestStatus = 50002,
        CloudControlInternalRequest = 51000,
        AccessBetaFeatures = 51001,

        // CAFE
        SyncAllCafe = 65000,
        ReadAllCafe = 65001,
    }

    public static class OperationHelper
    {
        public static IEnumerable<Operation> GetAll() => Enum.GetValues(typeof(Operation)).Cast<Operation>();
    }
}
