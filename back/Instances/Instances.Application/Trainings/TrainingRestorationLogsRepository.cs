using Authentication.Domain.Helpers;
using Environments.Domain;
using Environments.Domain.Storage;
using Instances.Domain.Trainings;
using Lucca.Core.Api.Abstractions.Paging;
using Rights.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace Instances.Application.Trainings
{
    public class TrainingRestorationLogsRepository
    {
        private readonly IEnvironmentLogsStore _store;
        private readonly ClaimsPrincipal _claimsPrincipal;

        public TrainingRestorationLogsRepository(IEnvironmentLogsStore store, ClaimsPrincipal claimsPrincipal)
        {
            _store = store;
            _claimsPrincipal = claimsPrincipal;
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
                            resetPassword = !restoration.KeepExistingTrainingPasswords,
                            anonymize = restoration.Anonymize,
                            imposedPassword = (string)null,
                        }),
                    }
                }
            };
            if (!string.IsNullOrWhiteSpace(restoration.Comment))
            {
                log.Messages.Add(new EnvironmentLogMessage()
                {
                    CreatedOn = DateTime.Now,
                    ExpiredOn =restoration.CommentExpiryDate ?? DateTime.Today.AddDays(7),
                    Message = restoration.Comment,
                    UserId = restoration.AuthorId,
                    Type = EnvironmentLogMessageTypes.EXPLANATION
                });
            }
            await _store.CreateAsync(log);
        }

        public async Task LogFailedTrainingRestorationAsync(TrainingRestoration restoration)
        {
            var log = new EnvironmentLog
            {
                ActivityId = EnvironmentLogActivity.TrainingRestorationFailed,
                CreatedOn = DateTime.Now,
                EnvironmentId = restoration.EnvironmentId,
                IsAnonymizedData = restoration.Anonymize,
                UserId = restoration.AuthorId,
            };

            await _store.CreateAsync(log);
        }
    }
}
