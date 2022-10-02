using System.Runtime.CompilerServices;

namespace SwedishAgoraDecentralized.Shared
{
    public sealed class LazyTask<T>
    {
        private readonly Lazy<Task<T>> _lazyTask;
        public LazyTask(Func<Task<T>> func)
        {
            _lazyTask = new Lazy<Task<T>>(func);
        }

        public TaskAwaiter<T> GetAwaiter() => _lazyTask.Value.GetAwaiter();
    }
}