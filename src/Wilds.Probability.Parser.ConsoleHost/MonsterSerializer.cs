namespace Wilds.Probability.Parser;
internal class MonsterSerializer
{
    public static MonsterData Deserialize(string monster)
    {
        var result = new MonsterData();
        var variant = new MonsterSizeTable();
        result.Variations.Add(variant);
        foreach (var line in monster.Split("\r\n", StringSplitOptions.RemoveEmptyEntries))
        {
            if (line.StartsWith("  "))
            {
                var (scale, probability) = GetScaleProbability(line);
                variant[scale] = probability;
            }
            else if (line.StartsWith("Creating"))
            {
                var (name, id, size) = AddMonsterInformation(line);
                result.Name = name;
                result.Id = id;
                variant.Encounters.Add(size);
            }
        }
        return result;
    }

    private static KeyValuePair<int, int> GetScaleProbability(string line)
    {
        line = line.Trim();
        var parts = line.Split("->", StringSplitOptions.RemoveEmptyEntries);
        return new(int.Parse(parts[0]), int.Parse(parts[1]));
    }

    private static (string name, int id, int size) AddMonsterInformation(string line)
    {
        line = line.Replace("Creating enemy ", "");
        var monsterName = line.Split("(")[0];
        var monsterId = int.Parse(line.Split("(")[1].Split(")")[0]);
        var monsterSize = int.Parse(line.Split("with size ")[1].Trim());

        return (monsterName, monsterId, monsterSize);
    }
}
