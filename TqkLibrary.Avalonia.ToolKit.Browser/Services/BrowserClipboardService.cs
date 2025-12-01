using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices.JavaScript;
using TqkLibrary.Avalonia.ToolKit.Interfaces.Services;
using TqkLibrary.Avalonia.ToolKit.Models;

namespace TqkLibrary.Avalonia.ToolKit.Browser.Services
{
    static unsafe partial class BrowserClipboardServiceHelper
    {
        [JSImport("writeText", "TqkLibrary.Avalonia.ToolKit.ClipboardHelper")]
        internal static partial Task writeText(string text);

        [JSImport("readText", "TqkLibrary.Avalonia.ToolKit.ClipboardHelper")]
        internal static partial Task<string?> readText();

        [JSImport("checkReadPermissions", "TqkLibrary.Avalonia.ToolKit.ClipboardHelper")]
        internal static partial Task<bool> checkReadPermissions();

        [JSImport("checkWritePermissions", "TqkLibrary.Avalonia.ToolKit.ClipboardHelper")]
        internal static partial Task<bool> checkWritePermissions();

        [JSImport("requestReadPermissions", "TqkLibrary.Avalonia.ToolKit.ClipboardHelper")]
        internal static partial Task<bool> requestReadPermissions();

        [JSImport("requestWritePermissions", "TqkLibrary.Avalonia.ToolKit.ClipboardHelper")]
        internal static partial Task<bool> requestWritePermissions();
    }
    public sealed class BrowserClipboardService : IClipboardService
    {
        public async Task<ClipboardPermission> RequestPermissionAsync(ClipboardPermission? request = null)
        {
            if (request is null)
                request = new() { Read = true, Write = true };
            if (request.Value.Write is null && request.Value.Read is null)
                return request.Value;

            bool isCanRead = false;
            bool isCanWrite = false;
            if (request.Value.Read == true)
            {
                isCanWrite = await BrowserClipboardServiceHelper.requestReadPermissions();
            }
            if (request.Value.Write == true)
            {
                isCanWrite = await BrowserClipboardServiceHelper.requestWritePermissions();
            }
            return new ClipboardPermission()
            {
                Read = isCanRead,
                Write = isCanWrite
            };
        }

        public async Task<ClipboardPermission> HasPermissionAsync()
        {
            bool isHasRead = await BrowserClipboardServiceHelper.checkReadPermissions();
            bool isHasWrite = await BrowserClipboardServiceHelper.checkWritePermissions();
            return new ClipboardPermission()
            {
                Read = isHasRead,
                Write = isHasWrite
            };
        }

        public Task SetTextAsync(string text) => BrowserClipboardServiceHelper.writeText(text);
        public Task<string?> GetTextAsync() => BrowserClipboardServiceHelper.readText();

    }
}
