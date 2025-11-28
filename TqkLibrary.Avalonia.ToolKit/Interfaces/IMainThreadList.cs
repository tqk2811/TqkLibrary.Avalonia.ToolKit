using System.Collections.Generic;

namespace TqkLibrary.Avalonia.ToolKit.Interfaces
{
    public interface IMainThreadList<T> : IMainThread, IList<T>, IMainThreadCollection<T>
    {

    }
}
