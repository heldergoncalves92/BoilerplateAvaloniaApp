using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.NUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using ServiceStudio.View;
using ServiceStudio.WebViewImplementation;

namespace Avalonia.UnitTests;

public class MouseDownUnitTest
{
    [AvaloniaTest]
    public void Test()
    {
        var aggregatorWindow = new AggregatorWindow();
        var aggregatorWindowView = aggregatorWindow as IAggregatorWindowView;
        aggregatorWindowView.CreateAggregatorView();
        aggregatorWindowView.CreateAggregatorView();
        Dispatcher.UIThread.RunJobs();
         
        aggregatorWindow.Show();
        Dispatcher.UIThread.RunJobs();
        
        var tab = aggregatorWindow.TabItems.ToArray()[1];
        
        var point = tab.TranslatePoint(new Point(1, 1), aggregatorWindow);
        
        // Act
        aggregatorWindow.MouseDown(point.Value, MouseButton.Right);
        
        // Assert
        var contextMenu = aggregatorWindow.FindDescendantOfType<ContextMenu>();
        Assert.IsNotNull(contextMenu);
    }
}