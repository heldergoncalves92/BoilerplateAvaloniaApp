using System;
using Avalonia;
using Avalonia.Controls;
using ReactViewControl;

namespace CefGlue.Demo.Avalonia;

public partial class ExtendedReactView : ReactView {

    protected override ReactViewFactory Factory => new ExtendedReactViewFactory();

    public ExtendedReactView(IViewModule mainModule) : base(mainModule) {
        this.AttachedToVisualTree += OnViewAttachedToVisualTree;
        this.DetachedFromVisualTree += OnViewDetachedToVisualTree;
    }

    protected override void InnerDispose() {
        base.InnerDispose();
        AttachedToVisualTree -= OnViewAttachedToVisualTree;
        DetachedFromVisualTree -= OnViewDetachedToVisualTree;
    }
        
    private void OnViewAttachedToVisualTree(object sender, VisualTreeAttachmentEventArgs e) {
        if (e.Root is WindowBase windowBase) {
            windowBase.Activated += OnActivated;
        }
    }

    private void OnViewDetachedToVisualTree(object sender, VisualTreeAttachmentEventArgs e) {
        if (e.Root is WindowBase windowBase) {
            windowBase.Activated -= OnActivated;
        }
    }

    private void OnActivated(object sender, EventArgs e) {
        Focus();
    }
}