using Email.Domain;
using Lucca.Emails.Client.Contracts;
using Lucca.Emails.Client.Contracts.Fragments;
using Resources.Translations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvancedFilters.Infra.Services.Sync
{
    public interface ISyncEmails
    {
        EmailContent GetSyncReportEmail(List<Exception> exceptions);
    }

    public class SyncEmails : ISyncEmails
    {
        private readonly Translations _translations;

        public SyncEmails(Translations translations)
        {
            _translations = translations;
        }
        public EmailContent GetSyncReportEmail(List<Exception> exceptions)
        {
            var builder = new EmailContentBuilder(_translations.EmailSyncReportTitle())
                .Add(new Paragraph(_translations.EmailSyncReportErrorCount(exceptions.Count)));

            builder.AddList(exceptions.Select(d => d.Message));

            return builder.Build();
        }
    }
}
