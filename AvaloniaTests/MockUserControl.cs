using Avalonia;
using Avalonia.Controls;

namespace CefGlue.Demo.Avalonia.Tests;

public class MockUserControl : UserControl {
    public void RaiseAttachedFromVisualTree(VisualTreeAttachmentEventArgs e) {
        base.OnAttachedToVisualTreeCore(e);
    }
}