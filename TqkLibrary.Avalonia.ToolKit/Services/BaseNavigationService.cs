using TqkLibrary.Avalonia.ToolKit.Collections.ObservableCollections;
using TqkLibrary.Avalonia.ToolKit.Interfaces.Services;
using TqkLibrary.Avalonia.ToolKit.ViewModels;

namespace TqkLibrary.Avalonia.ToolKit.Services
{
    public abstract partial class BaseNavigationService<TBaseViewModel> : ViewModelBase, INavigationService<TBaseViewModel>
    {
        TBaseViewModel? _currentView;
        public TBaseViewModel? CurrentView
        {
            get { return _currentView; }
            set { NavigateTo(value!); }
        }

        protected readonly DispatcherObservableCollection<TBaseViewModel> _back = new();
        protected readonly DispatcherObservableCollection<TBaseViewModel> _next = new();
        public IReadOnlyCollection<TBaseViewModel> Backs { get { return _back; } }
        public virtual bool IsBackAvalable => Backs.Any();
        public IReadOnlyCollection<TBaseViewModel> Nexts { get { return _next; } }
        public virtual bool IsNextAvalable => Nexts.Any();


        public BaseNavigationService()
        {
            _back.CollectionChanged += _back_CollectionChanged;
            _next.CollectionChanged += _next_CollectionChanged;
        }

        private void _back_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.OnPropertyChanged(nameof(IsBackAvalable));
        }
        private void _next_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.OnPropertyChanged(nameof(IsNextAvalable));
        }

        public virtual void NavigateTo(TBaseViewModel viewModel)
        {
            if (viewModel is null) throw new ArgumentNullException(nameof(viewModel));
            if (viewModel.Equals(CurrentView)) return;

            this.OnPropertyChanging(nameof(CurrentView));
            _next.Clear();
            var current = CurrentView;
            if (current is not null)
                _back.Add(current);
            _currentView = viewModel;
            this.OnPropertyChanged(nameof(CurrentView));
        }
        public virtual void NavigateTo<TViewModel>() where TViewModel : TBaseViewModel
            => NavigateTo(typeof(TViewModel));
        public abstract void NavigateTo(Type type);
        public virtual bool Back()
        {
            if (_back.Count > 0)
            {
                this.OnPropertyChanging(nameof(CurrentView));
                TBaseViewModel viewModel = _back.Last();
                _back.RemoveAt(_back.Count - 1);
                _next.Add(CurrentView!);
                _currentView = viewModel;
                this.OnPropertyChanged(nameof(CurrentView));
                return true;
            }
            return false;
        }
        public virtual bool Next()
        {
            if (_next.Count > 0)
            {
                this.OnPropertyChanging(nameof(CurrentView));
                TBaseViewModel viewModel = _next.Last();
                _next.RemoveAt(_back.Count - 1);
                _back.Add(CurrentView!);
                _currentView = viewModel;
                this.OnPropertyChanged(nameof(CurrentView));
                return true;
            }
            return false;
        }
    }
}
