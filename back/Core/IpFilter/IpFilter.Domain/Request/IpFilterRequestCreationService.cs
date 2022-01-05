using Cache.Abstractions;
using Email.Domain;
using Lock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tools;

namespace IpFilter.Domain
{
    public class RejectedUser
    {
        public IpFilterUser IpFilterUser { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class IpFilterRequestCreationService
    {
        private static readonly TimeSpan AuthorizationRequestValidity = TimeSpan.FromMinutes(10);
        private static readonly TimeSpan MinimumDurationBetweenEmails = TimeSpan.FromSeconds(10);
        private const string LockName = "IpRejectionEmail";

        private readonly IEmailService _emailService;
        private readonly IIpFilterEmails _ipFilterEmails;
        private readonly ITimeProvider _time;
        private readonly IGuidGenerator _guid;
        private readonly ICacheService _cacheService;
        private readonly ILockService _lockService;
        private readonly IIpFilterAuthorizationRequestStore _store;

        public IpFilterRequestCreationService
        (
            IEmailService emailService,
            IIpFilterEmails ipFilterEmails,
            ILockService lockService,
            IIpFilterAuthorizationRequestStore store,
            ITimeProvider time,
            IGuidGenerator guid,
            ICacheService cacheService
        )
        {
            _emailService = emailService;
            _ipFilterEmails = ipFilterEmails;
            _lockService = lockService;
            _store = store;
            _time = time;
            _guid = guid;
            _cacheService = cacheService;
        }

        public async Task SendRequestIfNeededAsync(RejectedUser user, EmailHrefBuilder emailHrefBuilder)
        {
            var key = new ContactedUserCacheKey(user.IpFilterUser.UserId, user.IpFilterUser.IpAddress);
            if (await _cacheService.GetAsync(key))
            {
                return;
            }
            using (await _lockService.TakeLockAsync(LockName, TimeSpan.FromSeconds(10)))
            {
                if (await _cacheService.GetAsync(key))
                {
                    return;
                }

                var request = await CreateOrGetPendingAsync(user.IpFilterUser);
                await _emailService.SendAsync
                (
                    RecipientForm.FromUserId(user.IpFilterUser.UserId),
                    _ipFilterEmails.GetRejectionEmail(user, request, emailHrefBuilder)
                );
                await _cacheService.SetAsync(key, true, CacheInvalidation.After(MinimumDurationBetweenEmails));
            }
        }

        public async Task<IpFilterAuthorizationRequest> CreateOrGetPendingAsync(IpFilterUser user)
        {
            var now = _time.Now();
            var filter = new IpFilterAuthorizationRequestFilter
            {
                UserId = user.UserId,
                Addresses = new HashSet<string> { user.IpAddress },
                ExpiresAt = CompareDateTime.IsStrictlyAfter(now.Add(AuthorizationRequestValidity)),
                RevokedAt = CompareNullableDateTime.IsNull(),
                Code = _guid.New(),
            };

            var requests = await _store.GetAsync(filter);
            if (requests.Any())
            {
                return requests.First();
            }

            return await _store.CreateAsync
            (
                new IpFilterAuthorizationRequest
                {
                    Address = user.IpAddress,
                    UserId = user.UserId,
                    CreatedAt = now,
                    ExpiresAt = now.Add(AuthorizationRequestValidity),
                    Code = _guid.New(),
                }
            );
        }

        private class ContactedUserCacheKey : CacheKey<bool>
        {
            private readonly int _userId;
            private readonly string _ipAddress;

            public ContactedUserCacheKey(int userId, string ipAddress)
            {
                _userId = userId;
                _ipAddress = ipAddress;
            }

            public override string Key => $"ip-filter:contact:{_userId}:{_ipAddress}";
        }
    }
}
