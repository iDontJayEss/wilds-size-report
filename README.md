# Monster Hunter Wilds: Size Reporting

This application is used to report the odds of finding monsters with a given size in the game Monster Hunter Wilds.

## Not a Developer?

Open the report [here](reports/monster_report.04_18_04_31.txt) to see the results of the latest run.

## Script Requirements

Aside from lua skills, and the ability to read and understand the code, the following libraries are required:

- [RE Framework](https://github.com/praydog/reframework-nightly/releases)
- [CatLib Library](https://www.nexusmods.com/monsterhunterwilds/mods/65)

If you don't know where to go from there, you are not the intended audience.

## How it Works

When monsters are spawned, a table of their sizes and probabilities is passed to a random number generator. The size is then determined by the random number generator, and the size is reported to the console.
The script intercepts the call to this function and writes the table and result to a file.

Separately, the script also intercepts the call to the function that spawns monsters and writes the monster name and size to a file. This is joined with the size report using the monster size to produce the [input file](src/Wilds.Probability.Parser.ConsoleHost/input/monster_probability.dat) for the Console Application.

## Limitations

Because lua sucks and I suck at lua, the script does not classify the monsters based on tempered/frenzied, difficulty, or locale.