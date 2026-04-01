namespace Yusr.Core.Abstractions.Utilities
{
    public static class YusrMath
    {
        public static decimal Round(decimal num)
        {
            return Math.Round(num, 2, MidpointRounding.AwayFromZero);
        }

        public static decimal GetFactor(decimal percentage)
        {
            return Round((100 + percentage) / 100);
        }
    }
}