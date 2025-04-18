// See https://aka.ms/new-console-template for more information
using Wilds.Probability.Parser;

var i = 0;
var results = new Dictionary<string, MonsterData>();

await foreach (var monster in FileParser.GetMonstersFromFileAsync("input/monster_probability.dat"))
{
    i++;
    var monsterData = MonsterSerializer.Deserialize(monster);
    Console.WriteLine($"{i} | Adding {monsterData.Name} encounter");
    if (results.TryGetValue(monsterData.Name, out var existing))
    {
        existing.Merge(monsterData);
    }
    else
    {
        results.Add(monsterData.Name, monsterData);
    }
}

var commonData = new MonsterData
{
    Name = "Common",
    Id = 0,
    Variations = []
};

foreach (var monster in results.Values)
{
    commonData.Merge(monster);
}

Console.WriteLine($"Encounters recorded for {results.Count} different monsters");
Console.WriteLine($"Monsters: {string.Join(", ", results.Keys)}");
Console.WriteLine($"Variant Counts:");
foreach (var monster in results)
{
    Console.WriteLine($"\t{monster.Key}: {monster.Value.Variations.Count}");
}

Console.WriteLine("Writing results to file");
var resultFile = $"monster_report.{DateTime.Now:MM_dd_hh_mm}.txt";
await ResultWriter.WriteResultAsync(resultFile, results.Values, commonData);
Console.WriteLine($"Results written to {resultFile}");