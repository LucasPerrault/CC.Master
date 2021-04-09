using System;

namespace Cache.Abstractions
{
    public abstract class CacheKey<T>
    {
        public abstract string Key { get; }
        public virtual TimeSpan? Invalidation => null;
    }
}
