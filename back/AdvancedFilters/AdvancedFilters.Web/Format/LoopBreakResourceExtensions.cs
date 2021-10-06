using AdvancedFilters.Domain.Billing.Models;
using AdvancedFilters.Domain.Contacts.Models;
using AdvancedFilters.Domain.Core.Models;
using AdvancedFilters.Domain.Instance.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Environment = AdvancedFilters.Domain.Instance.Models.Environment;

namespace AdvancedFilters.Web.Format
{
    public static class LoopBreakResourceExtensions
    {
        private enum Resources
        {
            Environment,
            LegalUnit,
            Establishment,
            AppInstance,
            AppContact,
            ClientContact,
            SpecializedContact,
            Client,
            Contract,
            EstablishmentContract,
        }

        public static IEnumerable<Environment> WithoutLoop(this IEnumerable<Environment> list)
            => list.WithoutLoop(new RootLoopBreaker());
        private static Environment WithoutLoop(this Environment item, LoopBreaker breaker)
        {
            return breaker.GetWithoutLoop(item, Resources.Environment, (e, b) => e.BreakLoop(b));
        }
        private static IEnumerable<Environment> WithoutLoop(this IEnumerable<Environment> list, LoopBreaker breaker)
        {
            return breaker.GetWithoutLoop(list, Resources.Environment, (e, b) => e.BreakLoop(b));
        }
        private static void BreakLoop(this Environment src, Func<LoopBreaker> getBreaker)
        {
            src.LegalUnits = src.LegalUnits.WithoutLoop(getBreaker());
            src.AppInstances = src.AppInstances.WithoutLoop(getBreaker());
        }

        public static IEnumerable<LegalUnit> WithoutLoop(this IEnumerable<LegalUnit> list)
            => list.WithoutLoop(new RootLoopBreaker());
        private static LegalUnit WithoutLoop(this LegalUnit item, LoopBreaker breaker)
        {
            return breaker.GetWithoutLoop(item, Resources.LegalUnit, (e, b) => e.BreakLoop(b));
        }
        private static IEnumerable<LegalUnit> WithoutLoop(this IEnumerable<LegalUnit> list, LoopBreaker breaker)
        {
            return breaker.GetWithoutLoop(list, Resources.LegalUnit, (e, b) => e.BreakLoop(b));
        }
        private static void BreakLoop(this LegalUnit src, Func<LoopBreaker> getBreaker)
        {
            src.Environment = src.Environment.WithoutLoop(getBreaker());
            src.Establishments = src.Establishments.WithoutLoop(getBreaker());
        }

        private static Establishment WithoutLoop(this Establishment item, LoopBreaker breaker)
        {
            return breaker.GetWithoutLoop(item, Resources.Establishment, (e, b) => e.BreakLoop(b));
        }
        private static IEnumerable<Establishment> WithoutLoop(this IEnumerable<Establishment> list, LoopBreaker breaker)
        {
            return breaker.GetWithoutLoop(list, Resources.Establishment, (e, b) => e.BreakLoop(b));
        }
        private static void BreakLoop(this Establishment src, Func<LoopBreaker> getBreaker)
        {
            src.Environment = src.Environment.WithoutLoop(getBreaker());
            src.LegalUnit = src.LegalUnit.WithoutLoop(getBreaker());
        }

        public static IEnumerable<AppInstance> WithoutLoop(this IEnumerable<AppInstance> list)
            => list.WithoutLoop(new RootLoopBreaker());
        private static AppInstance WithoutLoop(this AppInstance item, LoopBreaker breaker)
        {
            return breaker.GetWithoutLoop(item, Resources.AppInstance, (e, b) => e.BreakLoop(b));
        }
        private static IEnumerable<AppInstance> WithoutLoop(this IEnumerable<AppInstance> list, LoopBreaker breaker)
        {
            return breaker.GetWithoutLoop(list, Resources.AppInstance, (e, b) => e.BreakLoop(b));
        }
        private static void BreakLoop(this AppInstance src, Func<LoopBreaker> getBreaker)
        {
            src.Environment = src.Environment.WithoutLoop(getBreaker());
        }

        public static IEnumerable<AppContact> WithoutLoop(this IEnumerable<AppContact> list)
            => list.WithoutLoop(new RootLoopBreaker());
        private static AppContact WithoutLoop(this AppContact item, LoopBreaker breaker)
        {
            return breaker.GetWithoutLoop(item, Resources.AppContact, (e, b) => e.BreakLoop(b));
        }
        private static IEnumerable<AppContact> WithoutLoop(this IEnumerable<AppContact> list, LoopBreaker breaker)
        {
            return breaker.GetWithoutLoop(list, Resources.AppContact, (e, b) => e.BreakLoop(b));
        }
        private static void BreakLoop(this AppContact src, Func<LoopBreaker> getBreaker)
        {
            src.Environment = src.Environment.WithoutLoop(getBreaker());
            src.Establishment = src.Establishment.WithoutLoop(getBreaker());
            src.AppInstance = src.AppInstance.WithoutLoop(getBreaker());
        }

        public static IEnumerable<ClientContact> WithoutLoop(this IEnumerable<ClientContact> list)
            => list.WithoutLoop(new RootLoopBreaker());
        private static ClientContact WithoutLoop(this ClientContact item, LoopBreaker breaker)
        {
            return breaker.GetWithoutLoop(item, Resources.ClientContact, (e, b) => e.BreakLoop(b));
        }
        private static IEnumerable<ClientContact> WithoutLoop(this IEnumerable<ClientContact> list, LoopBreaker breaker)
        {
            return breaker.GetWithoutLoop(list, Resources.ClientContact, (e, b) => e.BreakLoop(b));
        }
        private static void BreakLoop(this ClientContact src, Func<LoopBreaker> getBreaker)
        {
            src.Environment = src.Environment.WithoutLoop(getBreaker());
            src.Establishment = src.Establishment.WithoutLoop(getBreaker());
            src.Client = src.Client.WithoutLoop(getBreaker());
        }

        public static IEnumerable<SpecializedContact> WithoutLoop(this IEnumerable<SpecializedContact> list)
            => list.WithoutLoop(new RootLoopBreaker());
        private static SpecializedContact WithoutLoop(this SpecializedContact item, LoopBreaker breaker)
        {
            return breaker.GetWithoutLoop(item, Resources.SpecializedContact, (e, b) => e.BreakLoop(b));
        }
        private static IEnumerable<SpecializedContact> WithoutLoop(this IEnumerable<SpecializedContact> list, LoopBreaker breaker)
        {
            return breaker.GetWithoutLoop(list, Resources.SpecializedContact, (e, b) => e.BreakLoop(b));
        }
        private static void BreakLoop(this SpecializedContact src, Func<LoopBreaker> getBreaker)
        {
            src.Environment = src.Environment.WithoutLoop(getBreaker());
            src.Establishment = src.Establishment.WithoutLoop(getBreaker());
        }

        private static Client WithoutLoop(this Client item, LoopBreaker breaker)
        {
            return breaker.GetWithoutLoop(item, Resources.Client, (e, b) => e.BreakLoop(b));
        }
        private static IEnumerable<Client> WithoutLoop(this IEnumerable<Client> list, LoopBreaker breaker)
        {
            return breaker.GetWithoutLoop(list, Resources.Client, (e, b) => e.BreakLoop(b));
        }
        private static void BreakLoop(this Client src, Func<LoopBreaker> getBreaker)
        {
            src.Contracts = src.Contracts.WithoutLoop(getBreaker());
        }

        private static Contract WithoutLoop(this Contract item, LoopBreaker breaker)
        {
            return breaker.GetWithoutLoop(item, Resources.Contract, (e, b) => e.BreakLoop(b));
        }
        private static IEnumerable<Contract> WithoutLoop(this IEnumerable<Contract> list, LoopBreaker breaker)
        {
            return breaker.GetWithoutLoop(list, Resources.Contract, (e, b) => e.BreakLoop(b));
        }
        private static void BreakLoop(this Contract src, Func<LoopBreaker> getBreaker)
        {
            src.Client = src.Client.WithoutLoop(getBreaker());
            src.EstablishmentAttachments = src.EstablishmentAttachments.WithoutLoop(getBreaker());
        }

        private static EstablishmentContract WithoutLoop(this EstablishmentContract item, LoopBreaker breaker)
        {
            return breaker.GetWithoutLoop(item, Resources.EstablishmentContract, (e, b) => e.BreakLoop(b));
        }
        private static IEnumerable<EstablishmentContract> WithoutLoop(this IEnumerable<EstablishmentContract> list, LoopBreaker breaker)
        {
            return breaker.GetWithoutLoop(list, Resources.EstablishmentContract, (e, b) => e.BreakLoop(b));
        }
        private static void BreakLoop(this EstablishmentContract src, Func<LoopBreaker> getBreaker)
        {
            src.Contract = src.Contract.WithoutLoop(getBreaker());
            src.Establishment = src.Establishment.WithoutLoop(getBreaker());
        }

        private class LoopBreaker
        {
            private HashSet<Resources> _visitsCache = new HashSet<Resources>();

            public LoopBreaker()
            { }

            public LoopBreaker(Resources root)
            {
                _visitsCache.Add(root);
            }

            private LoopBreaker(HashSet<Resources> resources)
            {
                _visitsCache = new HashSet<Resources>(resources);
            }

            public virtual T GetWithoutLoop<T>(T item, Resources resource, Action<T, Func<LoopBreaker>> breakLoopFunc)
                where T : class
            {
                if (item == null || CheckAndMarkVisited(resource, _visitsCache))
                {
                    return null;
                }

                var copy = item is IDeepCopyable<T> copyable
                    ? copyable.DeepCopy()
                    : item;

                breakLoopFunc(copy, () => GetNextLoopBreaker(resource));
                return copy;
            }

            public virtual IEnumerable<T> GetWithoutLoop<T>(IEnumerable<T> list, Resources resource, Action<T, Func<LoopBreaker>> breakLoopFunc)
                where T : class
            {
                if (list == null || CheckAndMarkVisited(resource, _visitsCache))
                {
                    return null;
                }

                foreach (var item in list)
                {
                    breakLoopFunc(item, () => GetNextLoopBreaker(resource));
                }
                return list.Where(item => item != null).ToList();
            }

            protected virtual LoopBreaker GetNextLoopBreaker(Resources current)
            {
                return new LoopBreaker(_visitsCache);
            }

            private bool CheckAndMarkVisited(Resources newVisit, HashSet<Resources> cache)
            {
                var hasVisited = cache.Contains(newVisit);

                cache.Add(newVisit);
                return hasVisited;
            }
        }

        private class RootLoopBreaker : LoopBreaker
        {
            protected override LoopBreaker GetNextLoopBreaker(Resources current)
            {
                return new LoopBreaker(current);
            }
        }
    }
}
