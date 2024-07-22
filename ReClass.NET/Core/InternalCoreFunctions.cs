using System.Runtime.InteropServices;
using ReClassNET.Extensions;
using ReClassNET.Native;
using ReClassNET.Util;

namespace ReClassNET.Core;

public delegate bool EnumerateInstructionCallback(ref InstructionData data);

internal class InternalCoreFunctions : NativeCoreWrapper, IInternalCoreFunctions, IDisposable {
    private const string CoreFunctionsModuleWindows = "NativeCore.dll";

    private static readonly Keys[] empty = [];

    private readonly GetPressedKeysDelegate getPressedKeysDelegate;

    private readonly IntPtr handle;

    private readonly InitializeInputDelegate initializeInputDelegate;
    private readonly ReleaseInputDelegate releaseInputDelegate;

    private InternalCoreFunctions(IntPtr handle)
        : base(handle) {
        this.handle = handle;

        initializeInputDelegate = GetFunctionDelegate<InitializeInputDelegate>(handle, "InitializeInput");
        getPressedKeysDelegate = GetFunctionDelegate<GetPressedKeysDelegate>(handle, "GetPressedKeys");
        releaseInputDelegate = GetFunctionDelegate<ReleaseInputDelegate>(handle, "ReleaseInput");
    }

    public IntPtr InitializeInput() => initializeInputDelegate();

    public Keys[] GetPressedKeys(IntPtr handle) {
        if (!getPressedKeysDelegate(handle, out var buffer, out var length) || length == 0) {
            return empty;
        }

        var keys = new int[length];
        Marshal.Copy(buffer, keys, 0, length);
        return (Keys[])(object)keys; // Yes, it's legal...
        //return Array.ConvertAll(keys, k => (Keys)k);
    }

    public void ReleaseInput(IntPtr handle) {
        releaseInputDelegate(handle);
    }

    public static InternalCoreFunctions Create() {
        var libraryName = CoreFunctionsModuleWindows;
        var libraryPath = Path.Combine(PathUtil.ExecutableFolderPath, libraryName);

        var handle = NativeMethods.LoadLibrary(libraryPath);
        if (handle.IsNull()) {
            throw new FileNotFoundException($"Failed to load native core functions! Couldnt find at location {libraryPath}");
        }

        return new InternalCoreFunctions(handle);
    }

    private delegate IntPtr InitializeInputDelegate();

    private delegate bool GetPressedKeysDelegate(IntPtr handle, out IntPtr pressedKeysArrayPtr, out int length);

    private delegate void ReleaseInputDelegate(IntPtr handle);

    #region IDisposable Support

    ~InternalCoreFunctions() {
        ReleaseUnmanagedResources();
    }

    private void ReleaseUnmanagedResources() {
        NativeMethods.FreeLibrary(handle);
    }

    public void Dispose() {
        ReleaseUnmanagedResources();

        GC.SuppressFinalize(this);
    }

    #endregion

}
