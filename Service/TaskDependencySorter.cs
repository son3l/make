using make.Models;

namespace make.Service
{
    internal static class TaskDependencySorter
    {
        public static List<MakeTask> Sort(Dictionary<string, MakeTask> tasks, string taskName)
        {
            if (!tasks.ContainsKey(taskName))
                throw new ArgumentException($"task {taskName} not found");
            List<MakeTask> resolvedTasks = [];
            HashSet<MakeTask> visitedTasks = [];
            Stack<(MakeTask task, bool visited)> stack = new();
            stack.Push((tasks[taskName], false));
            while (stack.Count > 0)
            {
                var (currentTask, visited) = stack.Pop();
                if (visited)
                {
                    visitedTasks.Add(currentTask);
                    resolvedTasks.Add(currentTask);
                    continue;
                }
                if (visitedTasks.Contains(currentTask))
                {
                    continue;
                }
                stack.Push((currentTask, true));
                foreach (var dependency in currentTask.Dependencies)
                {
                    stack.Push((dependency, false));
                }
            }
            return resolvedTasks;
        }
    }
}
