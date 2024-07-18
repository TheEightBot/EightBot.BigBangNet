using System;
using System.Threading.Tasks;

namespace EightBot.BigBang.Interfaces
{
    public interface IBackgroundTask
    {
        Task ProcessInBackground(Func<Task> backgroundTask);

        Task<T> ProcessInBackground<T>(Func<Task<T>> backgroundTask);
    }
}
