using System;
using Tools;

namespace Storage.Infra.Extensions
{
    public static class CompareBooleanExtensions
    {
        public static bool ToBoolean(this CompareBoolean compareBoolean)
        {
            return compareBoolean switch
            {
                CompareBoolean.TrueOnly => true,
                CompareBoolean.FalseOnly => false,
                _ => throw new ApplicationException($"Unexpected bool combination value {compareBoolean}")
            };
        }
    }
}
