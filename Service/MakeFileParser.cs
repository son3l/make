using make.Models;
namespace make.Service;

internal static class MakeFileParser
{
    public async static Task<Dictionary<string, MakeTask>> Parse(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("makefile not found");
        }
        string? currentLine;
        int currentLineNumber = 0;
        Dictionary<string, MakeTask> tasks = [];
        string[] parts;
        MakeTask? currentTask = null;
        using StreamReader stream = new(path);
        while ((currentLine = await stream.ReadLineAsync()) is not null)
        {
            currentLineNumber++;
            if (string.IsNullOrWhiteSpace(currentLine))
                continue;
            if (char.IsWhiteSpace(currentLine[0]))
            {
                if (currentTask is null)
                {
                    throw new InvalidDataException($"action without target at line {currentLineNumber}");
                }
                currentTask.Actions.Add(currentLine.Trim());
                continue;
            }
            parts = currentLine.Split(':');
            string taskName = string.Empty;
            List<string> dependencies = [];
            if (parts.Length == 1)
            {
                if (string.IsNullOrWhiteSpace(parts[0]))
                {
                    throw new InvalidDataException($"invalid task declaration at line {currentLineNumber}");
                }
                taskName = parts[0].Trim();
            }
            else if (parts.Length == 2)
            {
                if (string.IsNullOrWhiteSpace(parts[0]))
                {
                    throw new InvalidDataException($"invalid task declaration at line {currentLineNumber}");
                }
                taskName = parts[0].Trim();

                var currentTaskDependencies = parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Distinct().ToList();
                if (currentTaskDependencies.Count == 0)
                {
                    throw new InvalidDataException($"invalid task declaration at line {currentLineNumber}");
                }
                if (currentTaskDependencies.Contains(taskName))
                {
                    throw new InvalidDataException($"task cannot depend on itself at line {currentLineNumber}");
                }
                dependencies = [.. currentTaskDependencies];
            }
            else
            {
                throw new InvalidDataException($"invalid task declaration at line {currentLineNumber}");
            }

            currentTask = new MakeTask
            {
                Name = taskName,
                Dependencies = dependencies,
            };
            if (!tasks.TryAdd(currentTask.Name, currentTask))
            {
                throw new InvalidDataException($"duplicate task declaration at line {currentLineNumber}");
            }
        }
        return tasks;
    }
}
