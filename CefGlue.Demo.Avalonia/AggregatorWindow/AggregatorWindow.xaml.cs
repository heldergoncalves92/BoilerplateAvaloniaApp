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
using Avalonia.Platform;
using ServiceStudio.View;
using ServiceStudio.WebViewImplementation.Framework.Tooltip;

namespace ServiceStudio.WebViewImplementation {
    internal partial class AggregatorWindow : Window {
        
        private Action<ITopLevelView> selectedAggregatorChanged;

        public AggregatorWindow() {
            AvaloniaXamlLoader.Load(this);
            ExtendClientAreaToDecorationsHint = true;
            ExtendClientAreaChromeHints = ExtendClientAreaChromeHints.NoChrome;

        }

        public static readonly StyledProperty<Thickness> TitleBarMarginProperty =
            AvaloniaProperty.Register<AggregatorWindow, Thickness>(nameof(TitleBarMargin), defaultValue: new Thickness(0), inherits: true);

        public Thickness TitleBarMargin {
            get => GetValue(TitleBarMarginProperty);
            private set => SetValue(TitleBarMarginProperty, value);
        }

        private IEnumerable<TabItem> TabItems => Enumerable.Empty<TabItem>();

        //TODO HYBRID Finish
        private void OnSelectedTabChanged(object sender, SelectionChangedEventArgs e) {
            var tabItem = e.AddedItems.OfType<TabItem>().FirstOrDefault()?.Content as ITopLevelView;

            selectedAggregatorChanged?.Invoke(tabItem);
        }

        private void SelectTab(TabItem tabItem) {
            
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

        }

        private void RemoveTab(IAggregatorView aggregatorView) {
            
        }

        private void AddDragDropHandlers(TabItem tab) {
            Avalonia.Input.DragDrop.SetAllowDrop(tab, true);
            tab.AddHandler(Avalonia.Input.DragDrop.DragEnterEvent, OnDragEnter);
        }

        private void OnDragEnter(object sender, Avalonia.Input.DragEventArgs e) {
        }

        protected override void OnOpened(EventArgs e) {
            var nativeMenu = NativeMenu.GetMenu(this);
            var menuItem = new NativeMenuItem("AmazingNativeMenu");
            var menu = new NativeMenu();
            menu.NeedsUpdate += (_, __) => Console.WriteLine("Menu Needs To Update");
            
            var item2 = new NativeMenuItem();
            item2.Click += (a, b) => {
                Console.WriteLine("OnMenuItemClick Delete");
            };
            item2.Header = "Delete Amazing Action";
            item2.IsEnabled = true;
            menu.Items.Add(item2);

            var item = new NativeMenuItem();
            item.Click += (a, b) => {
                Console.WriteLine("OnMenuItemClick F4");
                item2.Gesture = item2.Gesture == null ? KeyGesture.Parse("Delete") : null;
            };
            item.Header = "F4 Amazing Action";
            item.IsEnabled = true;
            item.Gesture = KeyGesture.Parse("F4");
            menu.Items.Add(item);

            menuItem.Menu = menu;
            nativeMenu.Add(menuItem);
       }
    }
}
