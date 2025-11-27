using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections;
using System.ComponentModel;
using TqkLibrary.Avalonia.Utils.Interfaces;

namespace TqkLibrary.Avalonia.Utils.ViewModels
{
    public abstract class ViewModelBase : ObservableObject, INotifyDataErrorInfo
    {
        #region INotifyDataErrorInfo
        public virtual bool HasErrors => _errors.Any();

        public virtual event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public virtual IEnumerable GetErrors(string? propertyName)
        {
            return propertyName != null && _errors.TryGetValue(propertyName, out var list)
                ? list
                : Enumerable.Empty<string>();
        }
        #endregion

        private readonly Dictionary<string, List<string>> _errors = new();
        protected virtual void AddError(string prop, string error)
        {
            _errors[prop] = new List<string>() { error };
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(prop));
        }
        protected virtual void AddError(string prop, IEnumerable<string> errors)
        {
            _errors[prop] = [.. errors];
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(prop));
        }
        protected virtual void ClearErrors(string prop)
        {
            if (_errors.Remove(prop))
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(prop));
        }
        protected virtual void ClearErrors()
        {
            var keys = _errors.Keys.ToList();
            _errors.Clear();
            foreach (var key in keys)
            {
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(key));
            }
        }
    }
    public abstract class ViewModelBase<TData> : ViewModelBase, IViewModelUpdate<TData>
    {
        readonly Action? _saveCallback;
        TData _data;
        public event ChangeCallBack<TData>? Change;
        public virtual TData Data { get { return _data; } }

        public ViewModelBase(TData data)
        {
            _data = data ?? throw new ArgumentNullException(nameof(data));
        }
        public ViewModelBase(TData data, Action saveCallback) : this(data)
        {
            this._saveCallback = saveCallback ?? throw new ArgumentNullException(nameof(saveCallback));
        }

        public virtual void Save()
        {
            _saveCallback?.Invoke();
            Change?.Invoke(this, Data);
        }

        public virtual void Update(TData data)
        {
            this._data = data ?? throw new ArgumentNullException(nameof(data));
        }
    }
}
