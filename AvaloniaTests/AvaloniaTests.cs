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
    public void TestFineOn_Avalonia_v11_0_13() {
        // Arrange
        var topLevelViewMock = new MockUserControl();
        var dialogWindow = new Window();

        try {
            var args = new VisualTreeAttachmentEventArgs(dialogWindow, dialogWindow);

            // Act
            topLevelViewMock.RaiseAttachedFromVisualTree(args);
        } finally {
            dialogWindow.Close();
        }
    }
}