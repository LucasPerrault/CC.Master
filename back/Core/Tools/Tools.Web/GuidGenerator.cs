using System;

namespace Tools.Web;

public class GuidGenerator : IGuidGenerator
{
    public Guid New()
    {
        return Guid.NewGuid();
    }
}
