namespace DataAnalysis.Infrastructure.Snapshots;

internal static class SnapshotKeys
{
    public const string Prefix = "snap";
    public const string CurrentPointer = "snap:current";

    public static string Data(string version, string logicalKey) => $"{Prefix}:{version}:{logicalKey}";

    public static string Index(string version) => $"{Prefix}:{version}:__index";
}