namespace Cache.Abstractions
{
    public abstract class CacheKey<T>
    {
        public abstract string Key { get; }
    }
}
