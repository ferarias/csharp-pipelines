namespace Pipelines.ApiTests
{
    /// <summary>
    /// A stable string hash, unlike string.GetHashCode() which is randomized per process on .NET Core
    /// and therefore unsuitable for anything that needs the same input to produce the same output
    /// across runs (e.g. derived dictionary keys, or deterministic sample data).
    /// </summary>
    internal static class DeterministicHash
    {
        public static int Compute(string value)
        {
            unchecked
            {
                var hash = 23;
                foreach (var c in value)
                {
                    hash = hash * 31 + c;
                }
                return hash;
            }
        }
    }
}
