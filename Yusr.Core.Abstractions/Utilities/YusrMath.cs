namespace Yusr.Core.Abstractions.Utilities
{
    public static class YusrMath
    {
        public static decimal Round(decimal num)
        {
            return Math.Round(num, 2, MidpointRounding.AwayFromZero);
        }
    }
}