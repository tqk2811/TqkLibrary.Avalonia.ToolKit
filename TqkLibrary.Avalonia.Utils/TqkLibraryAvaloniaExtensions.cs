using Avalonia.Threading;
using TqkLibrary.Avalonia.Utils.Interfaces;

namespace TqkLibrary.Avalonia.Utils
{
    public static class TqkLibraryAvaloniaExtensions
    {
        public static System.Drawing.Color ToDrawingColor(this global::Avalonia.Media.Color color)
            => System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        public static global::Avalonia.Media.Color ToAvaloniaColor(this System.Drawing.Color color)
            => global::Avalonia.Media.Color.FromArgb(color.A, color.R, color.G, color.B);



        #region TrueThreadInvokeAsync
        public static Task TrueThreadInvokeAsync(
            this IMainThread mainThread,
            Action action,
            DispatcherPriority priority = default,
            CancellationToken cancellationToken = default
            )
            => mainThread.Dispatcher.TrueThreadInvokeAsync(action, priority, cancellationToken);
        public static async Task TrueThreadInvokeAsync(
            this Dispatcher dispatcher,
            Action action,
            DispatcherPriority priority = default,
            CancellationToken cancellationToken = default
            )
        {
            if (dispatcher is null) throw new ArgumentNullException(nameof(dispatcher));
            if (action is null) throw new ArgumentNullException(nameof(action));
            if (dispatcher.CheckAccess())
            {
                action.Invoke();
            }
            else
            {
                await dispatcher.InvokeAsync(action, priority, cancellationToken);
            }
        }

        public static Task<T> TrueThreadInvokeAsync<T>(
            this IMainThread mainThread,
            Func<T> func,
            DispatcherPriority priority = default,
            CancellationToken cancellationToken = default
            )
            => mainThread.Dispatcher.TrueThreadInvokeAsync(func, priority, cancellationToken);
        public static async Task<T> TrueThreadInvokeAsync<T>(
            this Dispatcher dispatcher,
            Func<T> func,
            DispatcherPriority priority = default,
            CancellationToken cancellationToken = default
            )
        {
            if (dispatcher is null) throw new ArgumentNullException(nameof(dispatcher));
            if (func is null) throw new ArgumentNullException(nameof(func));
            if (dispatcher.CheckAccess())
            {
                return func.Invoke();
            }
            else
            {
                return await dispatcher.InvokeAsync<T>(func, priority, cancellationToken);
            }
        }

        #endregion


        #region  ICollection<T>
        public static Task AddAsync<T>(this IMainThreadCollection<T> collection, T item, CancellationToken cancellationToken = default)
            => collection.Dispatcher.TrueThreadInvokeAsync(() => collection.Add(item), cancellationToken: cancellationToken);
        public static Task ClearAsync<T>(this IMainThreadCollection<T> collection, CancellationToken cancellationToken = default)
            => collection.Dispatcher.TrueThreadInvokeAsync(() => collection.Clear(), cancellationToken: cancellationToken);
        public static Task<bool> RemoveAsync<T>(this IMainThreadCollection<T> collection, T item, CancellationToken cancellationToken = default)
            => collection.Dispatcher.TrueThreadInvokeAsync(() => collection.Remove(item), cancellationToken: cancellationToken);
        public static Task InsertAsync<T>(this IMainThreadList<T> list, int index, T item, CancellationToken cancellationToken = default)
            => list.Dispatcher.TrueThreadInvokeAsync(() => list.Insert(index, item), cancellationToken: cancellationToken);
        public static Task RemoveAtAsync<T>(this IMainThreadList<T> list, int index, CancellationToken cancellationToken = default)
            => list.Dispatcher.TrueThreadInvokeAsync(() => list.RemoveAt(index), cancellationToken: cancellationToken);
        #endregion
    }
}
