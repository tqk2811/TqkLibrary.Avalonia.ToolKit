export function RegisterModule(dotnetRuntime) {
    dotnetRuntime.setModuleImports(
        'TqkLibrary.Avalonia.ToolKit.ClipboardHelper',
        {
            writeText: async (text) => {
                await globalThis.navigator.clipboard.writeText(text);
            },
            readText: async () => {
                return await globalThis.navigator.clipboard.readText();
            },
            checkReadPermissions: async () => {
                const readPermission = await globalThis.navigator.permissions.query({ name: 'clipboard-read' });
                return readPermission?.state == 'granted';
            },
            checkWritePermissions: async () => {
                const writePermission = await globalThis.navigator.permissions.query({ name: 'clipboard-write' });
                return writePermission?.state == 'granted';
            },
            requestReadPermissions: async () => {
                let readGranted = false;
                try {
                    await globalThis.navigator.clipboard.readText();
                    readGranted = true;
                } catch { }
                return readGranted;
            },
            requestWritePermissions: async () => {
                let writeGranted = false;
                try {
                    await globalThis.navigator.clipboard.writeText("permission-test");
                    writeGranted = true;
                } catch { }
                return writeGranted;
            }
        }
    )
    return dotnetRuntime;
}