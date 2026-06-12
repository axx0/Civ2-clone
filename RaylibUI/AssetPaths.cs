namespace RaylibUI;

internal static class AssetPaths
{
    public static string Resolve(params string[] pathParts)
    {
        var relativePath = Path.Combine(pathParts);
        var candidates = new[]
        {
            Path.Combine(AppContext.BaseDirectory, relativePath),
            Path.Combine(Environment.CurrentDirectory, relativePath),
            Path.Combine(Environment.CurrentDirectory, "RaylibUI", relativePath)
        };

        return candidates.FirstOrDefault(File.Exists) ?? candidates[0];
    }
}
