using System;

namespace Users.Web
{
    public class UsersConfiguration
    {
        public Uri ServerUri { get; set; }
        public string UsersEndpointPath { get; set; }
        public string AllUsersEndpointPath { get; set; }
        public Guid UserFetchToken { get; set; }
    }
}
