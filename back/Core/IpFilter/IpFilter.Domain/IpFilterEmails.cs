using Email.Domain;
using Lucca.Emails.Client.Contracts;
using Lucca.Emails.Client.Contracts.Fragments;
using Resources.Translations;
using System;
using System.Globalization;

namespace IpFilter.Domain
{
    public class EmailHrefBuilder
    {
        public Func<Guid, string> Accept { get; set; }
        public Func<Guid, string> Reject { get; set; }
    }

    public interface IIpFilterEmails
    {
        EmailContent GetRejectionEmail(RejectedUser user, IpFilterAuthorizationRequest request, EmailHrefBuilder builder);
    }

    public class IpFilterEmails : IIpFilterEmails
    {
        private readonly IIpFilterTranslations _translations;

        public IpFilterEmails(IIpFilterTranslations translations)
        {
            _translations = translations;
        }

        public EmailContent GetRejectionEmail(RejectedUser user, IpFilterAuthorizationRequest request, EmailHrefBuilder emailHrefBuilder)
        {
            return new EmailContentBuilder(_translations.RejectionEmailTitle())
                .Add(RejectionEmailGreetingsAndContext(user, request))
                .Add(RejectionEmailActionDescriptionAndExpirationTime(request))
                .Add(RejectionEmailSameDeviceWarning())
                .Add(RejectionEmailActionButtons(user, request, emailHrefBuilder))
                .Build();
        }

        private Fragment RejectionEmailActionButtons(RejectedUser user, IpFilterAuthorizationRequest request, EmailHrefBuilder emailHrefBuilder)
        {
            return new TwoButtons
            {
                LeftButton = new Button {Text = _translations.RejectionEmailAuthorizeActionButton(), Href = emailHrefBuilder.Accept(request.Code)},
                RightButton = new Button {Text = _translations.RejectionEmailRejectActionButton(), Href = emailHrefBuilder.Reject(request.Code)},
            };
        }

        private Fragment RejectionEmailSameDeviceWarning()
        {
            return new Paragraph(_translations.RejectionEmailSameDeviceWarning().WithHtmlNewLines());
        }

        private Fragment RejectionEmailActionDescriptionAndExpirationTime(IpFilterAuthorizationRequest request)
        {
            return new Paragraph
            (
                _translations.RejectionEmailActionDescriptionAndExpiration
                (
                    $"{request.CreatedAt.ToString(CultureInfo.CurrentCulture.DateTimeFormat.LongDatePattern)}",
                    $"{request.CreatedAt.ToString(CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern)}"
                ).WithHtmlNewLines()
            );
        }

        private Fragment RejectionEmailGreetingsAndContext(RejectedUser user, IpFilterAuthorizationRequest request)
        {
            return new Paragraph(_translations.RejectionEmailGreetingsAndContext
                (
                    user.FirstName,
                    $"{request.ExpiresAt.ToString(CultureInfo.CurrentCulture.DateTimeFormat.LongDatePattern)}",
                    $"{request.ExpiresAt.ToString(CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern)}",
                    request.Address
                ).WithHtmlNewLines()
            );
        }
    }

    public static class StringExtensions
    {
        public static string WithHtmlNewLines(this string s)
        {
            return s.Replace("\n", "\n<br/>\n");
        }
    }
}
