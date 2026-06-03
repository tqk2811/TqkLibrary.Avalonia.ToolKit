using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using TqkLibrary.Avalonia.ToolKit.Collections.ObservableCollections;

namespace TqkLibrary.Avalonia.ToolKit.Desktop.Collections.ObservableCollections
{
    /// <summary>
    /// ObservableCollection giới hạn số phần tử và (tuỳ chọn) ghi mỗi item ra file log.
    /// Đặt ở gói Desktop vì phụ thuộc filesystem (StreamWriter) - không dùng được trên Browser/WASM.
    /// Bản Avalonia của TqkLibrary.WpfUi.ObservableCollections.LimitObservableCollection.
    /// Lưu ý: <see cref="System.Collections.ObjectModel.ObservableCollection{T}"/> không an toàn đa luồng;
    /// hãy gọi <c>Add</c> trên UI thread (hoặc marshal trước khi gọi).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LimitObservableCollection<T> : DispatcherObservableCollection<T>
    {
        /// <summary>
        ///
        /// </summary>
        public LimitObservableCollection()
        {
            this.LogPath = () => Path.Combine(Directory.GetCurrentDirectory(), $"{DateTime.Now:yyyy-MM-dd}.log");
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="delegatePath">Hàm trả về đường dẫn file log (gọi mỗi lần ghi).</param>
        /// <param name="limit">Số phần tử tối đa giữ trong collection.</param>
        /// <param name="isInsertTop">Chèn item mới lên đầu (true) hay cuối (false).</param>
        public LimitObservableCollection(Func<string> delegatePath, int limit = 500, bool isInsertTop = true)
        {
            this.LogPath = delegatePath;
            this.Limit = limit;
            this.IsInsertTop = isInsertTop;
        }

        /// <summary>
        ///
        /// </summary>
        public int Limit { get; set; } = 100;
        /// <summary>
        ///
        /// </summary>
        public bool IsInsertTop { get; set; } = true;
        /// <summary>
        ///
        /// </summary>
        public bool IsExportToFile { get; set; } = true;
        /// <summary>
        ///
        /// </summary>
        public Func<string> LogPath { get; set; }

        /// <inheritdoc/>
        protected override void InsertItem(int index, T item)
        {
            if (this.Count == this.Limit)
                base.RemoveItem(this.IsInsertTop ? this.Count - 1 : 0);
            if (this.LogPath != null && this.IsExportToFile)
                AppendFile(item);
            base.InsertItem(this.IsInsertTop ? 0 : this.Count, item);
        }

        /// <inheritdoc/>
        protected override void MoveItem(int oldIndex, int newIndex)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        protected override void SetItem(int index, T item)
        {
            throw new NotSupportedException();
        }

        void AppendFile(T item)
        {
            string? data = item?.ToString();
            if (string.IsNullOrWhiteSpace(data))
                return;
            string path = this.LogPath.Invoke();
            if (string.IsNullOrEmpty(path))
                return;
            try
            {
                using StreamWriter sw = new StreamWriter(path, true, Encoding.UTF8);
                sw.WriteLine(data);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error writing to log file: {ex.Message}");
            }
        }
    }
}
