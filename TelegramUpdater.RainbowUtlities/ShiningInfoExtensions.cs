using System.Diagnostics.CodeAnalysis;

namespace TelegramUpdater.RainbowUtlities
{
    public static class ShiningInfoExtensions
    {
        /// <summary>
        /// Tries to drop all pending objects for a given queue.
        /// </summary>
        public static bool DropPendingAsync<TId, TValue>(
            this ShinigInfo<TId, TValue> shinigInfo) where TId : struct
            => shinigInfo.Rainbow.DropPendingAsync(shinigInfo.ProcessId);

        /// <summary>
        /// Tries to count pending objects of the queue.
        /// </summary>
        /// <param name="count">Returned count of the queue.</param>
        /// <returns></returns>
        public static bool TryCountPending<TId, TValue>(
            this ShinigInfo<TId, TValue> shinigInfo, [NotNullWhen(true)] out int? count) where TId : struct
            => shinigInfo.Rainbow.TryCountPending(shinigInfo.ProcessId, out count);
    }
}
