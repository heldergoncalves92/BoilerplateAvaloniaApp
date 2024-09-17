using Avalonia;
using Avalonia.Headless.NUnit;
using Avalonia.Controls;
using NUnit.Framework;

namespace CefGlue.Demo.Avalonia.Tests;

public class AvaloniaTests {

    [Test]
    public void Test1() {
        Console.WriteLine("RUM");
    }

    [AvaloniaTest]
    public void Test3() {
        // Arrange
        var topLevelViewMock = new MockUserControl();
        var dialogWindow = new Window();
        var args = new VisualTreeAttachmentEventArgs(dialogWindow, dialogWindow);

        // Act
        topLevelViewMock.RaiseAttachedFromVisualTree(args);
    }
}
