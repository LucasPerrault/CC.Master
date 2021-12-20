using Lucca.Emails.Client.Contracts;
using Lucca.Emails.Client.Contracts.Fragments;
using System.Collections.Generic;
using System.Linq;

namespace Email.Domain
{
    public class EmailContentHelper
    {
        public static string GetEmailSubject(string title) => $"CloudControl - {title}";
        public static MailHeader GetEmailHeader(string title) => new MailHeader { Title = GetEmailSubject(title) };
        public static EmailTemplate GetMailTemplate(string title, List<Fragment> fragments) => new EmailTemplate
            {
                Fragments = new List<Fragment>
                {
                    GetEmailHeader(title),
                    new CardWrapper
                    {
                        Fragments = new List<Fragment>(fragments) { new FooterImage() },
                    },
                },
            };
    }

    public class EmailContentBuilder
    {
        private readonly string _title;
        private readonly List<Fragment> _fragments = new List<Fragment>();

        public EmailContentBuilder(string title)
        {
            _title = title;
        }

        public EmailContentBuilder Add(Fragment f)
        {
            _fragments.Add(f);
            return this;
        }

        public EmailContentBuilder AddList(IEnumerable<string> elements)
        {
            var text = new List<Fragment>();
            text.Add(new Paragraph("<ul>"));
            text.AddRange(elements.Select(e => new Paragraph($"<li>{e}</li>")));
            text.Add(new Paragraph("</ul>"));
            Add(new Paragraph { Text = text });
            return this;
        }

        public EmailContent Build() => new EmailContent
        (
            EmailContentHelper.GetEmailSubject(_title),
            EmailContentHelper.GetMailTemplate(_title, _fragments)
        );
    }
}
