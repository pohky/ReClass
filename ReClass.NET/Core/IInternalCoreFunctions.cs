namespace ReClassNET.Core;

public interface IInternalCoreFunctions {
    IntPtr InitializeInput();

    Keys[] GetPressedKeys(IntPtr handle);

    void ReleaseInput(IntPtr handle);
}
