using System;

namespace Tools
{
    public interface ITimeProvider
    {
        DateTime Now();
        DateTime Today();
    }
}
