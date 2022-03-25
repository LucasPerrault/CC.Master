using Environments.Domain;
using Environments.Domain.Storage;
using Instances.Domain.Trainings;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace Instances.Application.Trainings
{
    public class TrainingRestorationLogsRepository
    {
        private readonly IEnvironmentLogsStore _store;

        public TrainingRestorationLogsRepository(IEnvironmentLogsStore store)
        {
            _store = store;
        }

        public async Task LogSuccessfulTrainingRestorationAsync(TrainingRestoration restoration)
        {
            var log = new EnvironmentLog
            {
                ActivityId = EnvironmentLogActivity.TrainingRestorationSucceeded,
                CreatedOn = restoration.InstanceDuplication.EndedAt.Value,
                EnvironmentId = restoration.EnvironmentId,
                IsAnonymizedData = restoration.Anonymize,
                UserId = restoration.AuthorId,
                Messages = new List<EnvironmentLogMessage>
                {
                    new EnvironmentLogMessage
                    {
                        CreatedOn = restoration.InstanceDuplication.EndedAt.Value,
                        UserId = restoration.AuthorId,
                        Type = EnvironmentLogMessageTypes.INTERNAL,
                        Message = JsonSerializer.Serialize(new
                        {
                            restoreFiles = restoration.RestoreFiles,
                            keepExistingPasswords = restoration.KeepExistingTrainingPasswords,
                            restorationId  = restoration.Id,
                            instanceDuplicationId  = restoration.InstanceDuplicationId,
                            apiKey = restoration.ApiKeyStorableId,
                        }),
                    }
                }
            };
            if (!string.IsNullOrWhiteSpace(restoration.Comment))
            {
                log.Messages.Add(new EnvironmentLogMessage
                {
                    CreatedOn = DateTime.Now,
                    ExpiredOn = restoration.CommentExpiryDate ?? DateTime.Today.AddDays(7),
                    Message = restoration.Comment,
                    UserId = restoration.AuthorId,
                    Type = EnvironmentLogMessageTypes.EXPLANATION
                });
            }
            await _store.CreateAsync(log);
        }

        public async Task LogFailedTrainingRestorationAsync(TrainingRestoration restoration)
        {
            var now = DateTime.Now;
            var log = new EnvironmentLog
            {
                ActivityId = EnvironmentLogActivity.TrainingRestorationFailed,
                CreatedOn = now,
                EnvironmentId = restoration.EnvironmentId,
                IsAnonymizedData = restoration.Anonymize,
                UserId = restoration.AuthorId,
                Messages = new List<EnvironmentLogMessage>
                {
                    new EnvironmentLogMessage
                    {
                        CreatedOn = now,
                        UserId = restoration.AuthorId,
                        Type = EnvironmentLogMessageTypes.INTERNAL,
                        Message = JsonSerializer.Serialize(new
                        {
                            restoreFiles = restoration.RestoreFiles,
                            keepExistingPasswords = restoration.KeepExistingTrainingPasswords,
                            restorationId  = restoration.Id,
                            instanceDuplicationId  = restoration.InstanceDuplicationId,
                            apiKey = restoration.ApiKeyStorableId,
                        }),
                    }
                }
            };

            await _store.CreateAsync(log);
        }
    }
}
