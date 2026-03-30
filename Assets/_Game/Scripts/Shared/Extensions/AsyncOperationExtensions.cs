using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Game.Scripts.Shared.Extensions
{
    public static class AsyncOperationExtensions
    {
        public static Task AsTask(this AsyncOperation operation)
        {
            var tcs = new TaskCompletionSource<bool>();

            operation.completed += _ => tcs.TrySetResult(true);
            return tcs.Task;
        }
    }
}
