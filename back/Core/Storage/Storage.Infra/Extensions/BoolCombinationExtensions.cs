using System;
using Tools;

namespace Storage.Infra.Extensions
{
    public static class BoolCombinationExtensions
    {
        public static bool ToBoolean(this BoolCombination boolCombination)
        {
            return boolCombination switch
            {
                BoolCombination.TrueOnly => true,
                BoolCombination.FalseOnly => false,
                _ => throw new ApplicationException($"Unexpected bool combination value {boolCombination}")
            };
        }
    }
}
