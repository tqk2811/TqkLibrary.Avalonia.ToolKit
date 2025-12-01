using System.Collections.Specialized;
using TqkLibrary.Avalonia.ToolKit;
using TqkLibrary.Avalonia.ToolKit.Interfaces;
using TqkLibrary.Avalonia.ToolKit.Interfaces.Services;

namespace TqkLibrary.Avalonia.ToolKit.Collections.ObservableCollections
{
    public class SaveStorageObservableCollection<TData, TViewModel> : SaveObservableCollection<TData, TViewModel>
      where TData : class
      where TViewModel : class, IViewModel<TData>
    {
        public SaveStorageObservableCollection(
            IDataStorageService dataStorageService,
            string? key = null
            )
        {

        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TViewModel"></typeparam>
    public class SaveObservableCollection<TData, TViewModel> : DispatcherObservableCollection<TViewModel>
      where TData : class
      where TViewModel : class, IViewModel<TData>
    {
        public class UpdateData
        {
            public required IEnumerable<TData> CurrentDatas { get; set; }
            public IEnumerable<TData>? ChangedDatas { get; set; }

            public int? NewStartingIndex { get; set; }
            public IEnumerable<TData>? NewDatas { get; set; }

            public int? OldStartingIndex { get; set; }
            public IEnumerable<TData>? OldDatas { get; set; }

        }
        private class LockHelper : IDisposable
        {
            readonly SaveObservableCollection<TData, TViewModel> _collection;
            public LockHelper(SaveObservableCollection<TData, TViewModel> collection)
            {
                _collection = collection ?? throw new ArgumentNullException(nameof(collection));
                Interlocked.Increment(ref _collection._lockCount);
            }
            ~LockHelper()
            {
                Interlocked.Decrement(ref _collection._lockCount);
            }
            public void Dispose()
            {
                Interlocked.Decrement(ref _collection._lockCount);
                GC.SuppressFinalize(this);
            }
        }
        public delegate void SaveCallBack(UpdateData updateData);


        /// <summary>
        /// 
        /// </summary>
        public event SaveCallBack? OnSave;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="datas"></param>
        /// <param name="func"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual void Load<TInput>(IEnumerable<TInput> datas, Func<TInput, TViewModel> func)
        {
            if (datas is null) throw new ArgumentNullException(nameof(datas));
            if (func is null) throw new ArgumentNullException(nameof(func));

            Dispatcher.VerifyAccess();

            using var l = GetLockLoader();
            Clear();
            foreach (var item in datas)
            {
                Add(func(item));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="datas"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual Task LoadAsync<TInput>(IEnumerable<TInput> datas, Func<TInput, TViewModel> func)
            => Dispatcher.TrueThreadInvokeAsync(() => Load(datas, func));

        /// <summary>
        /// 
        /// </summary>
        protected bool IsLoaded => _lockCount == 0;

        private int _lockCount = 0;
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected IDisposable GetLockLoader()
        {
            Dispatcher.VerifyAccess();
            return new LockHelper(this);
        }



        /// <summary>
        /// 
        /// </summary>
        public virtual void Save()
            => TriggerEventSave(this.Select(x => x.Data));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="datas"></param>
        protected virtual void TriggerEventSave(IEnumerable<TData> datas)
            => TriggerEventSave(new UpdateData() { CurrentDatas = datas });
        /// <summary>
        /// 
        /// </summary>
        /// <param name="updateData"></param>
        protected virtual void TriggerEventSave(UpdateData updateData)
            => OnSave?.Invoke(updateData);


        #region ObservableCollection
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected override void InsertItem(int index, TViewModel item)
        {
            item.Change += ItemData_Change;
            base.InsertItem(index, item);
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void ClearItems()
        {
            foreach (var item in this) item.Change -= ItemData_Change;
            base.ClearItems();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        protected override void RemoveItem(int index)
        {
            this[index].Change -= ItemData_Change;
            base.RemoveItem(index);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected override void SetItem(int index, TViewModel item)
        {
            this[index].Change -= ItemData_Change;
            item.Change += ItemData_Change;
            base.SetItem(index, item);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (IsLoaded)
            {
                TriggerEventSave(new UpdateData()
                {
                    CurrentDatas = this.Select(x => x.Data),
                    ChangedDatas = null,
                    NewStartingIndex = e.NewStartingIndex,
                    NewDatas = e.NewItems?.Cast<TViewModel>().Select(x => x.Data),
                    OldStartingIndex = e.OldStartingIndex,
                    OldDatas = e.OldItems?.Cast<TViewModel>().Select(x => x.Data)
                });
            }
            base.OnCollectionChanged(e);
        }

        #endregion ObservableCollection

        protected virtual void OnItemChanged(TData data)
        {

        }
        private void ItemData_Change(object obj, TData data)
        {
            OnItemChanged(data);
            TriggerEventSave(new UpdateData()
            {
                CurrentDatas = this.Select(x => x.Data),
                ChangedDatas = new[] { data },
            });
        }



    }
}