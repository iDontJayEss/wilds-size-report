using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Wilds.Probability.Parser;

/// <summary>
/// Represents the data for a monster, including its name, internal Id, and variations.
/// </summary>
public class MonsterData
{
    /// <summary>
    /// The name of the monster.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The internal Id of the monster.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The total number of encounters recorded for this monster across all variations.
    /// </summary>
    public int TotalEncounters => Variations.Sum(v => v.Encounters.Count);

    /// <summary>
    /// A monster variation is a set of encounters that share the same scale probability.
    /// </summary>
    /// <remarks>
    /// The script used to generate the data from the game does not record any form of classification for the variation.
    /// Aspects that may be used to classify the variation include: Locale, Difficulty, Tempered/Frenzied, etc.
    /// </remarks>
    public List<MonsterSizeTable> Variations { get; set; } = [];

    /// <summary>
    /// Merges the data from another <see cref="MonsterData"/> instance into this instance.
    /// </summary>
    /// <param name="data">The monster to merge.</param>
    public void Merge(MonsterData data)
    {
        foreach (var variation in data.Variations)
        {
            if (Variations.FirstOrDefault(v => v.IsSame(variation.ScaleProbability)) is MonsterSizeTable table)
            {
                table.Encounters.AddRange(variation.Encounters);
            }
            else
            {
                Variations.Add(variation);
            }
        }
    }
}

/// <summary>
/// Represents the size probability table for a monster variation.
/// </summary>
public class MonsterSizeTable : IDictionary<int, int>
{
    private const int tinyCrown = 90;

    private const int goldCrown = 123;

    /// <inheritdoc/>
    public int this[int key] { get => ((IDictionary<int, int>)ScaleProbability)[key]; set => ((IDictionary<int, int>)ScaleProbability)[key] = value; }

    /// <summary>
    /// The scale probability table for the monster variation.
    /// </summary>
    /// <remarks>
    /// The key is the size of the monster, and the value is the probability of that size.
    /// </remarks>
    public Dictionary<int, int> ScaleProbability { get; set; } = [];

    /// <inheritdoc/>
    public ICollection<int> Keys => ((IDictionary<int, int>)ScaleProbability).Keys;

    /// <inheritdoc/>
    public ICollection<int> Values => ((IDictionary<int, int>)ScaleProbability).Values;

    /// <inheritdoc/>
    public int Count => ((ICollection<KeyValuePair<int, int>>)ScaleProbability).Count;

    /// <inheritdoc/>
    public bool IsReadOnly => ((ICollection<KeyValuePair<int, int>>)ScaleProbability).IsReadOnly;

    /// <inheritdoc/>
    public void Add(int key, int value) => ((IDictionary<int, int>)ScaleProbability).Add(key, value);

    /// <inheritdoc/>
    public void Add(KeyValuePair<int, int> item) => ((ICollection<KeyValuePair<int, int>>)ScaleProbability).Add(item);

    /// <inheritdoc/>
    public void Clear() => ((ICollection<KeyValuePair<int, int>>)ScaleProbability).Clear();

    /// <inheritdoc/>
    public bool Contains(KeyValuePair<int, int> item) => ((ICollection<KeyValuePair<int, int>>)ScaleProbability).Contains(item);

    /// <inheritdoc/>
    public bool ContainsKey(int key) => ((IDictionary<int, int>)ScaleProbability).ContainsKey(key);

    /// <inheritdoc/>
    public void CopyTo(KeyValuePair<int, int>[] array, int arrayIndex) => ((ICollection<KeyValuePair<int, int>>)ScaleProbability).CopyTo(array, arrayIndex);

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<int, int>> GetEnumerator() => ((IEnumerable<KeyValuePair<int, int>>)ScaleProbability).GetEnumerator();

    /// <inheritdoc/>
    public bool Remove(int key) => ((IDictionary<int, int>)ScaleProbability).Remove(key);

    /// <inheritdoc/>
    public bool Remove(KeyValuePair<int, int> item) => ((ICollection<KeyValuePair<int, int>>)ScaleProbability).Remove(item);

    /// <inheritdoc/>
    public bool TryGetValue(int key, [MaybeNullWhen(false)] out int value) => ((IDictionary<int, int>)ScaleProbability).TryGetValue(key, out value);

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)ScaleProbability).GetEnumerator();

    /// <summary>
    /// Gets the minimum size of the monster variation.
    /// </summary>
    public int MinSize => ScaleProbability.Where(kvp => kvp.Value > 0).Min(kvp => kvp.Key);

    /// <summary>
    /// Gets the maximum size of the monster variation.
    /// </summary>
    public int MaxSize => ScaleProbability.Where(kvp => kvp.Value > 0).Max(kvp => kvp.Key);

    /// <summary>
    /// Gets the percentage of encounters that are tiny crowns.
    /// </summary>
    /// <remarks>
    /// This number is based on the (unverified) assumption that sizes 90 and below are tiny crowns.
    /// </remarks>
    public int TinyCrownPercent => ScaleProbability.Where(kvp => kvp.Key <= tinyCrown).Sum(kvp => kvp.Value);

    /// <summary>
    /// Gets the percentage of encounters that are gold crowns.
    /// </summary>
    /// <remarks>
    /// This number is based on the (unverified) assumption that sizes 123 and above are gold crowns.
    /// </remarks>
    public int GoldCrownPercent => ScaleProbability.Where(kvp => kvp.Key >= goldCrown).Sum(kvp => kvp.Value);

    /// <summary>
    /// Gets the average size of the monster variation.
    /// </summary>
    public double AverageSize => GetAverageSize(ScaleProbability);

    private static double GetAverageSize(IReadOnlyDictionary<int, int> scaleProbability)
    {
        var weightedTotal = scaleProbability.Select(kvp => .01 * kvp.Value * kvp.Key).Sum();
        return weightedTotal;
    }

    /// <summary>
    /// Checks if the scale probability table is the same as another scale probability table.
    /// </summary>
    /// <param name="scaleProbability">The <see cref="ScaleProbability"/> table to check against this instance.</param>
    /// <returns></returns>
    public bool IsSame(IReadOnlyDictionary<int, int> scaleProbability)
    {
        if (scaleProbability.Count != ScaleProbability.Count)
            return false;

        if (!scaleProbability.Keys.All(Keys.Contains))
            return false;

        return scaleProbability.Keys.All(key => ScaleProbability[key] == scaleProbability[key]);
    }

    /// <summary>
    /// The list of actual sizes from encounters recorded for this monster variation.
    /// </summary>
    public List<int> Encounters { get; set; } = [];
}