using Lucca.Emails.Client.Contracts;
using Lucca.Emails.Client.Contracts.Fragments;
using System.Collections.Generic;

namespace Email.Domain
{
    public class EmailContentHelper
    {
        public static string GetEmailSubject(string title) => $"CloudControl - {title}";
        public static MailHeader GetEmailHeader(string title) => new MailHeader { Title = GetEmailSubject(title) };

        public static EmailTemplate GetMailTemplate(string title) => new EmailTemplate
        {
            Fragments = new List<Fragment>
            {
                GetEmailHeader(title)
            }
        };
    }

    public class EmailContentBuilder
    {
        public EmailContent Content { get; }

        public EmailContentBuilder(string title)
        {
            Content = new EmailContent
            (
                EmailContentHelper.GetEmailSubject(title),
                EmailContentHelper.GetMailTemplate(title)
            );
        }

        public EmailContentBuilder Add(Fragment f)
        {
            Content.Template.Fragments.Add(f);
            return this;
        }

        public EmailContentBuilder AddHtmlList(IEnumerable<string> elements)
        {
            Add(new Paragraph("<ul>"));
            foreach (var element in elements)
            {
                Add(new Paragraph($"<li>{element}</li>"));
            }
            Add(new Paragraph("</ul>"));

            return this;
        }
    }
}
