using Avalonia.Threading;
using System.Runtime.Versioning;

namespace TqkLibrary.Avalonia.ToolKit.Interfaces
{
    public interface IMainThread
    {
        Dispatcher Dispatcher { get; }
    }
}
