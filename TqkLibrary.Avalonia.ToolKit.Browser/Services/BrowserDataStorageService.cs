using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;
using TqkLibrary.Avalonia.ToolKit.Interfaces.Services;
using TqkLibrary.Utils;

namespace TqkLibrary.Avalonia.ToolKit.Browser.Services
{
    static unsafe partial class BrowserDataStorageServiceHelper
    {
        [JSImport("setItem", "TqkLibrary.Avalonia.ToolKit.LocalStorageHelper")]
        internal static partial void SetLocalStorage(string key, string json);

        [JSImport("getItem", "TqkLibrary.Avalonia.ToolKit.LocalStorageHelper")]
        internal static partial string? GetLocalStorage(string key);
    }

    [RequiresUnreferencedCode(MiscellaneousUtils.TrimWarning)]
    [RequiresDynamicCode(MiscellaneousUtils.AotWarning)]
    public class BrowserDataStorageService : IDataStorageService
    {
        public bool IsFullTypeName { get; set; } = true;
        public virtual Task<TData?> GetAsync<TData>()
        {
            return GetAsync<TData>(typeof(TData).GetName(IsFullTypeName));
        }

        public virtual Task<TData?> GetAsync<TData>(string key)
        {
            string? json = BrowserDataStorageServiceHelper.GetLocalStorage(key);
            if (string.IsNullOrWhiteSpace(json)) return Task.FromResult<TData?>(default);
            return Task.FromResult<TData?>(JsonConvert.DeserializeObject<TData>(json));
        }

        public virtual Task SetAsync<TData>(TData value)
        {
            return SetAsync<TData>(typeof(TData).GetName(IsFullTypeName), value);
        }

        public virtual Task SetAsync<TData>(string key, TData value)
        {
            BrowserDataStorageServiceHelper.SetLocalStorage(key, JsonConvert.SerializeObject(value));
            return Task.CompletedTask;
        }
    }
}
