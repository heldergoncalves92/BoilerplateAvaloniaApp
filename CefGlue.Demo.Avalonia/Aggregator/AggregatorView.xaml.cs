using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using Avalonia.VisualTree;

namespace ServiceStudio.WebViewImplementation {
    internal partial class AggregatorView : UserControl {
        public AggregatorView() { }

        public AggregatorView(TabHeaderInfo tabHeaderInfo) {
            AvaloniaXamlLoader.Load(this);
            var btn = this.FindControl<Button>("btn");
            btn.Click += BtnOnClick;

            var btnModal = this.FindControl<Button>("btn-modal");
            btnModal.Click += BtnModalOnClick;
            
            var btnStandalone = this.FindControl<Button>("btn-standalone");
            btnStandalone.Click += BtnStandaloneOnClick;
            
            
            
            TabHeader = tabHeaderInfo;
        }

        private void BtnStandaloneOnClick(object sender, RoutedEventArgs e)
        {
            Task.Run(() => throw new Exception("User Forced Exception inside Task"));
            var w = new Window { Width = 200, Height = 200 };
            w.ExtendClientAreaChromeHints = ExtendClientAreaChromeHints.PreferSystemChrome;
            w.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            w.ShowActivated = true;
            w.Show();
        }
        
        private void BtnModalOnClick(object sender, RoutedEventArgs e)
        {
            var w = new Window { Width = 200, Height = 200 };
            w.ShowDialog(this.GetVisualRoot() as Window);
        }

        private void BtnOnClick(object sender, RoutedEventArgs e)
        {
            var w = new Window { Width = 200, Height = 200 };
            w.Show(this.GetVisualRoot() as Window);
        }

        public TabHeaderInfo TabHeader { get; }
    }
}
