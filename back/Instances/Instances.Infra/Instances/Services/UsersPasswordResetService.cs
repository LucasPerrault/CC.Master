using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Instances.Infra.Instances.Services
{
    public interface IUsersPasswordResetService
    {
        Task ResetPasswordAsync(Uri uri, string password);
    }

    public class UsersPasswordResetService : IUsersPasswordResetService
    {
        private const string _mediaType = "application/json";
        private const string _identityPasswordResetRoute = "/identity/api/users/resetallpasswords";
        private readonly HttpClient _client;

        public UsersPasswordResetService(HttpClient client)
        {
            _client = client;
        }

        public async Task ResetPasswordAsync(Uri instanceHref, string password)
        {
            var uri = new Uri(instanceHref, _identityPasswordResetRoute);
            var payload = GetPayload(password);
            await _client.PostAsync(uri, payload);
        }

        private StringContent GetPayload(string password)
        {
            var payload = new PasswordResetPayload(password);
            var payloadToString = JsonConvert.SerializeObject(payload);
            return new StringContent(payloadToString, Encoding.UTF8, _mediaType);
        }

        private class PasswordResetPayload
        {

            public string Password { get; }
            public PasswordResetPayload(string password)
            {
                Password = password;
            }
        }
    }
}
