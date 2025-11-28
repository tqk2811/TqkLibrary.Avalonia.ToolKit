using Avalonia;
using Avalonia.Input;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using TqkLibrary.Avalonia.ToolKit.Interfaces;

namespace TqkLibrary.Avalonia.ToolKit.Collections.ObservableCollections
{
    public class DispatcherObservableCollection<T> : ObservableCollection<T>, IMainThreadCollection<T>
    {

        Cursor? _Cursor = null;
        public virtual Cursor? Cursor
        {
            get { return _Cursor; }
            set { _Cursor = value; OnPropertyChanged(nameof(Cursor)); }
        }

        T? _SelectedItem = default;
        public virtual T? SelectedItem
        {
            get { return _SelectedItem; }
            set { _SelectedItem = value; OnPropertyChanged(nameof(SelectedItem)); }
        }

        public event Action<IReadOnlyList<T>>? OnItemsCleared;
        public event Action<T>? OnItemAdded;
        public event Action<T>? OnItemRemoved;
        public Dispatcher Dispatcher { get; } = Dispatcher.UIThread;

        public DispatcherObservableCollection()
        {

        }


        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems is not null && OnItemAdded is not null)
                    {
                        foreach (var item in e.NewItems.OfType<T>())
                        {
                            OnItemAdded?.Invoke(item);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems is not null && OnItemRemoved is not null)
                    {
                        foreach (var item in e.OldItems.OfType<T>())
                        {
                            OnItemRemoved?.Invoke(item);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    if (e.NewItems is not null && OnItemAdded is not null)
                    {
                        foreach (var item in e.NewItems.OfType<T>())
                        {
                            OnItemAdded?.Invoke(item);
                        }
                    }
                    if (e.OldItems is not null && OnItemRemoved is not null)
                    {
                        foreach (var item in e.OldItems.OfType<T>())
                        {
                            OnItemRemoved?.Invoke(item);
                        }
                    }
                    break;
            }
        }
        protected override void ClearItems()
        {
            var tmp = this.ToList();
            base.ClearItems();
            if (OnItemRemoved is not null)
                tmp.ForEach(x => OnItemRemoved?.Invoke(x));
            OnItemsCleared?.Invoke(tmp);
        }


        protected virtual void OnPropertyChanged([CallerMemberName] string name = "") => OnPropertyChanged(new PropertyChangedEventArgs(name));
    }
}
