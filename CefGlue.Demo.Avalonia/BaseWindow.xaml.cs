using System;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;

namespace ServiceStudio.WebViewImplementation {

    public class BaseWindow : Window {

        protected readonly WindowState windowState;

        private readonly string windowTitle;
        private readonly string windowType;
        private Guid contextualDispatcherFrameId;

        public BaseWindow() : this(WindowState.Maximized, null, null, nameof(BaseWindow), null) { }

        public BaseWindow(
            WindowState windowState,
            string windowTitle,
            string id,
            string windowType,
            string windowOpenActivityId) {
            AvaloniaXamlLoader.Load(this);
            this.windowState = windowState;
            RefreshExtendClientAreaChromeHints();
            SetPseudoOSClasses();

            this.windowTitle = windowTitle;
            this.windowType = windowType;
        }

        protected void RefreshExtendClientAreaChromeHints(bool isTitleBarVisible = true) {
            ExtendClientAreaChromeHints = isTitleBarVisible
                ? ExtendClientAreaChromeHints.PreferSystemChrome
                : ExtendClientAreaChromeHints.NoChrome;
        }

        protected event Action popInClicked;

        private void OnPopInButtonClick(object sender, RoutedEventArgs e) {
            popInClicked?.Invoke();
        }

        /// <summary>
        /// This property EnableClose, is implemented down in the Windows OS native code of the Avalonia package,
        /// NOT yet implemented in the Mac OSX (a PR is being worked to submit to Avalonia team for OSX).
        /// NOTE: This ONLY handles the Close (X) button. This does NOT (presently) handle (enable/disable) the other 'Close' type
        /// of actions (Windows: Alt-F4, left-menu (alt-space) Close; OSX: Cmd-W, Dock -> right-click-> Quit)
        /// Part of Avalonia PR submittal will be to raise the question to whether they will keep/bring forward micro/granular
        /// control to enable/disable each of the system buttons and also allow or their counterpart methods - or, do it as an
        /// all or nothing approach. (E.g., EnableClose=false would disable all methods to close, not just button).
        /// Repeat: This is PROPERTY setting (true/false). In upper levels we separate into EnableClose()/DisableClose methods.
        /// </summary>
        public static readonly StyledProperty<bool> EnableCloseProperty =
            AvaloniaProperty.Register<BaseWindow, bool>(nameof(EnableClose), defaultValue: true, inherits: true);

        public bool EnableClose {
            get => GetValue(EnableCloseProperty);
            set => SetValue(EnableCloseProperty, value);
        }

        public static readonly StyledProperty<bool> AllowMinimizeProperty =
           AvaloniaProperty.Register<BaseWindow, bool>(nameof(AllowMinimize), defaultValue: true, inherits: true);

        public bool AllowMinimize {
            get => GetValue(AllowMinimizeProperty);
            set => SetValue(AllowMinimizeProperty, value);
        }

        public static readonly StyledProperty<bool> AllowRestoreOrMaximizeProperty =
           AvaloniaProperty.Register<BaseWindow, bool>(nameof(AllowRestoreOrMaximize), defaultValue: true, inherits: true, coerce: CanRestoreOrMaximize);

        public bool AllowRestoreOrMaximize {
            get => GetValue(AllowRestoreOrMaximizeProperty);
            set => SetValue(AllowRestoreOrMaximizeProperty, value);
        }

        private static bool CanRestoreOrMaximize(AvaloniaObject window, bool allowRestoreOrMaximize) {
            return allowRestoreOrMaximize && ((BaseWindow)window).CanResize;
        }

        public static readonly StyledProperty<bool> ShowCloseProperty =
           AvaloniaProperty.Register<BaseWindow, bool>(nameof(ShowClose), defaultValue: true, inherits: true);

        public bool ShowClose {
            get => GetValue(ShowCloseProperty);
            set => SetValue(ShowCloseProperty, value);
        }

        public static readonly StyledProperty<bool> ShowPopInProperty =
           AvaloniaProperty.Register<BaseWindow, bool>(nameof(ShowPopIn), defaultValue: false, inherits: true);

        public bool ShowPopIn {
            get => GetValue(ShowPopInProperty);
            set => SetValue(ShowPopInProperty, value);
        }

        private bool ShouldTrackWindowState => windowState != null && WindowState == WindowState.Normal;

        protected override void OnClosing(WindowClosingEventArgs e) {
            base.OnClosing(e);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                // there is a bug on windows and the app loses focus
                // when another app was activated previously, and so we have to activate the owner
                if (!e.Cancel && IsActive) {
                    Owner?.Activate();
                }
            }
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
            base.OnPropertyChanged(change);

            if (change.Property == WindowStateProperty) {
                var state = (WindowState)change.NewValue;
                PseudoClasses.Set(":maximized", state == WindowState.Maximized);

                // if (ShouldTrackWindowState) {
                //     windowState.SuspendNotificationsIn(() => {
                //         windowState.IsMaximized = state == WindowState.Maximized;
                //     });
                // }
            }
        }


        private void SetPseudoOSClasses() {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                PseudoClasses.Add(":win");

                // https://learn.microsoft.com/en-us/answers/questions/605991/how-to-differentiate-between-windows-10-and-window
                var windowsVersion = Environment.OSVersion.Version;
                if (windowsVersion.Major == 10 && windowsVersion.Build < 22000) {
                    PseudoClasses.Add(":win10");
                }
            } else {
                PseudoClasses.Add(":osx");
            }
        }
    }
}
