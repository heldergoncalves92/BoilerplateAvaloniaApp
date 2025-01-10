using System;
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using ServiceStudio.WebViewImplementation.DriftChats.DisplayMode;
using ServiceStudio.WebViewImplementation.Framework;

namespace ServiceStudio.WebViewImplementation.DriftChats {
    internal class DriftChat : Window {
        public const double ShadowBorderThickness = 18;

        public const double EngagementWidth = 348;
        public const double EngagementHeight = 312;
        public const double ArrowWidth = 8;
        public const double Shadow = 18;
        public const double FullHeightWithEngagementAndShadow = EngagementHeight + Shadow;
        public const double EngagementWithArrowWidth = EngagementWidth + ArrowWidth;
        public const double ChatIconWidth = 88;
        public const double ChatIconHeight = 88;
        public const double ChatWidth = 352;
        public const double ChatHeight = 580;

        private const string ChatIconContainerName = "chatIconContainer";
        private const string ChatContainerBorderName = "chatContainerBorder";
        private const string ChatEngagementContainerName = "ChatEngagementContainer";

        private readonly Window windowView;

        public IDisplayMode CurrentDisplayMode { get; private set; } = new MinimizedDefault();

        public event Action UserClosed = () => { };

        public new event EventHandler<CancelEventArgs> Closing;

        public DriftChat() {
            AvaloniaXamlLoader.Load(this);
        }

        public DriftChat(Window windowView) {
            this.windowView = windowView;

            AvaloniaXamlLoader.Load(this);
            ApplyThemePseudoClasses();
        }

        public void Display(IDisplayMode displayMode) => Dispatcher.UIThread.ExecuteInUIThread(() => {
            var window = windowView as Window;
            if (window.IsVisible) {
                Show(window);
                Refresh(displayMode);
            }
        });

        public new void Close() => Dispatcher.UIThread.ExecuteInUIThread(() => base.Close());

        protected override void OnClosing(WindowClosingEventArgs e) {
            base.OnClosing(e);
            Closing?.Invoke(this, e);
        }

        private void Refresh(IDisplayMode displayMode) {
            var (width, height, bottomRightOffset) = displayMode.DetermineDimensions();

            SetDimensions(width, height);

            var chatIconContainer = this.FindControl<Panel>(ChatIconContainerName);
            var chatContainerBorder = this.FindControl<Border>(ChatContainerBorderName);
            var chatEngagementContainer = this.FindControl<StackPanel>(ChatEngagementContainerName);
            displayMode.SetVisible(chatIconContainer, chatContainerBorder, chatEngagementContainer);

            CurrentDisplayMode = displayMode;
        }

        private void ApplyThemePseudoClasses() {
            PseudoClasses.Set(":light", true);
        }

        private void OnMainChatIconClick(object sender, Avalonia.Interactivity.RoutedEventArgs e) {
            Refresh(new Open());
        }

        private void OnMinimizeButtonClick(object sender, Avalonia.Interactivity.RoutedEventArgs e) {
            Refresh(new MinimizedDefault());
        }

        private void OnChatIconCloseButtonClick(object sender, Avalonia.Interactivity.RoutedEventArgs e) {
            UserClosed();
            Close();
        }

        private void OnChatCloseButtonClick(object sender, Avalonia.Interactivity.RoutedEventArgs e) {
            UserClosed();
            Close();
        }

        private void SetDimensions(double width, double height) {
            Width = width;
            Height = height;
        }
    }
}
