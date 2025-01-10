using Avalonia.Controls;

namespace ServiceStudio.WebViewImplementation.DriftChats.DisplayMode {

    internal class MinimizedDefault : Minimized {

        public override (double width, double height, int bottomRightOffset) DetermineDimensions()
            => (DriftChat.ChatIconWidth, DriftChat.ChatIconHeight, -12);

        public override void SetVisible(Panel chatIconContainer, Border chatContainerBorder, StackPanel chatEngagementContainer) {
            chatIconContainer.IsVisible = true;
            chatContainerBorder.IsVisible = false;
            chatEngagementContainer.IsVisible = false;
        }
    }
}
