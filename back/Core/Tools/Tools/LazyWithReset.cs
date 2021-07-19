using System;
using System.ComponentModel;

namespace Tools
{
    public enum LazyWithResetRetry
    {
        None,
        Once
    }

    public class LazyWithReset<T> where T : class
    {
        private readonly Func<T> _init;
        private T _instance;
        private readonly object locker = new object();

        public LazyWithReset(Func<T> init)
        {
            this._init = init;
            this._instance = null;
        }

        private T Value
        {
            get
            {
                lock (locker)
                {
                    return _instance ??= _init();
                }
            }
        }

        public void SafeRun(LazyWithResetRetry retry, Action<T> action)
        {
            try
            {
                action(Value);
            }
            catch (Exception)
            {
                _instance = null;
                switch (retry)
                {
                    case LazyWithResetRetry.None:
                        throw;
                    case LazyWithResetRetry.Once:
                        SafeRun(LazyWithResetRetry.None, action);
                        break;
                    default:
                        throw new InvalidEnumArgumentException(nameof(retry), (int)retry, typeof(LazyWithResetRetry));
                }
            }
        }
    }
}
