using make.Models;

namespace make.Service
{
    internal static class MakeTaskExtensions
    {
        /// <summary>
        /// Сортирует задачи от последнего предка до необходимой задачи
        /// </summary>
        /// <param name="task">задача у которой необходимо отсортировать зависимости</param>
        /// <returns>отсортированный список задач от самого предка до необходимой задачи</returns>
        public static List<MakeTask> SortDependencies(this MakeTask task)
        {
            List<MakeTask> sortedTasks = [];
            HashSet<MakeTask> visitedTasks = [];
            Stack<(MakeTask task, bool visited)> stack = new();
            stack.Push((task, false));
            while (stack.Count > 0)
            {
                var (currentTask, visited) = stack.Pop();
                if (visited)
                {
                    visitedTasks.Add(currentTask);
                    sortedTasks.Add(currentTask);
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
            return sortedTasks;
        }
    }
}
