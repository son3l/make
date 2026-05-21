using make.Models;
using make.Service;

if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
{
    Console.WriteLine("no task specified");
    return;
}
List<MakeTask> tasks;
try
{
    tasks = await MakeFileParser.Parse("makefile");
}
catch (Exception ex)
{
    Console.WriteLine($"error: {ex.Message}");
    return;
}

var task = tasks.FirstOrDefault(t => t.Name == args[0]);
if (task is null)
{
    Console.WriteLine($"task '{args[0]}' not found");
    return;
}
foreach (var dep in task.SortDependencies())
{
    Console.WriteLine(string.Join("\n", dep.Actions));
}
