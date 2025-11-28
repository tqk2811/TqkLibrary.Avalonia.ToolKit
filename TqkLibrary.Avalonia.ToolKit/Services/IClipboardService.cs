using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Text;

namespace TqkLibrary.Avalonia.ToolKit.Services
{
    public interface IClipboardService
    {
        Task<bool> RequestPermissionAsync();
        Task<bool> HasPermissionAsync();

        Task SetTextAsync(string text);
        Task<string?> GetTextAsync();
    }
    public sealed class DesktopClipboardService : IClipboardService
    {
        private IClipboard? Clipboard =>
            (Application.Current?.ApplicationLifetime as ClassicDesktopStyleApplicationLifetime)?
            .MainWindow?.Clipboard;

        public Task<bool> RequestPermissionAsync() => Task.FromResult(Clipboard is not null);
        public Task<bool> HasPermissionAsync() => Task.FromResult(Clipboard is not null);

        public Task SetTextAsync(string text) => Clipboard?.SetTextAsync(text) ?? Task.CompletedTask;
        public Task<string?> GetTextAsync() => Clipboard?.TryGetTextAsync() ?? Task.FromResult<string?>(null);
    }

#if BROWSER
#if NET5_0_OR_GREATER
    [SupportedOSPlatform("browser")]
#endif
    public sealed class WebClipboardService : IClipboardService
    {
        public Task<bool> HasPermissionAsync() => Task.FromResult(true); // browser không có API kiểm tra thật sự

        public async Task<bool> RequestPermissionAsync()
        {
            try
            {
                // BrowserInterop đã wrap navigator.clipboard.writeText()
                await global::Avalonia.Browser.Interop.BrowserInterop.WriteTextToClipboard("test");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Task SetTextAsync(string text) =>
            Avalonia.Web.Interop.BrowserInterop.WriteTextToClipboard(text);

        public Task<string?> GetTextAsync() =>
            Avalonia.Web.Interop.BrowserInterop.ReadTextFromClipboard();
    }
#endif
}
