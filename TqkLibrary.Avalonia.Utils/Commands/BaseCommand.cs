using Avalonia;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TqkLibrary.Avalonia.Utils.Commands
{
    public class BaseCommand : ICommand
    {
        protected readonly Dispatcher _dispatcher;
        protected BaseCommand() : this(Dispatcher.UIThread)
        {

        }
        protected BaseCommand(Dispatcher dispatcher)
        {
            this._dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        }

        public BaseCommand(Action execute) : this(execute, () => true)
        {
        }
        public BaseCommand(Action execute, Func<bool> canExecute) : this(execute, canExecute, Dispatcher.UIThread)
        {
        }
        public BaseCommand(Action execute, Func<bool> canExecute, Dispatcher dispatcher)
        {
            this._actionExecute = execute ?? throw new ArgumentNullException(nameof(execute));
            this._canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
            this._dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        }




        public BaseCommand(Func<Task> execute) : this(execute, () => true, Dispatcher.UIThread)
        {

        }
        public BaseCommand(Func<Task> execute, Func<bool> canExecute) : this(execute, canExecute, Dispatcher.UIThread)
        {

        }
        public BaseCommand(Func<Task> execute, Func<bool> canExecute, Dispatcher dispatcher)
        {
            this._funcExecute = execute ?? throw new ArgumentNullException(nameof(execute));
            this._canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
            this._dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        }

        readonly Func<bool>? _canExecute;
        readonly Action? _actionExecute;
        readonly Func<Task>? _funcExecute;

        public virtual bool CanExecute(object? parameter)
        {
            if (IsLocked) return false;
            return _canExecute?.Invoke() ?? true;
        }

        public virtual async void Execute(object? parameter)
        {
            if (_actionExecute is not null)
            {
                _actionExecute.Invoke();
            }
            else
            {
                using var l = LockButton();
                Task task = _dispatcher.TrueThreadInvokeAsync(() => _funcExecute!.Invoke()).Unwrap();
                await task;
            }
        }

        public event EventHandler? CanExecuteChanged;
        public virtual void FireCanExecuteChanged(object? sender, EventArgs e)
        {
            _ = _dispatcher.TrueThreadInvokeAsync(() => CanExecuteChanged?.Invoke(sender, e));
        }
        public virtual void FireCanExecuteChanged() => FireCanExecuteChanged(null, EventArgs.Empty);


        public virtual IDisposable LockButton() => new ButtonDisposable(this);

        protected bool IsLocked { get; private set; } = false;
        private class ButtonDisposable : IDisposable
        {
            readonly BaseCommand _baseCommand;
            public ButtonDisposable(BaseCommand baseCommand)
            {
                this._baseCommand = baseCommand ?? throw new ArgumentNullException(nameof(baseCommand));
                _ForceLock();
            }
            ~ButtonDisposable()
            {
                _ForceRelease();
            }
            public void Dispose()
            {
                _ForceRelease();
                GC.SuppressFinalize(this);
            }
            void _ForceLock()
            {
                if (this._baseCommand.IsLocked) throw new InvalidOperationException($"This button was locked");
                this._baseCommand.IsLocked = true;
                this._baseCommand.FireCanExecuteChanged();
            }
            void _ForceRelease()
            {
                if (!this._baseCommand.IsLocked) throw new InvalidOperationException($"This button was unlocked");
                this._baseCommand.IsLocked = false;
                this._baseCommand.FireCanExecuteChanged();
            }
        }
    }
    public class BaseCommand<TParam> : BaseCommand
    {
        public BaseCommand(Action<TParam> execute) : this(execute, (p) => true)
        {
        }
        public BaseCommand(Action<TParam> execute, Func<TParam, bool> canExecute)
        {
            this._actionExecute = execute ?? throw new ArgumentNullException(nameof(execute));
            this._canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
        }

        public BaseCommand(Func<TParam, Task> execute) : this(execute, (p) => true)
        {
        }
        public BaseCommand(Func<TParam, Task> execute, Func<TParam, bool> canExecute)
        {
            this._funcExecute = execute ?? throw new ArgumentNullException(nameof(execute));
            this._canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
        }


        readonly Func<TParam, bool> _canExecute;
        readonly Action<TParam>? _actionExecute;
        readonly Func<TParam,Task>? _funcExecute;

        public override bool CanExecute(object? parameter)
        {
            if (IsLocked) return false;
            return _canExecute.Invoke((TParam)parameter!);
        }

        public override async void Execute(object? parameter)
        {
            if (_actionExecute is not null)
            {
                _actionExecute.Invoke((TParam)parameter!);
            }
            else
            {
                using var l = LockButton();
                Task task = _dispatcher.TrueThreadInvokeAsync(() => _funcExecute!.Invoke((TParam)parameter!)).Unwrap();
                await task;
            }
        }
    }
}
