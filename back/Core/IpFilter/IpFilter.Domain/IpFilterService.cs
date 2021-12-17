using IpFilter.Domain.Accessors;
using Lucca.Core.Shared.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using TeamNotification.Abstractions;
using Tools;

namespace IpFilter.Domain
{

    public class IpFilterService
    {
        private static readonly TimeSpan AuthorizationRequestValidity = TimeSpan.FromMinutes(10);
        private readonly IIpFilterAuthorizationStore _authorizationStore;
        private readonly IIpFilterAuthorizationRequestStore _requestStore;
        private readonly IUserAgentAccessor _accessor;
        private readonly ITimeProvider _time;
        private readonly ITeamNotifier _teamNotifier;

        public IpFilterService
        (
            IIpFilterAuthorizationStore authorizationStore,
            IIpFilterAuthorizationRequestStore requestStore,
            IUserAgentAccessor accessor,
            ITimeProvider time,
            ITeamNotifier teamNotifier
        )
        {
            _authorizationStore = authorizationStore;
            _requestStore = requestStore;
            _accessor = accessor;
            _time = time;
            _teamNotifier = teamNotifier;
        }

        public async Task<IEnumerable<IpFilterAuthorization>> GetValidAsync(IpFilterUser user)
        {
            var now = _time.Now();
            var filter = new IpFilterAuthorizationFilter
            {
                UserId = user.UserId,
                IpAddress = user.IpAddress,
                CreatedAt = CompareDateTime.IsStrictlyBefore(now),
                ExpiresAt = CompareDateTime.IsStrictlyAfter(now),
            };
            return await _authorizationStore.GetAsync(filter);
        }

        public async Task RejectAsync(IpFilterUser user, Guid requestCode)
        {
            var now = _time.Now();
            var validRequest = await GetValidRequestAsync(now, user, requestCode);
            if (validRequest is null)
            {
                throw new BadRequestException("Could not reject. Code is unknown or has expired.");
            }

            await NotifyRejectedRequest(user, validRequest, now);
            await _requestStore.RevokeAsync(validRequest);
        }

        public async Task ConfirmAsync(IpFilterUser user, Guid requestCode, AuthorizationDuration duration)
        {
            var now = _time.Now();
            var validRequest = await GetValidRequestAsync(now, user, requestCode);
            if (validRequest is null || await _authorizationStore.ExistsAsync(validRequest.Id))
            {
                throw new BadRequestException("Could not authorize. Code is unknown or has expired.");
            }

            var authorization = new IpFilterAuthorization
            {
                RequestId = validRequest.Id,
                CreatedAt = now,
                ExpiresAt = GetRequestExpiration(now, duration),
                IpAddress = user.IpAddress,
                UserId = user.UserId,
                Device = _accessor.UserAgent,
            };

            await _authorizationStore.CreateAsync(authorization);
        }

        public async Task<DateTime> GetRequestExpirationAsync(IpFilterUser user, Guid code)
        {
            var validRequest = await GetValidRequestAsync(_time.Now(), user, code);
            if (validRequest is null)
            {
                throw new NotFoundException("Code is unknown or has expired.");
            }

            return validRequest.ExpiresAt;
        }

        private Task<IpFilterAuthorizationRequest> GetValidRequestAsync(DateTime now, IpFilterUser user, Guid code)
        {
            return _requestStore.FirstOrDefaultAsync(new IpFilterAuthorizationRequestFilter
            {
                UserId = user.UserId,
                Addresses = new HashSet<string> { user.IpAddress },
                Code = code,
                CreatedAt = CompareDateTime.IsStrictlyBefore(now),
                ExpiresAt = CompareDateTime.IsStrictlyAfter(now),
                RevokedAt = CompareNullableDateTime.IsNull(),
            });
        }

        private DateTime GetRequestExpiration(DateTime createdAt, AuthorizationDuration duration) => duration switch
        {
            AuthorizationDuration.OneDay => createdAt.AddDays(1),
            AuthorizationDuration.SixMonth => createdAt.AddMonths(6),
            _ => throw new InvalidEnumArgumentException(nameof(duration), (int) duration, typeof(AuthorizationDuration))
        };

        private async Task NotifyRejectedRequest(IpFilterUser user, IpFilterAuthorizationRequest validRequest, DateTime now)
        {
            var message = new string[]
            {
                ":warning: Un utilisateur remonte une tentative de connexion frauduleuse !",
                $"Ip malveillante: {validRequest.Address}",
                $"Utilisateur cible : {validRequest.UserId}",
                $"Heure de la tentative : {validRequest.CreatedAt:hh:mm:ss}",
            };
            await _teamNotifier.NotifyAsync(Team.IpFilterGuardians, string.Join("\r\n", message));
        }
    }
}
