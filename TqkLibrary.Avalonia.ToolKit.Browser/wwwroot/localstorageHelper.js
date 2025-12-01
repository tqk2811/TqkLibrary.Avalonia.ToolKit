export function RegisterModule(dotnetRuntime){
    dotnetRuntime.setModuleImports(
        'TqkLibrary.Avalonia.ToolKit.LocalStorageHelper',
        {
            setItem: (key, value) => globalThis.localStorage.setItem(key, value),
            getItem: (key) => globalThis.localStorage.getItem(key),
        }
    )
    return dotnetRuntime;
}