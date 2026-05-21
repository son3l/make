using make.Models;
namespace make.Service;

internal static class MakeFileParser
{
    /// <summary>
    /// Парсит с файла все задачи с их зависимостями
    /// </summary>
    /// <param name="path">путь до файла</param>
    /// <returns>список задач</returns>
    /// <exception cref="FileNotFoundException"> не нашелся файл</exception>
    /// <exception cref="InvalidDataException"> некорректное содержание файла</exception>
    public async static Task<List<MakeTask>> Parse(string path)
    {
        // нет файла => выход
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("makefile not found");
        }
        string? currentLine;
        int currentLineNumber = 0;
        Dictionary<string, MakeTask> tasks = [];
        Dictionary<string, List<string>> taskDependencies = [];
        string[] parts;
        MakeTask? currentTask = null;
        using StreamReader stream = new(path);
        while ((currentLine = await stream.ReadLineAsync()) is not null)
        {
            currentLineNumber++;
            // пропускаем пустые строки
            if (string.IsNullOrWhiteSpace(currentLine))
                continue;
            // если строка начинается с пробела(ов), то это action
            if (char.IsWhiteSpace(currentLine[0]))
            {
                if (currentTask is null)
                    throw new InvalidDataException($"action without target at line {currentLineNumber}");
                currentTask.Actions.Add(currentLine.Trim());
                continue;
            }
            // сплитуем строку, если всего 1 значение, то без зависимостей, если 2, то с зависимостями, если >2 ошибка
            parts = currentLine.Split(':');
            string taskName = string.Empty;
            List<string> dependencies = [];
            if (parts.Length == 1)
            {
                if (string.IsNullOrWhiteSpace(parts[0]))
                    throw new InvalidDataException($"invalid task declaration at line {currentLineNumber}");
                taskName = parts[0].Trim();
            }
            else if (parts.Length == 2)
            {
                if (string.IsNullOrWhiteSpace(parts[0]))
                    throw new InvalidDataException($"invalid task declaration at line {currentLineNumber}");
                taskName = parts[0].Trim();
                // решил сделать distinct, а не ругаться на дубляж 
                var currentTaskDependencies = parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Distinct().ToList();
                if (currentTaskDependencies.Count == 0)
                    throw new InvalidDataException($"invalid task declaration at line {currentLineNumber}");
                if (currentTaskDependencies.Contains(taskName))
                    throw new InvalidDataException($"task cannot depend on itself at line {currentLineNumber}");
                dependencies = [.. currentTaskDependencies];
            }
            else
            {
                throw new InvalidDataException($"invalid task declaration at line {currentLineNumber}");
            }
            currentTask = new MakeTask
            {
                Name = taskName,
            };
            // если добавили задачу, то добавляем её зависимости
            if (!tasks.TryAdd(currentTask.Name, currentTask))
                throw new InvalidDataException($"duplicate task declaration at line {currentLineNumber}");
            else
                taskDependencies.Add(currentTask.Name, dependencies);
        }
        // для каждой задачи добавляем её зависимости
        foreach (var task in tasks)
        {
            foreach (var depName in taskDependencies[task.Key])
            {
                if (!tasks.ContainsKey(depName))
                    throw new InvalidDataException($"task '{task.Key}' depends on non-existent task '{depName}'");
                task.Value.Dependencies.Add(tasks[depName]);
            }
        }
        return [.. tasks.Select(kv => kv.Value)];
    }
}
