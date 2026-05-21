using make.Models;
using make.Service;

if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
{
    Console.WriteLine("no task specified");
    return;
}
Dictionary<string, MakeTask> tasks;
try
{
    tasks = await MakeFileParser.Parse("makefile");
    var resolvedTasks = TaskDependencyResolver.Resolve(tasks, args[0]);
    foreach(var task in resolvedTasks)
    {
        Console.WriteLine(string.Join("\n", tasks[task].Actions));
    }
}
catch (Exception ex)
{
    Console.WriteLine($"error: {ex.Message}");
    return;
}


//Console.WriteLine(string.Join("\n", tasks));
