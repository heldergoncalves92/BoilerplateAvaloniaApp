using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using ServiceStudio.WebViewImplementation.DriftChats;

namespace ServiceStudio.WebViewImplementation {
    internal partial class AggregatorView : UserControl {
        public AggregatorView() { }

        public AggregatorView(TabHeaderInfo tabHeaderInfo) {
            AvaloniaXamlLoader.Load(this);
            var btn = this.FindControl<Button>("btn");
            btn.Click += BtnOnClick;
            
            TabHeader = tabHeaderInfo;
        }

        private void BtnOnClick(object sender, RoutedEventArgs e)
        {
            var chat = new DriftChatAdapter(new DriftChat(this.GetVisualRoot() as Window));
            chat.Display(new View.DriftChats.DisplayModes.Minimized());
        }

        public TabHeaderInfo TabHeader { get; }
    }
}
