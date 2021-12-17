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
        private static readonly ContactedUserInMemoryCache _cache = new ContactedUserInMemoryCache();
        private const string LockName = "IpRejectionEmail";

        private readonly IEmailService _emailService;
        private readonly IIpFilterEmails _ipFilterEmails;
        private readonly ITimeProvider _time;
        private readonly IGuidGenerator _guid;
        private readonly ILockService _lockService;
        private readonly IIpFilterAuthorizationRequestStore _store;

        public IpFilterRequestCreationService
        (
            IEmailService emailService,
            IIpFilterEmails ipFilterEmails,
            ILockService lockService,
            IIpFilterAuthorizationRequestStore store,
            ITimeProvider time,
            IGuidGenerator guid
        )
        {
            _emailService = emailService;
            _ipFilterEmails = ipFilterEmails;
            _lockService = lockService;
            _store = store;
            _time = time;
            _guid = guid;
        }

        public async Task SendRequestIfNeededAsync(RejectedUser user, EmailHrefBuilder emailHrefBuilder)
        {
            var contactedUser = new ContactedUser { UserId = user.IpFilterUser.UserId, Address = user.IpFilterUser.IpAddress };
            if (_cache.Has(contactedUser))
            {
                return;
            }
            using (await _lockService.TakeLockAsync(LockName, TimeSpan.FromSeconds(10)))
            {
                if (_cache.Has(contactedUser))
                {
                    return;
                }

                var request = await CreateOrGetPendingAsync(user.IpFilterUser);
                await _emailService.SendAsync
                (
                    RecipientForm.FromUserId(user.IpFilterUser.UserId),
                    _ipFilterEmails.GetRejectionEmail(user, request, emailHrefBuilder)
                );
                _cache.Cache(contactedUser);
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


        private class ContactedUser : ValueObject
        {
            public int UserId { get; set; }
            public string Address { get; set; }

            protected override IEnumerable<object> EqualityComponents
            {
                get
                {
                    yield return UserId;
                    yield return Address;
                }
            }
        }

        private class ContactedUserInMemoryCache : InMemoryValueObjectCache<ContactedUser>
        {
            public ContactedUserInMemoryCache() : base(TimeSpan.FromSeconds(10))
            { }
        }
    }
}
