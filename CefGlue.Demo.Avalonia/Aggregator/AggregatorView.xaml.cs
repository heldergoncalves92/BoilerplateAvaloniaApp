using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ServiceStudio.WebViewImplementation {
    internal partial class AggregatorView : UserControl {
        public AggregatorView() { }

        public AggregatorView(TabHeaderInfo tabHeaderInfo) {
            AvaloniaXamlLoader.Load(this);
            TabHeader = tabHeaderInfo;
        }

        public TabHeaderInfo TabHeader { get; }
    }
}
