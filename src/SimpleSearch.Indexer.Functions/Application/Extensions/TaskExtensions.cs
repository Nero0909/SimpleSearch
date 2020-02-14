using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleSearch.Indexer.Functions.Application.Extensions
{
    public static class TaskExtensions
    {
        public static async Task Throttle(this IEnumerable<Task> taskToProcess, int concurrencyLevel)
        {
            using var taskEnumerator = taskToProcess.GetEnumerator();
            var preLoadedTasks = 0;
            var activeTasks = new List<Task>();
            while (preLoadedTasks < concurrencyLevel && taskEnumerator.MoveNext())
            {
                activeTasks.Add(taskEnumerator.Current);
                preLoadedTasks++;
            }

            while (activeTasks.Count > 0)
            {
                var finishedTask = await Task.WhenAny(activeTasks).ConfigureAwait(false);
                activeTasks.Remove(finishedTask);

                if (taskEnumerator.MoveNext())
                {
                    activeTasks.Add(taskEnumerator.Current);
                }
            }
        }
    }
}