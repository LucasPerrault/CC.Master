using Lucca.Emails.Client;
using System;

namespace Email.Infra.Configuration
{
    public class EmailConfiguration : LuccaEmailsConfiguration
    {
        public Uri ServerUri { get; set; }
        public string EmailAppEndpointPath { get; set; }
    }
}
