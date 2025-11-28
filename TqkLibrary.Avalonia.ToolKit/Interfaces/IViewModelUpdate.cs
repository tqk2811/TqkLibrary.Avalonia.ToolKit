namespace TqkLibrary.Avalonia.ToolKit.Interfaces
{
    public interface IViewModelUpdate<T> : IViewModel<T>
    {
        void Update(T data);
    }
}