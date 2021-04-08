using Email.Domain;
using Instances.Domain.Demos;
using Lucca.Emails.Client.Contracts.Fragments;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Instances.Application.Demos.Emails
{
    public static class DemoEmails
    {
        public static EmailContentBuilder IntentEmail(DateTime deletionDate, IEnumerable<DemoCleanupInfo> infos)
        {
            var infoPerState = infos.GroupBy(i => i.State).ToDictionary(i => i.Key, i => i.ToList());

            var aliveAndWellCount = infoPerState.ContainsKey(DemoState.AliveAndWell)
                ? infoPerState[DemoState.AliveAndWell].Count
                : 0;

            var builder = new EmailContentBuilder($"Nettoyage des démos - {deletionDate:yyyy-MM-dd}")
                .Add(new Paragraph("Un nettoyage automatique des démos vient d'être lancé."))
                .Add(new Paragraph($"{aliveAndWellCount} démos sont actives en ce moment."));

            if (infoPerState.ContainsKey(DemoState.ErrorAtStateEvaluation))
            {
                var unknownState = infoPerState[DemoState.ErrorAtStateEvaluation];
                builder.Add(new Paragraph($"{unknownState.Count} démos dont l'égibilité à la suppression n'a pu être déterminée"));
                builder.AddHtmlList(unknownState.Select(d => d.Demo.Subdomain));
            }

            if (infoPerState.ContainsKey(DemoState.DeletionScheduledToday))
            {
                var deletions = infoPerState[DemoState.DeletionScheduledToday];
                builder.Add(new Paragraph($"{deletions.Count} suppressions programmées"));
                builder.AddHtmlList(deletions.Select(d => d.Demo.Subdomain));
            }
            else
            {
                builder.Add(new Paragraph("Aucune suppression programmée."));
            }

            if (infoPerState.ContainsKey(DemoState.DeletionScheduledSoon))
            {
                var soonDeletions = infoPerState[DemoState.DeletionScheduledSoon];
                builder.Add(new Paragraph($"{soonDeletions.Count} suppressions à venir"));
                builder.AddHtmlList(soonDeletions.Select(d => $"{d.Demo.Subdomain} {d.DeletionScheduledDate.ToLongDateString()}"));
            }
            else
            {
                builder.Add(new Paragraph("Aucune suppression à programmer dans les jours à venir."));
            }

            return builder;
        }
    }
}
