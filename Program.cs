using make.Models;
using make.Service;

/*if (args.Length == 0)
{
    Console.WriteLine("no task specified");
    return;
}*/
Dictionary<string, MakeTask> tasks;
try
{
    tasks = await MakeFileParser.Parse("makefile");
}
catch (Exception ex)
{
    Console.WriteLine($"error: {ex.Message}");
    return;
}


Console.WriteLine(string.Join("\n", tasks));
