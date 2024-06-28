using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using CefGlue.Demo.Avalonia;

namespace Avalonia.UnitTests;

public class UnitTestUsingAvaloniaTestAttr
{
    [SetUp]
    public void Setup() {
    }

    [AvaloniaTest]
    public void Test1() {
        Assert.Pass();
    }
    
    [AvaloniaTest]
    [Repeat(10)]
    public async Task Test3()
    {
        var view = new ToolTip();
        var content = new ContentControl { Content = "Test" };
        view.Content = content;
        await Task.Delay(100);
        view.SetValue(ToolTip.IsVisibleProperty, true);
        Assert.That(view.IsVisible, Is.EqualTo(true));
    }
    [AvaloniaTest]
    public void Test4()
    {
        var view = new TabView(1);
        view.ToggleIsEnabled();
        Assert.That((view.Content as MainView).IsEnabled, Is.EqualTo(false));
    }

    [AvaloniaTest]
    [Repeat(100)]
    public async Task Test6() {
        await Task.Delay(10);
        var dialog = new Window();
            
        var res = dialog.Position;
        
        Assert.That(res.X, Is.EqualTo(0));
    }
    [AvaloniaTest]
    [Repeat(50)]
    public void Test7() {
        Assert.Pass();
    }
}