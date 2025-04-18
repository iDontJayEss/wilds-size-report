using System.Text;

namespace Wilds.Probability.Parser;

/// <summary>
/// Handles writing the results of the parser to a file.
/// </summary>
public static class ResultWriter
{
    /// <summary>
    /// Writes the results of the parser to a file asynchronously.
    /// </summary>
    /// <param name="file">File to write the report to.</param>
    /// <param name="monsters">The collection of deserialized monster data.</param>
    /// <param name="totals">The aggregated sums from the input.</param>
    /// <returns>A task that will complete when the report file has finished writing.</returns>
    public static async Task WriteResultAsync(string file, IEnumerable<MonsterData> monsters, MonsterData totals)
    {
        var result = new StringBuilder();
        result.AppendLine("Monster Report");
        result.AppendLine("=============");
        result.AppendLine("This report is generated from the Wilds Probability Parser.");
        result.AppendLine($"It has been generated on {DateTime.Now} based on data from {monsters.Sum(static m => m.Variations.Sum(static v => v.Encounters.Count))} encounters with {monsters.Count()} different monsters");
        result.AppendLine("==============");
        foreach (var monster in monsters)
        {
            result = WriteMonster(result, monster);
        }

        result.AppendLine("====== AGGREGATED TOTALS ========");
        WriteMonster(result, totals);

        await File.WriteAllTextAsync(file, result.ToString());
    }

    private static StringBuilder WriteMonster(StringBuilder result, MonsterData monster)
    {
        result.AppendLine($"Report for {monster.Name} ({monster.Id}):");
        result.AppendLine($" Total Encounters: {monster.Variations.Sum(v => v.Encounters.Count)}");
        result.AppendLine($" Variants: {monster.Variations.Count}");
        var variantNumber = 1;
        foreach (var variant in monster.Variations)
        {
            result = WriteVariant(result, variant, variantNumber);
            variantNumber++;
        }

        return result;
    }

    private static StringBuilder WriteVariant(StringBuilder result, MonsterSizeTable variant, int variantNumber)
    {
        result.AppendLine($"  Variant {variantNumber}:");
        result.AppendLine($"   Size Range(% of 100% scale): [{variant.MinSize}-{variant.MaxSize}]");
        result.AppendLine($"   Tiny Crown Odds: {variant.TinyCrownPercent}%");
        result.AppendLine($"   Gold Crown Odds: {variant.GoldCrownPercent}%");
        result.AppendLine($"   Average Size: {variant.AverageSize:F}");
        result.AppendLine($"   Probability Table (Size->%Chance):");
        foreach (var scaleProbability in variant.ScaleProbability)
        {
            result.AppendLine($"    {scaleProbability.Key}/100 -> {scaleProbability.Value}%");
        }

        result.AppendLine($"   Actual sizes from {variant.Encounters.Count} Encounters with this variant: {string.Join(", ", variant.Encounters)}");
        return result;
    }
}
