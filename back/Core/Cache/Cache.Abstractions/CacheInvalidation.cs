using System;

namespace Cache.Abstractions
{
    public abstract class CacheInvalidation
    {
        internal CacheInvalidation()
        {

        }

        public static CacheInvalidation Never => new NeverCacheInvalidation();

        public static CacheInvalidation Default => new DefaultCacheInvalidation();

        public static CacheInvalidation After(TimeSpan duration) => new DurationCacheInvalidation(duration);
    }

    public  class NeverCacheInvalidation : CacheInvalidation
    {
        internal NeverCacheInvalidation()
        { }
    }

    public  class DefaultCacheInvalidation : CacheInvalidation
    {
        internal DefaultCacheInvalidation()
        { }
    }

    public class DurationCacheInvalidation : CacheInvalidation
    {
        internal DurationCacheInvalidation(TimeSpan duration)
        { }

        public TimeSpan Duration { get; }
    }
}
