﻿using Email.Domain;
using Instances.Domain.Demos.Cleanup;
using Lucca.Emails.Client.Contracts;
using Lucca.Emails.Client.Contracts.Fragments;
using Resources.Translations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Instances.Application.Demos.Emails
{
    public interface IDemoEmails
    {
        EmailContent GetIntentEmail(DateTime deletionDate, IEnumerable<DemoCleanupInfo> infos);
    }

    public class DemoEmails : IDemoEmails
    {
        private readonly Translations _translations;

        public DemoEmails(Translations translations)
        {
            _translations = translations;
        }

        public EmailContent GetIntentEmail(DateTime deletionDate, IEnumerable<DemoCleanupInfo> infos)
        {
            var infoPerState = infos.GroupBy(i => i.State).ToDictionary(i => i.Key, i => i.ToList());

            var aliveAndWellCount = infoPerState.ContainsKey(DemoState.AliveAndWell)
                ? infoPerState[DemoState.AliveAndWell].Count
                : 0;

            var builder = new EmailContentBuilder(_translations.EmailsDemoCleanupIntentTitle($"{deletionDate:yyyy-MM-dd}"))
                .Add(new Paragraph(_translations.EmailsDemoCleanupIntentContext()))
                .Add(new Paragraph(_translations.EmailsDemoCleanupIntentAliveAndWellCount(aliveAndWellCount)));

            if (infoPerState.ContainsKey(DemoState.ErrorAtStateEvaluation))
            {
                var unknownState = infoPerState[DemoState.ErrorAtStateEvaluation];
                builder.Add(new Paragraph(_translations.EmailsDemoCleanupIntentUndeterminedCount(unknownState.Count)));
                builder.AddList(unknownState.Select(d => $"<b>{ d.Demo.Subdomain }</b> : { d.Message }"));
            }

            if (infoPerState.ContainsKey(DemoState.DeletionScheduledToday))
            {
                var deletions = infoPerState[DemoState.DeletionScheduledToday];
                builder.Add(new Paragraph(_translations.EmailsDemoCleanupIntentTriggeredCount(deletions.Count)));
                builder.AddList(deletions.Select(d => $"<b>{ d.Demo.Subdomain }</b>"));
            }
            else
            {
                builder.Add(new Paragraph(_translations.EmailsDemoCleanupIntentNoneTriggered()));
            }

            if (infoPerState.ContainsKey(DemoState.DeletionScheduledSoon))
            {
                var soonDeletions = infoPerState[DemoState.DeletionScheduledSoon];
                builder.Add(new Paragraph(_translations.EmailsDemoCleanupIntentToBeScheduledCount(soonDeletions.Count)));
                builder.AddList
                (
                    soonDeletions
                        .OrderBy(d => d.DeletionScheduledDate)
                        .Select(d => $"<b>{ d.Demo.Subdomain }</b> {d.DeletionScheduledDate.ToLongDateString()}")
                );
            }
            else
            {
                builder.Add(new Paragraph(_translations.EmailsDemoCleanupIntentNoneToBeScheduled()));
            }

            return builder.Build();
        }
    }
}
