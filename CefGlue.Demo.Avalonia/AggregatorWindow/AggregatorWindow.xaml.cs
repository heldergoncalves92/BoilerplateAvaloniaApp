using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ServiceStudio.View;
using ServiceStudio.WebViewImplementation.Framework.Tooltip;

namespace ServiceStudio.WebViewImplementation {
    internal partial class AggregatorWindow : BaseWindow {
        private readonly TabControl tabs;
        private Action<ITopLevelView> selectedAggregatorChanged;

        public AggregatorWindow() {
            AvaloniaXamlLoader.Load(this);
            tabs = this.FindControl<TabControl>("tabs");
            this.AttachDevTools();
            WindowState = WindowState.Maximized;
        }

        public static readonly StyledProperty<Thickness> TitleBarMarginProperty =
            AvaloniaProperty.Register<AggregatorWindow, Thickness>(nameof(TitleBarMargin), defaultValue: new Thickness(0), inherits: true);

        public Thickness TitleBarMargin {
            get => GetValue(TitleBarMarginProperty);
            private set {
                SetValue(TitleBarMarginProperty, value);
                SetValue(TabControlMarginProperty, new Thickness(value.Left + 101 + 14, 0, 0, 0));
            }
        }

        public static readonly StyledProperty<Thickness> TabControlMarginProperty =
            AvaloniaProperty.Register<AggregatorWindow, Thickness>(nameof(TabControlMargin), defaultValue: new Thickness(0), inherits: true);

        public Thickness TabControlMargin {
            get => GetValue(TabControlMarginProperty);
            private set => SetValue(TabControlMarginProperty, value);
        }

        private IEnumerable<TabItem> TabItems => tabs.Items.Cast<TabItem>();

        private void OnSelectedTabChanged(object sender, SelectionChangedEventArgs e) {
            var tabItem = e.AddedItems.OfType<TabItem>().FirstOrDefault()?.Content as ITopLevelView;

            selectedAggregatorChanged?.Invoke(tabItem);
        }

        private const int TitleBarHeight = 34;

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
            base.OnPropertyChanged(change);

            if (change.Property == WindowStateProperty) {
                var state = (WindowState)change.NewValue;
                ApplyTitleBarMargin(state);
                ExtendClientAreaTitleBarHeightHint = TitleBarHeight;
            }
        }

        private void ApplyTitleBarMargin(WindowState state) {
            TitleBarMargin = state switch {
                WindowState.FullScreen => new Thickness(16, 8, 0, 0),
                _ => new Thickness(72 + 16, 8, 0, 0),
            };
        }

        private void ShowTooltipFor(TabItem tabItem, TabHeaderInfo tabHeaderInfo, PointerEventArgs e) {
            var position = e.GetPosition(this);
            TooltipServiceProvider.ShowTooltip(this, "Amazing tooltip", position.X, position.Y, showDelayed: true);
        }

        private void OnTabCloseButtonClick(object sender, RoutedEventArgs e) {
            var button = (Button)sender;
            button.IsEnabled = false;

            var tabHeaderInfo = (TabHeaderInfo)button.DataContext;
            tabHeaderInfo?.TriggerClose().ContinueWith(t => button.IsEnabled = true, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private int GetTabIndex(ITopLevelView topLevelView) => Array.IndexOf(TabItems.Select(t => t.Content).ToArray(), topLevelView);

        private void AddTab(TabHeaderInfo header, Control view, bool isVisible = true, bool isEnabled = true) {
            var tab = new TabItem() {
                Content = view,
                DataContext = header,
                Header = header,
                IsVisible = isVisible,
                IsEnabled = isEnabled
            };

            var isMouseInside = false;

            void OnMouseLeft() {
                isMouseInside = false;
                TooltipServiceProvider.HideTooltip();
            }

            tab.PointerMoved += (sender, e) => {
                // in order to properly get the position we need to listen to pointer moved
                if (isMouseInside) {
                    return;
                }
                isMouseInside = true;
                ShowTooltipFor(tab, header, e);
            };
            tab.PointerExited += delegate { OnMouseLeft(); };
            tab.Tapped += delegate { OnMouseLeft(); };
            tab.LostFocus += delegate { OnMouseLeft(); };
            tab.PointerPressed += OnPointerPressed;
            AddDragDropHandlers(tab);

            var pos = ((IList)tabs.Items).Add(tab);
        }

        private void RemoveTab(IAggregatorView aggregatorView) {
            var index = GetTabIndex(aggregatorView);

            if (tabs.SelectedIndex == index) {
                tabs.SelectedIndex = index - 1;
                tabItemSelectedForDragDrop = null;
            }
            
            var currentTab = (TabItem)tabs.Items.ElementAt(index);
            currentTab.Content = null;
            currentTab.DataContext = null;
            currentTab.Header = null;
            
            tabs.Items.RemoveAt(index);
            TooltipServiceProvider.HideTooltip();
        }

        private void AddDragDropHandlers(TabItem tab) {
            Avalonia.Input.DragDrop.SetAllowDrop(tab, true);
            tab.AddHandler(Avalonia.Input.DragDrop.DragEnterEvent, OnDragEnter);
        }

        private void OnDragEnter(object sender, Avalonia.Input.DragEventArgs e) {
            tabs.SelectedIndex = GetTabIndex(((TabItem)sender).Content as ITopLevelView);
        }
    }
}
