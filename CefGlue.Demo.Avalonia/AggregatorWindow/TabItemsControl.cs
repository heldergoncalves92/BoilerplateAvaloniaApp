using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.VisualTree;

namespace ServiceStudio.WebViewImplementation {

    internal class TabItemsControl : StackPanel {

        private const int WinCaptionButtonsWidth = 130;
        private const int TabItemMaxWidth = 232;
        private const int ToggleButtonMarginLeft = 4;
        private const int ToggleButtonMarginRight = 16;
        private const double ToggleButtonWidth = 24 + ToggleButtonMarginLeft + ToggleButtonMarginRight;

        public TabItemsControl() {
            Orientation = Orientation.Horizontal;
        }

        protected override void ChildrenChanged(object sender, NotifyCollectionChangedEventArgs e) {
            base.ChildrenChanged(sender, e);
            if (e.NewItems != null) {
                foreach (var newItem in e.NewItems.OfType<Layoutable>()) {
                    var tabHeaderInfo = (TabHeaderInfo)newItem.DataContext;
                    if (!tabHeaderInfo.IsFixedSize) {
                        newItem.MaxWidth = TabItemMaxWidth;
                    }
                }
            }
        }

        protected override Size MeasureOverride(Size availableSize) {
            var childrenWithFixedSizeWidth = 0.0;
            var height = 0.0;
            var childrenWithVariableSize = new List<Control>();

            foreach (var child in Children) {
                var tabHeaderInfo = (TabHeaderInfo)child.DataContext;
                if (tabHeaderInfo.IsFixedSize) {
                    child.Measure(availableSize);
                    childrenWithFixedSizeWidth += child.DesiredSize.Width;
                    height = Math.Max(child.DesiredSize.Height, height);
                } else {
                    childrenWithVariableSize.Add(child);
                }
            }

            var visualParent = this.GetVisualParent<Control>();
            // Fetch ToggleButton width and/if others controls after this
            var siblingsAfter = visualParent.GetVisualChildren().OfType<Layoutable>().SkipWhile(child => child != this).Skip(1).ToArray();
            var siblingsTotalWidth = 0.0;
            foreach (var sibling in siblingsAfter) {
                if (!sibling.IsMeasureValid) {
                    sibling.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                }
                siblingsTotalWidth += sibling.DesiredSize.Width;
            }

            // availableSize is not taking Windows buttons into account in Avalonia11
            var availableWidth = availableSize.Width - childrenWithFixedSizeWidth - siblingsTotalWidth - ToggleButtonWidth;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                availableWidth -= WinCaptionButtonsWidth;
            }

            var tabsWidth = childrenWithFixedSizeWidth;

            if (availableWidth > 0 && childrenWithVariableSize.Count > 0) {
                var tabItemAvailableMaxWidth = availableWidth / childrenWithVariableSize.Count;

                foreach (var child in childrenWithVariableSize) {
                    child.Measure(new Size(tabItemAvailableMaxWidth, double.PositiveInfinity));
                    tabsWidth += child.DesiredSize.Width;
                    height = Math.Max(child.DesiredSize.Height, height);
                }
            }

            var window = this.GetVisualAncestors().OfType<Window>().FirstOrDefault();

            var toggleButton = window?.FindControl<ToggleButton>("Toggle");

            var toggleButtonMarginLeft = this.Margin.Left + tabsWidth + ToggleButtonMarginLeft;

            if (toggleButton != null) {
                toggleButton.Margin = new Thickness(toggleButtonMarginLeft, 4, ToggleButtonMarginRight, 0);
            }

            return new Size(tabsWidth, height);
        }
    }
}
