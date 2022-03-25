using Distributors.Domain.Models;
using Instances.Domain.Instances.Models;
using Instances.Domain.Shared;
using System;

namespace Instances.Domain.Instances
{
    public enum InstanceDuplicationProgress
    {
        Pending = 1,
        Running = 2,
        FinishedWithSuccess = 3,
        FinishedWithFailure = 4,
        Canceled = 5
    }

    public class InstanceDuplication
    {
        public Guid Id { get; set; }

        public string SourceSubdomain { get; set; }
        public string SourceCluster { get; set; }
        public InstanceType SourceType { get; set; }

        public string TargetSubdomain { get; set; }
        public string TargetCluster { get; set; }
        public InstanceType TargetType { get; set; }

        public DateTime StartedAt { get; internal set; }
        public DateTime? EndedAt { get; set; }

        public InstanceDuplicationProgress Progress { get; set; }
        public int DistributorId { get; set; }

        public Distributor Distributor { get; set; }

        public InstanceDuplication()
        {
            StartedAt = DateTime.Now;
        }
    }

    public class InstanceDuplicationOptions
    {
        private InstanceDuplicationOptions() { }

        private const string KeepExistingPasswordsScriptKeyword = "KeepExistingPasswords";

        public bool WithAnonymization { get; init; }
        public bool SkipBufferServer { get; init; }
        public string[] SpecificPreRestoreScriptKeywordSelector { get; init; }
        public string[] SpecificPostRestoreScriptKeywordSelector { get; init; }
        public string CallbackPath { get; init; }
        public DuplicateInstanceScope Scope { get; init; }
        public DuplicateInstanceFileOptions FilesOptions { get; init; }


        public static InstanceDuplicationOptions ForDemo(string callBackPath)
        {
            return new InstanceDuplicationOptions
            {
                WithAnonymization = false,
                SkipBufferServer = true,
                SpecificPreRestoreScriptKeywordSelector = Array.Empty<string>(),
                SpecificPostRestoreScriptKeywordSelector = Array.Empty<string>(),
                CallbackPath = callBackPath,
                Scope = ToDuplicateInstanceScope(duplicateDatabase: true, duplicateFiles: true),
                FilesOptions = ToDuplicateInstanceFileOptions(diff: false, anonymize: false),
            };
        }

        public static InstanceDuplicationOptions ForTraining(bool withAnonymization, bool keepExistingPasswords, bool withFiles, string callBackPath)
        {
            return new InstanceDuplicationOptions
            {
                WithAnonymization = withAnonymization,
                SkipBufferServer = false,
                SpecificPreRestoreScriptKeywordSelector = keepExistingPasswords ? new string[] { KeepExistingPasswordsScriptKeyword }  : Array.Empty<string>(),
                SpecificPostRestoreScriptKeywordSelector = keepExistingPasswords ? new string[] { KeepExistingPasswordsScriptKeyword } : Array.Empty<string>(),
                CallbackPath = callBackPath,
                Scope = ToDuplicateInstanceScope(duplicateDatabase: true, duplicateFiles: withFiles),
                FilesOptions = ToDuplicateInstanceFileOptions(diff: true, anonymize: withAnonymization),
            };
        }

        private static DuplicateInstanceScope ToDuplicateInstanceScope(bool duplicateDatabase, bool duplicateFiles)
        {
            var scope = DuplicateInstanceScope.NONE;
            if (duplicateDatabase)
            {
                scope |= DuplicateInstanceScope.DATABASE;
            }
            if (duplicateFiles)
            {
                scope |= DuplicateInstanceScope.FILES;
            }
            return scope;
        }

        private static DuplicateInstanceFileOptions ToDuplicateInstanceFileOptions(bool diff, bool anonymize)
        {
            var fileOptions = DuplicateInstanceFileOptions.NONE;
            if (diff)
            {
                fileOptions |= DuplicateInstanceFileOptions.DIFF;
            }
            if (anonymize)
            {
                fileOptions |= DuplicateInstanceFileOptions.CLEAN;
            }
            return fileOptions;
        }

    }
}
