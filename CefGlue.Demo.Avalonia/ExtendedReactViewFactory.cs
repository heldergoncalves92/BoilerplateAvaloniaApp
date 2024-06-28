using ReactViewControl;
using WebViewControl;

namespace CefGlue.Demo.Avalonia;

partial class ExtendedReactView {

    protected class ExtendedReactViewFactory : ReactViewFactory {

        public override ResourceUrl DefaultStyleSheet =>
            new(typeof(ExtendedReactViewFactory).Assembly, "Generated");

        public override bool ShowDeveloperTools => false;

        public override bool EnableViewPreload => true;

#if DEBUG
        public override bool EnableDebugMode => true;
#endif
    }
}