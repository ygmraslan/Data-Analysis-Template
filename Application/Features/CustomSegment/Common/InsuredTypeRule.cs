namespace DataAnalysis.Application.Features.CustomSegment.Common;
internal static class InsuredTypeRule
{
    public const string TuzelAgeBand = "0-17";

    private static readonly string[] TuzelMatchers = { "TUZEL", "TÜZEL" };

    public static bool ContainsTuzel(IEnumerable<string>? insuredTypes)
    {
        if (insuredTypes == null) return false;
        foreach (var t in insuredTypes)
        {
            if (string.IsNullOrWhiteSpace(t)) continue;
            var upper = t.ToUpperInvariant();
            foreach (var m in TuzelMatchers)
            {
                if (upper.Contains(m)) return true;
            }
        }
        return false;
    }

    public static bool IsAgeCombinationValid(IEnumerable<string>? insuredTypes, IEnumerable<string>? insuredAges)
    {
        if (!ContainsTuzel(insuredTypes)) return true;
        if (insuredAges == null) return true;

        foreach (var age in insuredAges)
        {
            if (string.IsNullOrWhiteSpace(age)) continue;
            if (!string.Equals(age.Trim(), TuzelAgeBand, System.StringComparison.Ordinal))
            {
                return false;
            }
        }
        return true;
    }
}