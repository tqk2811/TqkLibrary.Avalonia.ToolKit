using System.Diagnostics;
using System.Timers;
using TqkLibrary.Avalonia.ToolKit.Interfaces;
using TqkLibrary.Avalonia.ToolKit.Interfaces.Services;
using TqkLibrary.Utils;

namespace TqkLibrary.Avalonia.ToolKit.Collections.ObservableCollections
{
    public class SaveStorageObservableCollection<TData, TViewModel> : SaveObservableCollection<TData, TViewModel>
      where TData : class
      where TViewModel : class, IViewModel<TData>
    {
        readonly IDataStorageService _dataStorageService;
        readonly Func<TData, TViewModel> _func;
        readonly string _key;
        readonly System.Timers.Timer _timer;
        public SaveStorageObservableCollection(
            IDataStorageService dataStorageService,
            Func<TData, TViewModel> func,
            string? key = null
            )
        {
            this._dataStorageService = dataStorageService ?? throw new ArgumentNullException(nameof(dataStorageService));
            this._func = func ?? throw new ArgumentNullException(nameof(func));
            this._key = key ?? typeof(List<TData>).GetNameFixed(true);
            this._timer = new System.Timers.Timer(0.5);
            this._timer.AutoReset = false;
            this._timer.Elapsed += _timer_Elapsed;
            this.OnSave += SaveStorageObservableCollection_OnSave;
            _ = PreLoad();
        }

        List<TData>? _datas = null;
        private async void SaveStorageObservableCollection_OnSave(UpdateData updateData)
        {
            this._timer.Stop();
            this._datas = updateData.CurrentDatas.ToList();
            this._timer.Start();
        }
        private async void _timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            var datas = _datas;
            if (datas is not null)
            {
                try
                {
                    await _dataStorageService.SetAsync(_key, _datas);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error on save: {ex.GetType().FullName}: {ex.Message}{Environment.NewLine}{ex.StackTrace}");
                }
            }
        }

        async Task PreLoad()
        {
            try
            {
                var datas = await _dataStorageService.GetAsync<List<TData>>(_key);
                if (datas is not null)
                    await this.LoadAsync(datas, _func);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error on {nameof(PreLoad)}: {ex.GetType().FullName}: {ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }
    }
}