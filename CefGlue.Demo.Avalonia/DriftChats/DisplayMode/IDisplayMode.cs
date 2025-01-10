using Avalonia.Controls;

namespace ServiceStudio.WebViewImplementation.DriftChats.DisplayMode {
    internal interface IDisplayMode {

        (double width, double height, int bottomRightOffset) DetermineDimensions();

        void SetVisible(Panel chatIconContainer, Border chatContainerBorder, StackPanel chatEngagementContainer);
    }
}
