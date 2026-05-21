using make.Models;

namespace make.Service
{
    internal static class TaskDependencyResolver
    {
        public static List<string> Resolve(Dictionary<string, MakeTask> tasks, string taskName)
        {
            if (!tasks.ContainsKey(taskName))
            {
                throw new ArgumentException($"task {taskName} not found");
            }
            List<string> resolvedTasks = [];
            HashSet<string> visitedTasks = [];
            Stack<(string Task, bool Visited)> stack = new();
            stack.Push((taskName, false));
            while (stack.Count > 0)
            {
                var (currentTaskName, visited) = stack.Pop();
                if (!tasks.ContainsKey(currentTaskName))
                {
                    throw new ArgumentException($"task {currentTaskName} not found");
                }
                if (visited)
                {
                    visitedTasks.Add(currentTaskName);
                    resolvedTasks.Add(currentTaskName);
                    continue;
                }
                if (visitedTasks.Contains(currentTaskName))
                {
                    continue;
                }
                stack.Push((currentTaskName, true));
                foreach (var dependency in tasks[currentTaskName].Dependencies)
                {
                    if (!tasks.ContainsKey(dependency))
                    {
                        throw new ArgumentException($"task {dependency} not found");
                    }
                    stack.Push((dependency, false));
                }
            }
            return resolvedTasks;
        }
    }
}
