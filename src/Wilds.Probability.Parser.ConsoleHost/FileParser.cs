namespace Wilds.Probability.Parser;

/// <summary>
/// Handles file operations for the parser.
/// </summary>
public class FileParser
{
    /// <summary>
    /// Reads the monster data from a file asynchronously and yields each monster as a string.
    /// </summary>
    /// <param name="file">Path to the input file.</param>
    /// <returns>A string representing a serialized single encounter.</returns>
    public static async IAsyncEnumerable<string> GetMonstersFromFileAsync(string file)
    {
        var text = File.ReadAllTextAsync(file);
        foreach (var monster in (await text).Split("\r\n\r\n", StringSplitOptions.RemoveEmptyEntries))
        {
            yield return monster;
        }
    }
}
